using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Camera_Controller : MonoBehaviour
{
    public ExcavatorScript excavatorScript;
    public GameObject gameObject;
    public float sensitivity = 10f;
    private Gamepad gamepad;

    private GameObject cameraObject = null;
    private Vector3 inputDir = Vector3.zero;
    private float yaw = 0f;
    private float pitch = 0f;

    private Vector3 followOffset = new Vector3(0, 0, 0);
    public float followSpeed = 5f; 

    void Start()
    {
        cameraObject = GameObject.Find("Main Camera");
        yaw = cameraObject.transform.eulerAngles.y;
        pitch = cameraObject.transform.eulerAngles.x;
        cameraObject.transform.LookAt(gameObject.transform);
        followOffset = cameraObject.transform.position - gameObject.transform.position;
        if (cameraObject == null)
        {
            Debug.LogError("Main Camera not found!");
        }
        // Cursor.lockState = CursorLockMode.Locked;
        gamepad = Gamepad.current;
    }

    void Update()
    {
        HandleMovementInput();
        HandleRotationInput();
        MoveCamera();
        if(gamepad != null) {
            FollowExcavator();
        }
    }

    void HandleMovementInput()
    {
        inputDir = Vector3.zero;
        // Keyboard Control
        if (Input.GetKey(KeyCode.W)) inputDir.z = 1f;
        if (Input.GetKey(KeyCode.S)) inputDir.z = -1f;
        if (Input.GetKey(KeyCode.A)) inputDir.x = -1f;
        if (Input.GetKey(KeyCode.D)) inputDir.x = 1f;
        if (Input.GetKey(KeyCode.Space)) inputDir.y = 1f;
        if (Input.GetKey(KeyCode.LeftControl)) inputDir.y = -1f;

        // Gamepad control
        if(gamepad != null) {
            Vector2 dpad = gamepad.dpad.ReadValue();
            inputDir.x = dpad.x;
            inputDir.z = dpad.y;
        }
    }

    void HandleRotationInput()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            yaw -= sensitivity * 10 * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            yaw += sensitivity * 10 * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            pitch -= sensitivity * 10 * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            pitch += sensitivity * 10 * Time.deltaTime;
        }

        //Gamepad Control
        if(gamepad != null) {
            Vector2 leftStick = gamepad.rightStick.ReadValue();
            yaw += leftStick.x * 100 * Time.deltaTime;
            pitch -= leftStick.y * 100 * Time.deltaTime;
        }
        
        pitch = Mathf.Clamp(pitch, -90f, 90f);
        // cameraObject.transform.eulerAngles = new Vector3(pitch, yaw, 0f);

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            cameraObject.transform.position += cameraObject.transform.forward * scroll * sensitivity;
        }
    }

    void MoveCamera()
    {
        Vector3 movement = cameraObject.transform.forward * inputDir.z + cameraObject.transform.right * inputDir.x + cameraObject.transform.up * inputDir.y;
        cameraObject.transform.position += movement * sensitivity * Time.deltaTime;
        cameraObject.transform.eulerAngles = new Vector3(pitch, yaw, 0f);
        // followOffset = cameraObject.transform.position - gameObject.transform.position;
    }

    void FollowExcavator()
    {
        if (transform.parent != null)
        {
            Vector3 targetPosition = gameObject.transform.position + followOffset;
            Debug.Log(targetPosition+" , "+cameraObject.transform.position);
            cameraObject.transform.position = Vector3.Lerp(cameraObject.transform.position, targetPosition, followSpeed * Time.deltaTime);
            // cameraObject.transform.LookAt(gameObject.transform.parent);
        }
    }
}
