using UnityEngine;
using UnityEngine.UI;
using AGXUnity;
using AGXUnity.Utils;
using TMPro;
using System.Collections.Generic;
using System;
using UnityEngine.InputSystem.XInput;
using UnityEngine.InputSystem;

public class ExcavatorScript : MonoBehaviour
{
    public enum trackParts { left_sprocket_wheel, right_sprocket_wheel }
    // public enum armParts {scoop1}
    public enum armParts { full_arm_rotation, lower_arm, upperToLow, scoop1, tankSpin_wheel, plow1}
    public agx.Vec3 rbf = new agx.Vec3();
    public agx.Vec3 rbt = new agx.Vec3();
    private Gamepad gamepad;

    // Track Variables
    private float forwardSpeed = 3f;
    private TargetSpeedController leftController;
    private TargetSpeedController rightController;
    private float leftTrackSpeed = 0f;
    private float rightTrackSpeed = 0f;
    private bool trackCommandReceived = false;
    private float commandTimeOut = 1.0f;
    private float lastCommandTime = 0f;

    // Arm Variables
    private int selectedArmIndex = 0;
    private GameObject[] armGameObjects;
    private TargetSpeedController[] armControllers;
    public Text selectedArmText;
    private float armMoveSpeed = 0.4f;

    // Force and Torque Logging
    public Dictionary<string, ForceData> armForces = new Dictionary<string, ForceData>();
    public struct ForceData
    {
        public float Time;
        public string RigidBody;
        public Vector3 Force;
        public Vector3 Torque;

        public ForceData(float time, string rigidBody, Vector3 force, Vector3 torque) {
            Time = time;
            RigidBody = rigidBody;
            Force = force;
            Torque = torque;
        }
    }

    public Dictionary<string, ForceData> GetArmForces() {
        return armForces;
    }

    // Arm parts mapper
    private static Dictionary<armParts, string> armPartNames = new Dictionary<armParts, string> {
        { armParts.full_arm_rotation, "Swing" },
        { armParts.lower_arm, "Boom" },
        { armParts.upperToLow, "Arm" },
        { armParts.scoop1, "Bucket" },
        { armParts.tankSpin_wheel, "Slew" },
        { armParts.plow1, "Plow" }
    };

    public static string GetDisplayName(armParts part) {
        return armPartNames[part];
    }

    void Start()
    {
        // Transform parentTransform = transform.parent;
        gamepad = Gamepad.current;
        // Initialize track parts
        foreach (trackParts part in Enum.GetValues(typeof(trackParts)))
        {
            GameObject wheel = GameObject.Find(part.ToString());
            var wheelConstraint = wheel.GetComponent<AGXUnity.Constraint>();

            if (part == trackParts.left_sprocket_wheel)
            {
                leftController = wheelConstraint.GetController<TargetSpeedController>();
            }
            else if (part == trackParts.right_sprocket_wheel)
            {
                rightController = wheelConstraint.GetController<TargetSpeedController>();
            }
        }

        // Initialize arm parts and controllers
        armGameObjects = new GameObject[Enum.GetValues(typeof(armParts)).Length];
        armControllers = new TargetSpeedController[armGameObjects.Length];

        foreach (armParts part in Enum.GetValues(typeof(armParts)))
        {
            GameObject armPart = GameObject.Find(part.ToString());
            armGameObjects[(int)part] = armPart;

            if (armPart != null)
            {
                var armConstraint = armPart.GetComponent<AGXUnity.Constraint>();
                if (armConstraint != null)
                {
                    armControllers[(int)part] = armConstraint.GetController<TargetSpeedController>();

                    // Enable force computation
                    var nativeConstraint = armConstraint.Native as agx.Constraint;
                    if (nativeConstraint != null)
                    {
                        nativeConstraint.setEnableComputeForces(true);
                    }
                }
            }
        }
        UpdateSelectedArmText();
    }

    void Update()
    {
        if (!trackCommandReceived)
        {
            // Handle WASD movement for tracks
            float forwardInput = Input.GetKey(KeyCode.W) ? 1.0f : (Input.GetKey(KeyCode.S) ? -1.0f : 0);
            float turnInput = Input.GetKey(KeyCode.A) ? -1.0f : (Input.GetKey(KeyCode.D) ? 1.0f : 0);

            //Handle GamePad
            if(gamepad != null) {
                Vector2 leftStick = gamepad.leftStick.ReadValue();
                forwardInput = leftStick.y * forwardSpeed;
                turnInput = leftStick.x * forwardSpeed;
            }

            leftTrackSpeed = forwardSpeed * (forwardInput + turnInput);
            rightTrackSpeed = forwardSpeed * (forwardInput - turnInput);

            float maxSpeed = Mathf.Max(Mathf.Abs(leftTrackSpeed), Mathf.Abs(rightTrackSpeed));
            if (maxSpeed > forwardSpeed)
            {
                leftTrackSpeed *= forwardSpeed / maxSpeed;
                rightTrackSpeed *= forwardSpeed / maxSpeed;
            }
            trackCommandReceived = false;
        }

        if(trackCommandReceived && (Time.time - lastCommandTime) > commandTimeOut) {
            trackCommandReceived = false;
        }

        // Apply the speeds to the controllers....
        if (leftController != null && rightController != null)
        {
            leftController.Speed = leftTrackSpeed;
            rightController.Speed = rightTrackSpeed;
        }

        // Handle left and right arrow key input for arm selection
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SelectPreviousArmPart();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            SelectNextArmPart();
        }

        // Handle up and down arrow key input for moving the selected arm part
        HandleArmMovement();


        //Gamepad Control for manipulation
        if(gamepad != null) {
            gamepadSelectedArm();
            handleSlewGamepad();
        }

        // Log the forces and torques applied to the arm parts
        LogArmForces();
    }

    void HandleArmMovement()
    {
        // Keyboard controls
        if (Input.GetKey(KeyCode.UpArrow)) SetArmSpeed(armMoveSpeed);
        else if (Input.GetKey(KeyCode.DownArrow)) SetArmSpeed(-armMoveSpeed);
        else SetArmSpeed(0);

        //Gamepad controls
        if(gamepad != null) {
            var rightTrigger = gamepad.rightTrigger.ReadValue();
            var leftTrigger  =  gamepad.leftTrigger.ReadValue();
            float armSpeed = (rightTrigger - leftTrigger) * armMoveSpeed;
            SetArmSpeed(armSpeed);

            var rightBumper = gamepad.rightShoulder.ReadValue();
            var leftBumper  =  gamepad.leftShoulder.ReadValue();
            float slewSpeed = (rightBumper - leftBumper) * armMoveSpeed;
            armControllers[(int)armParts.tankSpin_wheel].Speed = slewSpeed;
        }
    }

    void handleSlewGamepad() {

    }

    void SetArmSpeed(float speed)
    {
        var selectedArmController = armControllers[selectedArmIndex];
        if (selectedArmController != null)
        {
            selectedArmController.Speed = speed;
            // Debug.Log(selectedArmController.GetCurrentForce());
        }
    }

    void LogArmForces()
{
    foreach (armParts part in Enum.GetValues(typeof(armParts)))
    {
        GameObject armPart = armGameObjects[(int)part];
        if (armPart != null)
        {
            var armConstraint = armPart.GetComponent<AGXUnity.Constraint>();
            var name = GetDisplayName(part);
            if (armConstraint != null)
            {
                var nativeConstraint = armConstraint.Native as agx.Constraint;
                // Debug.Log(nativeConstraint.getBodyAt(0).getMassProperties().getMass() + " , " + name);
                if (nativeConstraint != null)
                {
                    if (nativeConstraint.getLastForce(nativeConstraint.getBodyAt(0), ref rbf, ref rbt))
                    {
                        Vector3 force = new Vector3((float)rbf.x, (float)rbf.y, (float)rbf.z);
                        Vector3 torque = new Vector3((float)rbt.x, (float)rbt.y, (float)rbt.z);
                        if (armForces.ContainsKey(name)) {
                            armForces[name] = new ForceData(Time.time, name, force, torque);
                        } else {
                            armForces.Add(name, new ForceData(Time.time, name, force, torque));
                        }

                        // Debug.Log($"Force on RigidBody1: ({rbf.x}, {rbf.y}, {rbf.z})");
                        // Debug.Log($"Torque on RigidBody1: ({rbt.x}, {rbt.y}, {rbt.z})");
                    }
                }
            }
        }
    }
}


    void SelectNextArmPart()
    {
        selectedArmIndex = (selectedArmIndex + 1) % armGameObjects.Length;
        UpdateSelectedArmText();
    }

    void SelectPreviousArmPart()
    {
        selectedArmIndex = (selectedArmIndex - 1 + armGameObjects.Length) % armGameObjects.Length;
        UpdateSelectedArmText();
    }

    void gamepadSelectedArm(){
        // selectedArmIndex = -1;
        if (gamepad.xButton.wasPressedThisFrame) 
            selectedArmIndex = (int)armParts.scoop1;
        else if (gamepad.bButton.wasPressedThisFrame)
            selectedArmIndex = (int)armParts.lower_arm;
        else if (gamepad.yButton.wasPressedThisFrame)
            selectedArmIndex = (int)armParts.plow1;
        else if (gamepad.aButton.wasPressedThisFrame)
            selectedArmIndex = (int)armParts.upperToLow;
    }

    void UpdateSelectedArmText()
    {
        GameObject canvasBoard = GameObject.Find("canvasboard");
        if (canvasBoard != null){
            var text = "Selected Joint (Left/Right) : " + GetDisplayName((armParts)selectedArmIndex).ToString() 
                + " \nMode (Tab) : Camera/Excavator";
            TextMeshProUGUI textMesh = canvasBoard.GetComponent<TextMeshProUGUI>();
            textMesh.text = text;
        }
    }

    // Method to move the tracks (called by the ROS subscriber)
    public void MoveExcavatorTracks(float leftTrackSpeed_, float rightTrackSpeed_)
    {
        GameObject parentGameObject = transform.parent.gameObject;
        // Debug.Log("Parent GameObject: " + parentGameObject.name);

        leftTrackSpeed = leftTrackSpeed_;
        rightTrackSpeed = rightTrackSpeed_;
        trackCommandReceived = true; 
        lastCommandTime = Time.time;
    }

    // Method to move the excavator arm (called by the ROS subscriber)
    public void MoveExcavatorArm(float slewSpeed, float boomSpeed, float armSpeed, float bucketSpeed)
    {
        if (armControllers[(int)armParts.full_arm_rotation] != null)
            armControllers[(int)armParts.full_arm_rotation].Speed = slewSpeed;

        if (armControllers[(int)armParts.lower_arm] != null)
            armControllers[(int)armParts.lower_arm].Speed = boomSpeed;

        if (armControllers[(int)armParts.upperToLow] != null)
            armControllers[(int)armParts.upperToLow].Speed = armSpeed;

        if (armControllers[(int)armParts.scoop1] != null)
            armControllers[(int)armParts.scoop1].Speed = bucketSpeed;
    }
}
