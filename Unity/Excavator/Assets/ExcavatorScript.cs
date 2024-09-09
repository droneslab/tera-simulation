using UnityEngine;
using UnityEngine.UI;
using AGXUnity;
using TMPro;

public class ExcavatorScript : MonoBehaviour
{
    public enum trackParts { left_sprocket_wheel, right_sprocket_wheel }
    public enum armParts {
        full_arm_rotation, lower_arm, upperToLow, scoop1
    }

    // Track Variables
    private float forwardSpeed = 3f;
    private float turnSpeed = 2f;
    private TargetSpeedController leftController;
    private TargetSpeedController rightController;

    // Arm Variables
    private int selectedArmIndex = 0;
    private GameObject[] armGameObjects;
    private TargetSpeedController[] armControllers;
    public Text selectedArmText;
    private float armMoveSpeed = 0.4f;

    void Start()
    {
        // Initialize track parts
        foreach (trackParts part in System.Enum.GetValues(typeof(trackParts)))
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
        armGameObjects = new GameObject[System.Enum.GetValues(typeof(armParts)).Length];
        armControllers = new TargetSpeedController[armGameObjects.Length];

        foreach (armParts part in System.Enum.GetValues(typeof(armParts)))
        {
            GameObject armPart = GameObject.Find(part.ToString());
            armGameObjects[(int)part] = armPart;

            if (armPart != null)
            {
                var armConstraint = armPart.GetComponent<AGXUnity.Constraint>();
                if (armConstraint != null)
                {
                    armControllers[(int)part] = armConstraint.GetController<TargetSpeedController>();
                    // var controller = armConstraint.GetController<TargetSpeedController>();
                    // controller.Speed = -2;
                }
            }
        }

        UpdateSelectedArmText();
    }

    void Update()
    {
        // Handle WASD movement for tracks
        float forwardInput = Input.GetKey(KeyCode.W) ? forwardSpeed : (Input.GetKey(KeyCode.S) ? -forwardSpeed : 0);
        float turnInput = Input.GetKey(KeyCode.A) ? -turnSpeed : (Input.GetKey(KeyCode.D) ? turnSpeed : 0);

        if (leftController != null && rightController != null)
        {
            leftController.Speed = forwardInput + turnInput;
            rightController.Speed = forwardInput - turnInput;
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

    void UpdateSelectedArmText()
    {
        var text = "Selected Arm Part: " + ((armParts)selectedArmIndex).ToString();
        GameObject canvasBoard = GameObject.Find("canvasboard");
        TextMeshProUGUI textMesh = canvasBoard.GetComponent<TextMeshProUGUI>();
        textMesh.text = text;
        // Debug.Log(((armParts)selectedArmIndex));
    }
}
