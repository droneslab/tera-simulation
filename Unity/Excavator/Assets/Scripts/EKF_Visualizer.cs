using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using OdometryMsg = RosMessageTypes.Nav.OdometryMsg;

public class EKF_Visualizer : MonoBehaviour
{
    private GameObject redCube;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ROSConnection.GetOrCreateInstance().Subscribe<OdometryMsg>("joy_deltacan", OnOdomestryMsgRecieved);
        // Create a red cube
        GameObject redCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        redCube.transform.localScale = new Vector3(1, 1, 1);
        redCube.GetComponent<Renderer>().material.color = Color.red;
    }

    void OnOdomestryMsgRecieved(OdometryMsg commandMessage)
    {
        Debug.Log(commandMessage);
        // Update the position of the red cube
        Vector3 newPosition = new(
            (float)commandMessage.pose.pose.position.x,
            (float)commandMessage.pose.pose.position.y,
            (float)commandMessage.pose.pose.position.z
        );
        redCube.transform.position = newPosition;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
