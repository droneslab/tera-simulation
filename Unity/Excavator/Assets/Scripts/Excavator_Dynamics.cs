    using UnityEngine;
    using AGXUnity;
    using AGXUnity.Utils;
    using System.Collections.Generic;
    using Unity.Robotics.ROSTCPConnector;
    using RosMessageTypes.Std;
    using RosMessageTypes.Geometry;
    using RosMessageTypes.BuiltinInterfaces;
using agxCollide;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;

public class Excavator_Dynamics : ScriptComponent {
        public AGXUnity.Model.DeformableTerrainShovel shovel;
        public AGXUnity.RigidBody box;
        private float m_massInBucket = 0;
        private float m_massOnBox = 0;
        private float m_massOfBucket = 0;
        private ExcavatorScript excavatorScript;
        private const float gravityAcceleration = 9.81f;

        private List<float> recentMassValues = new List<float>();
        private const int averageWindowSize = 10;
        private ROSConnection ros;

        // test objects can be deleted
        public AGXUnity.RigidBody testObject;
        public AGXUnity.RigidBody leftSprocket;
        public AGXUnity.RigidBody rightSprocket;
        private agxControl.EventSensor sensor;
        private agxCollide.Geometry shovelGeometry;
        private TwistMsg twistMessage;
        private Float32Msg massMessage;

    // protected override bool Initialize()
    // {
    //     // Initialize the shovel geometry as a sensor
    //     shovelGeometry = shovel.GetComponentsInChildren<AGXUnity.Collide.Mesh>()[1].GetInitialized<AGXUnity.Collide.Shape>().NativeGeometry;
    //     Debug.Log(shovelGeometry);
    //     // shovelGeometry.setSensor(true);

    //     // Initialize the event sensor for contact detection
    //     sensor = new agxControl.EventSensor(shovelGeometry);
    //     GetSimulation().add(sensor);

    //     return base.Initialize();
    // }

    void Start()
    {
        excavatorScript = GetComponent<ExcavatorScript>();
        m_massOfBucket = shovel.GetComponent<AGXUnity.RigidBody>().MassProperties.Mass.Value;
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<Float32Msg>("Excavator/mass_in_bucket");
        ros.RegisterPublisher<TwistMsg>("Excavator/left_sprocket_velocity");
        ros.RegisterPublisher<TwistMsg>("Excavator/right_sprocket_velocity");
        twistMessage = new TwistMsg();
        massMessage = new Float32Msg();
    }

    void FixedUpdate()
    {
        // Calculating terrain mass on the shovel
        // GameObject terrainObject = GameObject.Find("Terrain");
        // AGXUnity.Model.DeformableTerrain deformableTerrain = terrainObject.GetComponent<AGXUnity.Model.DeformableTerrain>();
        // m_massInBucket = (float)deformableTerrain.Native.getDynamicMass( shovel.Native );
        // Debug.Log(m_massInBucket);


        // Calculation terrain mass from the force on shovel
        var armForces = excavatorScript.GetArmForces();
        float forceOnBucket = armForces["Bucket"].Force;
        m_massInBucket = (forceOnBucket / gravityAcceleration) - m_massOfBucket;

        recentMassValues.Add(m_massInBucket);
        if (recentMassValues.Count > averageWindowSize) recentMassValues.RemoveAt(0);
        if (recentMassValues.Count == averageWindowSize)
        {
            float averageBucketMass = 0;
            foreach (float mass in recentMassValues) averageBucketMass += mass;
            averageBucketMass /= recentMassValues.Count;
            Float32Msg massMsg = new Float32Msg(averageBucketMass);
            ros.Publish("Excavator/mass_in_bucket", massMsg);
            // Debug.Log("Average mass in bucket: " + averageBucketMass);
        }

        // Calculating angular velocity of the sprockets

        twistMessage.linear = new Vector3Msg(leftSprocket.LinearVelocity.x, leftSprocket.LinearVelocity.y, leftSprocket.LinearVelocity.z);
        twistMessage.angular = new Vector3Msg(leftSprocket.AngularVelocity.x, leftSprocket.AngularVelocity.y, leftSprocket.AngularVelocity.z);
        ros.Publish("Excavator/left_sprocket_velocity", twistMessage);

        twistMessage.linear = new Vector3Msg(rightSprocket.LinearVelocity.x, rightSprocket.LinearVelocity.y, rightSprocket.LinearVelocity.z);
        twistMessage.angular = new Vector3Msg(rightSprocket.AngularVelocity.x, rightSprocket.AngularVelocity.y, rightSprocket.AngularVelocity.z);
        ros.Publish("Excavator/right_sprocket_velocity", twistMessage);
    }
}
