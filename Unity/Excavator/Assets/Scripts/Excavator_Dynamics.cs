    using UnityEngine;
    using AGXUnity;
    using AGXUnity.Utils;

    public class Excavator_Dynamics : ScriptComponent {
        public AGXUnity.Model.DeformableTerrainShovel shovel;
        public AGXUnity.RigidBody box;
        private float m_massInBucket = 0;
        private float m_massOnBox = 0;

        // test objects can be deleted
        public AGXUnity.RigidBody testObject;
        private agxControl.EventSensor sensor;
        private agxCollide.Geometry shovelGeometry;    

        protected override bool Initialize()
        {
            // Initialize the shovel geometry as a sensor
            shovelGeometry = shovel.GetComponentsInChildren<AGXUnity.Collide.Mesh>()[1].GetInitialized<AGXUnity.Collide.Shape>().NativeGeometry;
            Debug.Log(shovelGeometry);
            // shovelGeometry.setSensor(true);

            // Initialize the event sensor for contact detection
            sensor = new agxControl.EventSensor(shovelGeometry);
            GetSimulation().add(sensor);

            return base.Initialize();
        }

        void Start() {

            Debug.Log("Dynamics..........");
            Initialize();
        }

        void Update() {
            // Calculating terrain mass on the shovel
            GameObject terrainObject = GameObject.Find("Terrain");
            AGXUnity.Model.DeformableTerrain deformableTerrain = terrainObject.GetComponent<AGXUnity.Model.DeformableTerrain>();
            // Debug.Log();
            // AGXUnity.Model.DeformableTerrainShovel shovel = deformableTerrain.GetComponent<AGXUnity.Model.DeformableTerrainShovel>();
            m_massInBucket = (float)deformableTerrain.Native.getDynamicMass( shovel.Native );
            // Debug.Log(box.MassProperties.Mass.Value);

            //Calculating some object on the shovel
            var ids = sensor.getContactingParticleIds();
            
            var ps = GetSimulation().getParticleSystem();
            foreach ( var id in ids ) {
                Debug.Log(id);
                var p = ps.getParticle(id);
                Debug.Log((float)p.getMass());
            
                // Kill the particle
                ps.destroyParticle( p );
            }
        }
    }
