using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Sensor;
using RosMessageTypes.Std;
using RosMessageTypes.BuiltinInterfaces;
using System.Collections.Generic;
using System;

public class Ground_Truth_Publisher : MonoBehaviour
{
    ROSConnection ros;
    public string topicName = "ground_truth";
    public float publishMessageFrequency = 0.5f;
    private float timeElapsed;
    public GameObject gtHandlerObject;
    private gt_handler gtHandler;
    float cabin_angle;
    float cabin_angular_velocity;
    float boom_angle;
    float boom_angular_velocity;
    float arm_angle;
    float arm_angular_velocity;
    float bucket_angle;
    float bucket_angular_velocity;
    void Start()
    {
        ros = ROSConnection.instance;
        ros.RegisterPublisher<JointStateMsg>(topicName);
        gtHandler = gtHandlerObject.GetComponent<gt_handler>();
        cabin_angle = gtHandler.cabin_angle;
        cabin_angular_velocity = gtHandler.cabin_angular_velocity;
        boom_angle = gtHandler.boom_angle;
        boom_angular_velocity = gtHandler.boom_angular_velocity;
        arm_angle = gtHandler.arm_angle;
        arm_angular_velocity = gtHandler.arm_angular_velocity; 
        bucket_angle = gtHandler.bucket_angle;
        bucket_angular_velocity = gtHandler.bucket_angular_velocity;
    }

    // Update is called once per frame
    void Update()
    {
        cabin_angle = gtHandler.cabin_angle;
        cabin_angular_velocity = gtHandler.cabin_angular_velocity;
        boom_angle = gtHandler.boom_angle;
        boom_angular_velocity = gtHandler.boom_angular_velocity;
        arm_angle = gtHandler.arm_angle;
        arm_angular_velocity = gtHandler.arm_angular_velocity; 
        bucket_angle = gtHandler.bucket_angle;
        bucket_angular_velocity = gtHandler.bucket_angular_velocity;

        JointStateMsg msg = new JointStateMsg
        {
            header = new HeaderMsg
            {
            stamp = new TimeMsg
            {
                sec = (uint)Time.time,
                nanosec = (uint)((Time.time - (int)Time.time) * 1e9)
            },
            },
            name = new string[] { "cabin_joint", "boom_joint", "arm_joint", "bucket_joint" },
            position = new double[] { cabin_angle, boom_angle, arm_angle, bucket_angle},
            velocity = new double[] { cabin_angular_velocity, boom_angular_velocity, arm_angular_velocity, bucket_angular_velocity},
            effort = new double[] { }
        };
        Debug.Log($"Publishing message {msg} on {topicName}");

        ros.Publish(topicName, msg);
        timeElapsed = 0;
        
    }
}
