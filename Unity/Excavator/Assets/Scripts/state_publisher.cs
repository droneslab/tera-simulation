using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Sensor;
using RosMessageTypes.Std;
using RosMessageTypes.BuiltinInterfaces;
using System.Collections.Generic;
using System;

public class StatePublisher : MonoBehaviour
{
    ROSConnection ros;
    public string topicName = "joint_states_simulator";
    public float publishMessageFrequency = 0.5f;
    private float timeElapsed;
    // Variables to hold child GameObjects
    private GameObject cabin_imu;
    private GameObject boom_imu;
    private GameObject arm_imu;
    private GameObject bucket_imu;
    private Component cabin_imu_sensor;
    private Component boom_imu_sensor;
    private Component arm_imu_sensor;
    private Component bucket_imu_sensor;
    // private List<string> imuDataFields = new List<string> { "position", "velocity", "acceleration", "rotation", "angularVelocity", "RollPitchYaw" };
    private List<string> imuDataFields = new List<string> { "angularVelocity", "RollPitchYaw" };
    Transform FindChildRecursive(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            // Debug.Log(child.name);
            if (child.name == childName)
            {
                return child;
            }
            Transform result = FindChildRecursive(child, childName);
            if (result != null)
            {
                // Debug.Log($"Found {childName} in {parent.name}");
                return result;
            }
        }
        return null;
    }

    List<Vector3> GrabIMUProperties(Component imuComponent)
    {
        List<Vector3> imuPropertiesList = new List<Vector3>();
        
        foreach (string field in imuDataFields)
        {
            var property = imuComponent.GetType().GetProperty(field);
            if (property != null)
            {
                Vector3 value = (Vector3)property.GetValue(imuComponent, null);
                // Debug.Log($"Value: {value} Type: {value.GetType().Name}");                
                imuPropertiesList.Add(value);
            }
            else
            {
                Debug.LogWarning($"Property {field} not found on {imuComponent.GetType().Name}");
            }
        }
        
        return imuPropertiesList;
    }
    // Start is called before the first frame update
    void Start()
    {
        // Get the ROSConnection from the ROSConnector
        ros = ROSConnection.instance;
        ros.RegisterPublisher<JointStateMsg>(topicName);
        
        // Find the child GameObjects by name
        cabin_imu = FindChildRecursive(this.gameObject.transform, "Cabin_IMU").gameObject;
        boom_imu = FindChildRecursive(this.gameObject.transform, "Boom_IMU").gameObject;
        arm_imu = FindChildRecursive(this.gameObject.transform, "Arm_IMU").gameObject;
        bucket_imu = FindChildRecursive(this.gameObject.transform, "Bucket_IMU").gameObject;
        
        // Get the IMUSensor 
        cabin_imu_sensor = cabin_imu.GetComponent("IMUSensor");
        boom_imu_sensor = boom_imu.GetComponent("IMUSensor");
        arm_imu_sensor = arm_imu.GetComponent("IMUSensor");
        bucket_imu_sensor = bucket_imu.GetComponent("IMUSensor");   
    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;

        if (timeElapsed > publishMessageFrequency)
        {
            List<Vector3> cabin_data = GrabIMUProperties(cabin_imu_sensor);
            List<Vector3> boom_data = GrabIMUProperties(boom_imu_sensor);
            List<Vector3> arm_data = GrabIMUProperties(arm_imu_sensor);
            List<Vector3> bucket_data = GrabIMUProperties(bucket_imu_sensor);
            // Debug.Log($"Cabin data: {cabin_data}");
            // Debug.Log($"{cabin_data[0].x}");
            JointStateMsg msg = new JointStateMsg
            {
                // header = new HeaderMsg { stamp = new TimeMsg { secs = (uint)Time.time, nsecs = (uint)((Time.time - (int)Time.time) * 1e9) } },
                // header = new Std.Header { stamp = 0.0f },
                name = new string[] { "cabin_joint", "boom_joint", "arm_joint", "bucket_joint" },
                position = new double[] { cabin_data[1].x, boom_data[1].x, arm_data[1].x, bucket_data[1].x},
                velocity = new double[] { cabin_data[0].z, boom_data[0].z, arm_data[0].z, bucket_data[0].z},
                effort = new double[] { }
            };
            Debug.Log($"Publishing message {msg}");

            ros.Publish(topicName, msg);
            timeElapsed = 0;
        }
    }
}
