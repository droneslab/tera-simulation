using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Controller : MonoBehaviour
{
    public ExcavatorScript excavatorScript;
    public float sensitivity = 10f;

    private GameObject cameraObject = null;
    private Vector3 inputDir = Vector3.zero;
    private float yaw = 0f;
    private float pitch = 0f;

    void Start()
    {
        cameraObject = GameObject.Find("Main Camera");
        if (cameraObject == null)
        {
            Debug.LogError("Main Camera not found!");
        }
        // Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        HandleMovementInput();
        HandleRotationInput();
        MoveCamera();
    }

    void HandleMovementInput()
    {
        inputDir = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) inputDir.z = 1f;
        if (Input.GetKey(KeyCode.S)) inputDir.z = -1f;
        if (Input.GetKey(KeyCode.A)) inputDir.x = -1f;
        if (Input.GetKey(KeyCode.D)) inputDir.x = 1f;
        if (Input.GetKey(KeyCode.Space)) inputDir.y = 1f;
        if (Input.GetKey(KeyCode.LeftControl)) inputDir.y = -1f;
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
    }
}
