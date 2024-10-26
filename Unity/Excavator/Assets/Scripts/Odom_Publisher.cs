using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Sensor;
using RosMessageTypes.Std;
using RosMessageTypes.Geometry;
using RosMessageTypes.Nav;
using RosMessageTypes.BuiltinInterfaces;
using System.Collections.Generic;
using System;

public class Odom_Publisher : MonoBehaviour
{
    [SerializeField]
    private string topicName = "odom";
    public GameObject excavator;
    public string excavatorName;
    public GameObject base_link;
    ROSConnection ros;
    Transform base_transform;

    // Start is called before the first frame update
    void Start()
    {
        excavator = this.gameObject;
        excavatorName = excavator.name;
        topicName = excavatorName + "/" + topicName;
        ros = ROSConnection.instance;
        ros.RegisterPublisher<OdometryMsg>(topicName);
        base_transform = base_link.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        // Get the position and orientation of the excavator part
        Vector3 position = base_transform.position;
        Quaternion orientation = base_transform.rotation;

        // Create the odom message
        OdometryMsg odomMsg = new OdometryMsg
        {
            header = new HeaderMsg
            {
            stamp = new TimeMsg
            {
                sec = (int)Time.time,
                nanosec = (uint)((Time.time - (int)Time.time) * 1e9)
            },
            frame_id = "odom"
            },
            child_frame_id = "base_link",
            pose = new PoseWithCovarianceMsg
            {
            pose = new PoseMsg
            {
                position = new PointMsg
                {
                x = position.x,
                y = position.y,
                z = position.z
                },
                orientation = new QuaternionMsg
                {
                x = orientation.x,
                y = orientation.y,
                z = orientation.z,
                w = orientation.w
                }
            }
            },
            twist = new TwistWithCovarianceMsg
            {
            twist = new TwistMsg
            {
                linear = new Vector3Msg
                {
                x = 0,
                y = 0,
                z = 0
                },
                angular = new Vector3Msg
                {
                x = 0,
                y = 0,
                z = 0
                }
            }
            }
        };

        // Publish the odom message
        ros.Publish(topicName, odomMsg);
        // Debug.Log($"Publishing {topicName} message: {odomMsg}");
        
    }
}
