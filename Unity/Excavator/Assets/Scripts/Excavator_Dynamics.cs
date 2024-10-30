    using UnityEngine;
    using AGXUnity;
    using AGXUnity.Utils;
    using System.Collections.Generic;
    using Unity.Robotics.ROSTCPConnector;
    using RosMessageTypes.Std;
    using RosMessageTypes.BuiltinInterfaces;

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
        private agxControl.EventSensor sensor;
        private agxCollide.Geometry shovelGeometry;

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

        void Start() {
            excavatorScript = GetComponent<ExcavatorScript>();
            m_massOfBucket = shovel.GetComponent<AGXUnity.RigidBody>().MassProperties.Mass.Value;
            ros = ROSConnection.GetOrCreateInstance();
            ros.RegisterPublisher<Float32Msg>("Excavator/mass_in_bucket");
            Debug.Log("Dynamics..........");
        }

        void Update() {
            // Calculating terrain mass on the shovel
            // GameObject terrainObject = GameObject.Find("Terrain");
            // AGXUnity.Model.DeformableTerrain deformableTerrain = terrainObject.GetComponent<AGXUnity.Model.DeformableTerrain>();
            // m_massInBucket = (float)deformableTerrain.Native.getDynamicMass( shovel.Native );
            // Debug.Log(m_massInBucket);


            // Calculation terrain mass from the force on shovel
            var armForces =  excavatorScript.GetArmForces();
            float forceOnBucket = armForces["Bucket"].Force;
            m_massInBucket = (forceOnBucket / gravityAcceleration) - m_massOfBucket;

            recentMassValues.Add(m_massInBucket);
            if(recentMassValues.Count > averageWindowSize) recentMassValues.RemoveAt(0);
            if (recentMassValues.Count == averageWindowSize)
            {
                float averageBucketMass = 0;
                foreach (float mass in recentMassValues) averageBucketMass += mass;
                averageBucketMass /= recentMassValues.Count;
                Debug.Log("Mass : "+averageBucketMass);

                Float32Msg massMsg = new Float32Msg(averageBucketMass);
                ros.Publish("Excavator/mass_in_bucket", massMsg);
            }

            // //Calculating some object on the shovel
            // var ids = sensor.getContactingParticleIds();
            
            // var ps = GetSimulation().getParticleSystem();
            // foreach ( var id in ids ) {
            //     Debug.Log(id);
            //     var p = ps.getParticle(id);
            //     Debug.Log((float)p.getMass());
            
            //     // Kill the particle
            //     ps.destroyParticle( p );
            // }
        }
    }
