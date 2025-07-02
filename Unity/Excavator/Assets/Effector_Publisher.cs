using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;
using RosMessageTypes.Std;
using RosMessageTypes.BuiltinInterfaces;
using System.Collections.Generic;
using System;

public class Effector_Publisher : MonoBehaviour
{
    // Start is called before the first frame update
    ROSConnection ros;
    public GameObject excavator;
    public string excavatorName;
    public string topicName = "effector_pose";
    public float publishMessageFrequency = 0.5f;
    private float timeElapsed;
    public GameObject efHandlerObject;
    private Effector_Pose efHandler;
    Vector3 position,rotation;
    void Start()
    {
        excavator = this.gameObject;
        excavatorName = excavator.name;
        topicName = excavatorName + "/" + topicName;
        ros = ROSConnection.instance;
        Debug.Log("Registering publisher for topic: " + topicName);
        ros.RegisterPublisher<TransformMsg>(topicName);
        efHandler = efHandlerObject.GetComponent<Effector_Pose>();
    }

    // Update is called once per frame
    void Update()
    {
        position = efHandler.relative_position_E;
        rotation = efHandler.relative_rotation_E;

        Quaternion q = Quaternion.Euler(rotation);


        TransformMsg msg = new TransformMsg
        {
            translation = new Vector3Msg
            {
                x = position.x,
                y = position.y,
                z = position.z
            },
            rotation = new QuaternionMsg
            {
                x = q.x,
                y = q.y,
                z = q.z,
                w = q.w
            }
        };
        ros.Publish(topicName, msg);
    }
}
