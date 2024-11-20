using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using OdometryMsg = RosMessageTypes.Nav.OdometryMsg;

public class EKF_Visualizer : MonoBehaviour
{
    public Mesh arrowMesh; // Assign the imported Arrow mesh in the inspector
    private Vector3 initialPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialPosition = transform.position; // Initialize the initial position of the mesh
        ROSConnection.GetOrCreateInstance().Subscribe<OdometryMsg>("/Odometry/filtered", OnOdomestryMsgRecieved);
    }

    void OnOdomestryMsgRecieved(OdometryMsg commandMessage)
    {
        Debug.Log(commandMessage);
        // Calculate the new position based on the initial position and the received message
        Vector3 newPosition = initialPosition + new Vector3(
            (float)commandMessage.pose.pose.position.x,
            (float)commandMessage.pose.pose.position.y,
            (float)commandMessage.pose.pose.position.z
        );

        // Update the position of the mesh
        transform.position = newPosition;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}