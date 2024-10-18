using UnityEngine;
using UnityEngine.UI;
using AGXUnity;
using AGXUnity.Utils;
using TMPro;
using System.Collections.Generic;
using System;

public class ExcavatorScript : MonoBehaviour
{
    public enum trackParts { left_sprocket_wheel, right_sprocket_wheel }
    // public enum armParts { scoop1 }
    public enum armParts { full_arm_rotation, lower_arm, upperToLow, scoop1 }

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
    private List<ForceData> armForces = new List<ForceData>();

    public struct ForceData
    {
        public float Time;
        public Vector3 RigidBody1Force;
        public Vector3 RigidBody2Force;
        public Vector3 RigidBody1Torque;
        public Vector3 RigidBody2Torque;
    }

    void Start()
    {
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
    }

    void Update()
    {
        if (!trackCommandReceived)
        {
            // Handle WASD movement for tracks
            float forwardInput = Input.GetKey(KeyCode.W) ? 1.0f : (Input.GetKey(KeyCode.S) ? -1.0f : 0);
            float turnInput = Input.GetKey(KeyCode.A) ? -1.0f : (Input.GetKey(KeyCode.D) ? 1.0f : 0);

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

        // Log the forces and torques applied to the arm parts
        // LogArmForces();
    }

    void HandleArmMovement()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            SetArmSpeed(armMoveSpeed);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            SetArmSpeed(-armMoveSpeed);
        }
        else
        {
            SetArmSpeed(0);
        }
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
    foreach (var armPart in armGameObjects)
    {
        if (armPart != null)
        {
            var armConstraint = armPart.GetComponent<AGXUnity.Constraint>();
            if (armConstraint != null)
            {
                var nativeConstraint = armConstraint.Native as agx.Constraint;
                Debug.Log(nativeConstraint.getBodyAt(0).getMassProperties().getMass()+" , "+armPart);
                if (nativeConstraint != null)
                {
                    agx.Vec3 rbf = new agx.Vec3(), rbt = new agx.Vec3();
                    if (nativeConstraint.getLastForce(nativeConstraint.getBodyAt(0), ref rbf, ref rbt))
                    {
                        Debug.Log($"Force on RigidBody1: ({rbf.x}, {rbf.y}, {rbf.z})");
                        Debug.Log($"Torque on RigidBody1: ({rbt.x}, {rbt.y}, {rbt.z})");
                    }
                }
            }
        }
    }
}


    void SelectNextArmPart()
    {
        selectedArmIndex = (selectedArmIndex + 1) % armGameObjects.Length;
        // UpdateSelectedArmText();
    }

    void SelectPreviousArmPart()
    {
        selectedArmIndex = (selectedArmIndex - 1 + armGameObjects.Length) % armGameObjects.Length;
        // UpdateSelectedArmText();
    }

    void UpdateSelectedArmText()
    {
        var text = "Selected Arm Part: " + ((armParts)selectedArmIndex).ToString();
        GameObject canvasBoard = GameObject.Find("canvasboard");
        TextMeshProUGUI textMesh = canvasBoard.GetComponent<TextMeshProUGUI>();
        textMesh.text = text;
    }

    // Method to move the tracks (called by the ROS subscriber)
    public void MoveExcavatorTracks(float leftTrackSpeed_, float rightTrackSpeed_)
    {
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
