using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using YamlDotNet.Serialization;

public class Offset
{
    public double x { get; set; }
    public double y { get; set; }
    public double z { get; set; }
}

public class Rotation
{
    public double x { get; set; }
    public double y { get; set; }
    public double z { get; set; }
}

public class Noise
{
    public double mean { get; set; }
    public double std_dev { get; set; }
}

public class Sensor
{
    public string id { get; set; }
    public string type { get; set; }
    public string topic { get; set; }
    public string location { get; set; }
    public Offset offset { get; set; }
    public Rotation rotation { get; set; }
    public Noise noise { get; set; }
    
}

public class Excavator
{
    public string id { get; set; }
    public string type { get; set; }
    public Offset offset { get; set; }
    public Rotation rotation { get; set; }
    public List<Sensor> sensors { get; set; }
}

public class ExcavatorList
{
    public List<Excavator> Excavator { get; set; }
}

public class Excavator_Creator : MonoBehaviour
{
    public GameObject excavatorPrefab;
    public GameObject IMUPrefab;
    public GameObject GPSPrefab;
    public GameObject RGBCameraPrefab;
    public GameObject RGBDCameraPrefab;
    public GameObject LidarPrefab;
    public void ListAllComponents(GameObject sensorObject)
    {
        Component[] components = sensorObject.GetComponents<Component>();
        foreach (Component component in components)
        {
            Debug.Log("Component: " + component.GetType().Name);
        }
    }
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
    Component PrintHierarchy(Transform parent, string childName, string indent = "")
    {
        Debug.Log(indent + parent.name);
        if (parent.name == childName)
        {
            return parent.GetComponent<Component>();
        }
        foreach (Transform child in parent)
        {
            Component result = PrintHierarchy(child, childName, indent + "  ");
            if (result != null)
            {
                return result;
            }
        }
        return null;
    }

    void Start()
    {
        var deserializer = new DeserializerBuilder().Build();
        var yamlInput = File.ReadAllText("/home/moog/Moog/moog-simulation/moog-simulation/excavator_config.yaml");
        var excavatorList = deserializer.Deserialize<ExcavatorList>(yamlInput);
        
        foreach (var excavator in excavatorList.Excavator)
        {
            Debug.Log($"Excavator ID: {excavator.id}, Type: {excavator.type}");
            GameObject excavatorObject = Instantiate(excavatorPrefab);
            excavatorObject.name = excavator.id;
            excavatorObject.transform.position = new Vector3((float)excavator.offset.x, (float)excavator.offset.y, (float)excavator.offset.z);
            excavatorObject.transform.rotation = Quaternion.Euler((float)excavator.rotation.x, (float)excavator.rotation.y, (float)excavator.rotation.z);
            foreach (var sensor in excavator.sensors)
            {
                // Debug.Log($"Sensor ID: {sensor.id}, Type: {sensor.type}, Location: {sensor.location}, Offset: x={sensor.offset.x}, y={sensor.offset.y}, z={sensor.offset.z}, Rotation: x={sensor.rotation.x}, y={sensor.rotation.y}, z={sensor.rotation.z}");
                GameObject sensorObject = null;
                Component componentTransform = null;
                switch (sensor.type)
                {
                    case "IMU":
                        sensorObject = Instantiate(IMUPrefab);
                        componentTransform = sensorObject.GetComponent("IMUMsgPublisher");
                        // sensorObject.SetNoise(sensor.noise.mean, sensor.noise.std_dev);
                        break;
                    case "GPS":
                        sensorObject = Instantiate(GPSPrefab);
                        componentTransform = sensorObject.GetComponent("NavSatFixMsgPublisher");
                        break;
                    case "RGB_CAMERA":
                        sensorObject = Instantiate(RGBCameraPrefab);
                        componentTransform = sensorObject.GetComponent("CameraImageMsgPublisher");
                        break;
                    case "RGBD_CAMERA":
                        sensorObject = Instantiate(RGBDCameraPrefab);
                        componentTransform = sensorObject.GetComponent("ImageMsgPublisher");
                        break;
                    case "LIDAR":
                        sensorObject = Instantiate(LidarPrefab);
                        Transform sensorChild = sensorObject.transform.Find("Sensor");
                        componentTransform = sensorChild.GetComponent("RaycastLiDARPointCloud2MsgPublisher");
                        break;
                }
                if (sensorObject != null)
                {
                    sensorObject.name = sensor.id;
                    // PrintHierarchy(sensorObject.transform);
                    switch (sensor.location)
                    {
                        case "CHASIS":
                            sensorObject.transform.SetParent(FindChildRecursive(excavatorObject.transform, "compact_excavator_cabin_body_cmpl_VisualMesh"));
                            // sensorObject.transform.SetParent(excavatorObject.transform.Find("compact_excavator_cabin_body_cmpl"));
                            break;
                        case "BOOM":
                            sensorObject.transform.SetParent(FindChildRecursive(excavatorObject.transform, "part02_cmpl_VisualMesh"));                                           
                            break;
                        case "ARM":
                            sensorObject.transform.SetParent(FindChildRecursive(excavatorObject.transform, "part03_VisualMesh"));
                            break;
                        case "BUCKET":
                            sensorObject.transform.SetParent(FindChildRecursive(excavatorObject.transform, "part04"));
                            break;
                    }
                    sensorObject.transform.localPosition = new Vector3((float)sensor.offset.x, (float)sensor.offset.y, (float)sensor.offset.z);
                    sensorObject.transform.localRotation = Quaternion.Euler((float)sensor.rotation.x, (float)sensor.rotation.y, (float)sensor.rotation.z);
                }
                if (componentTransform != null)
                {
                    // Debug.Log($"Component Name: {componentTransform.GetType().Name}");
                    FieldInfo topicField = componentTransform.GetType().GetField("_topicName", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                    if (topicField != null)
                    {
                        topicField.SetValue(componentTransform, "/" + excavator.id + sensor.topic);
                    }
                    else
                    {
                        Debug.LogWarning($"Field '_topicName' not found in component of type {componentTransform.GetType().Name}");
                    }
                }
            }
        }
    }
}
