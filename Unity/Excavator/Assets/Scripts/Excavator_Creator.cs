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

    FieldInfo FindFieldInHierarchy(Type type, string fieldName)
    {
        while (type != null)
        {
            FieldInfo field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (field != null)
            {
                return field;
            }
            type = type.BaseType;
        }

        return null;
    }

    string NormalizeTopic(string topic)
    {
        if (string.IsNullOrWhiteSpace(topic))
        {
            return "/";
        }

        return "/" + topic.Trim('/');
    }

    string BuildNamespacedTopic(string excavatorId, string sensorTopic)
    {
        string topic = string.IsNullOrWhiteSpace(sensorTopic) ? string.Empty : sensorTopic.Trim('/');
        return NormalizeTopic(excavatorId + "/" + topic);
    }

    string BuildNamespacedCameraTopic(string excavatorId, string sensorTopic, string originalTopic)
    {
        string namespacedBase = BuildNamespacedTopic(excavatorId, sensorTopic);
        string normalizedOriginal = NormalizeTopic(originalTopic);
        const string cameraRoot = "/camera";

        if (normalizedOriginal.StartsWith(cameraRoot + "/"))
        {
            return NormalizeCameraInfoTopic(namespacedBase + normalizedOriginal.Substring(cameraRoot.Length));
        }

        return NormalizeCameraInfoTopic(NormalizeTopic(excavatorId + "/" + normalizedOriginal.Trim('/')));
    }

    string NormalizeCameraInfoTopic(string topicName)
    {
        if (topicName.EndsWith("/info"))
        {
            return topicName.Substring(0, topicName.Length - "/info".Length) + "/camera_info";
        }

        return topicName;
    }

    string BuildSensorFrameId(string excavatorId, Sensor sensor)
    {
        return excavatorId + "/" + sensor.id + "_frame";
    }

    void SetSerializerHeaderFrameId(Component publisherComponent, string frameId)
    {
        FieldInfo serializerField = FindFieldInHierarchy(publisherComponent.GetType(), "_serializer");
        if (serializerField == null)
        {
            return;
        }

        object serializer = serializerField.GetValue(publisherComponent);
        if (serializer == null)
        {
            return;
        }

        FieldInfo headerField = FindFieldInHierarchy(serializer.GetType(), "_header");
        if (headerField == null)
        {
            return;
        }

        object header = headerField.GetValue(serializer);
        if (header == null)
        {
            return;
        }

        FieldInfo frameIdField = FindFieldInHierarchy(header.GetType(), "_frame_id");
        if (frameIdField != null && frameIdField.FieldType == typeof(string))
        {
            frameIdField.SetValue(header, frameId);
        }
    }

    void SetSensorPublisherTopics(GameObject sensorObject, string excavatorId, Sensor sensor)
    {
        List<Tuple<Component, FieldInfo, string>> topicPublishers = new List<Tuple<Component, FieldInfo, string>>();
        Component[] components = sensorObject.GetComponentsInChildren<Component>(true);

        foreach (Component component in components)
        {
            if (component == null)
            {
                continue;
            }

            FieldInfo topicField = FindFieldInHierarchy(component.GetType(), "_topicName");
            if (topicField == null || topicField.FieldType != typeof(string))
            {
                continue;
            }

            string originalTopic = topicField.GetValue(component) as string;
            topicPublishers.Add(Tuple.Create(component, topicField, originalTopic));
        }

        if (topicPublishers.Count == 0)
        {
            Debug.LogWarning($"No '_topicName' publisher fields found on sensor '{sensor.id}' of type '{sensor.type}'");
            return;
        }

        bool multiTopicSensor = topicPublishers.Count > 1;
        string frameId = BuildSensorFrameId(excavatorId, sensor);
        foreach (Tuple<Component, FieldInfo, string> publisher in topicPublishers)
        {
            string topicName = multiTopicSensor
                ? BuildNamespacedCameraTopic(excavatorId, sensor.topic, publisher.Item3)
                : BuildNamespacedTopic(excavatorId, sensor.topic);

            publisher.Item2.SetValue(publisher.Item1, topicName);
            SetSerializerHeaderFrameId(publisher.Item1, frameId);
            Debug.Log($"Sensor '{sensor.id}' publishing '{publisher.Item3}' as '{topicName}'");
        }
    }

    void AddSensorDebugGizmo(GameObject sensorObject, string excavatorId, Sensor sensor)
    {
        SensorDebugGizmo gizmo = sensorObject.GetComponent<SensorDebugGizmo>();
        if (gizmo == null)
        {
            gizmo = sensorObject.AddComponent<SensorDebugGizmo>();
        }

        gizmo.label = excavatorId + "/" + sensor.id;
        gizmo.color = sensor.type == "RGB_CAMERA" || sensor.type == "RGBD_CAMERA" ? Color.yellow : Color.cyan;
    }

    void ConfigureDirectCameraImagePublisher(GameObject sensorObject, string excavatorId, Sensor sensor)
    {
        if (sensor.type != "RGB_CAMERA")
        {
            return;
        }

        string topicName = BuildNamespacedCameraTopic(excavatorId, sensor.topic, "/camera/color/image/compressed");
        CameraCompressedImagePublisher imagePublisher = sensorObject.GetComponent<CameraCompressedImagePublisher>();
        if (imagePublisher == null)
        {
            imagePublisher = sensorObject.AddComponent<CameraCompressedImagePublisher>();
        }

        imagePublisher.topicName = topicName;
        imagePublisher.frameId = BuildSensorFrameId(excavatorId, sensor);

        Component[] components = sensorObject.GetComponents<Component>();
        foreach (Component component in components)
        {
            if (component != null && component.GetType().Name == "CameraImageMsgPublisher")
            {
                MonoBehaviour monoBehaviour = component as MonoBehaviour;
                if (monoBehaviour != null)
                {
                    monoBehaviour.enabled = false;
                    Debug.Log($"Disabled generic CameraImageMsgPublisher on '{sensorObject.name}'");
                }
            }
        }
    }

    void Start()
    {
        var deserializer = new DeserializerBuilder().Build();
        string currentDirectory = Directory.GetCurrentDirectory();
        string yamlPath = Path.Combine(currentDirectory,"..", "..", "excavator_config.yaml");
        var yamlInput = File.ReadAllText(yamlPath);
        var excavatorList = deserializer.Deserialize<ExcavatorList>(yamlInput);
        
        foreach (var excavator in excavatorList.Excavator)
        {
            Debug.Log($"Excavator ID: {excavator.id}, Type: {excavator.type}");
            GameObject excavatorObject = Instantiate(excavatorPrefab);
            excavatorObject.name = excavator.id;
            excavatorObject.transform.position = new Vector3((float)excavator.offset.x, (float)excavator.offset.y, (float)excavator.offset.z);
            excavatorObject.transform.rotation = Quaternion.Euler((float)excavator.rotation.x, (float)excavator.rotation.y, (float)excavator.rotation.z);
            if (excavator.sensors == null)
            {
                excavator.sensors = new List<Sensor>();
            }
            foreach (var sensor in excavator.sensors)
            {
                // Debug.Log($"Sensor ID: {sensor.id}, Type: {sensor.type}, Location: {sensor.location}, Offset: x={sensor.offset.x}, y={sensor.offset.y}, z={sensor.offset.z}, Rotation: x={sensor.rotation.x}, y={sensor.rotation.y}, z={sensor.rotation.z}");
                GameObject sensorObject = null;
                switch (sensor.type)
                {
                    case "IMU":
                        sensorObject = Instantiate(IMUPrefab);
                        // sensorObject.SetNoise(sensor.noise.mean, sensor.noise.std_dev);
                        break;
                    case "GPS":
                        sensorObject = Instantiate(GPSPrefab);
                        break;
                    case "RGB_CAMERA":
                        sensorObject = Instantiate(RGBCameraPrefab);
                        break;
                    case "RGBD_CAMERA":
                        sensorObject = Instantiate(RGBDCameraPrefab);
                        break;
                    case "LIDAR":
                        sensorObject = Instantiate(LidarPrefab);
                        break;
                }
                if (sensorObject != null)
                {
                    sensorObject.name = excavator.id + "_" + sensor.id;
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
                    AddSensorDebugGizmo(sensorObject, excavator.id, sensor);
                    SetSensorPublisherTopics(sensorObject, excavator.id, sensor);
                    ConfigureDirectCameraImagePublisher(sensorObject, excavator.id, sensor);
                }
            }
        }
    }
}
