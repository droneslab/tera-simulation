using UnityEngine;
using AGXUnity;

public class Excavator_Dynamics : MonoBehaviour {
    public AGXUnity.Model.DeformableTerrainShovel shovel;
    public AGXUnity.RigidBody box;
    float m_massInBucket = 0;
    void Start() {
        Debug.Log("dfd");
        
    }

    void Update() {
        GameObject terrainObject = GameObject.Find("Terrain");
        AGXUnity.Model.DeformableTerrain deformableTerrain = terrainObject.GetComponent<AGXUnity.Model.DeformableTerrain>();
        // AGXUnity.Model.DeformableTerrainShovel shovel = deformableTerrain.GetComponent<AGXUnity.Model.DeformableTerrainShovel>();
        m_massInBucket = (float)deformableTerrain.Native.getDynamicMass( shovel.Native );


        // Debug.Log(box.MassProperties.Mass.Value);
    }
}
