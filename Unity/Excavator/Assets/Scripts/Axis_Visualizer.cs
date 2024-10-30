using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axis_Visualizer : MonoBehaviour
{
    public float lineThickness = 0.05f; // Adjust this value to change the thickness
    public Mesh arrowMesh; // Assign the imported Arrow mesh in the inspector

    private void OnDrawGizmos()
    {
        Vector3 origin = transform.position;
        Vector3 scale = Vector3.one * 0.5f; // Scale down by 0.5

        // Draw the X axis in red
        Gizmos.color = Color.red;
        // Gizmos.DrawLine(origin, origin + transform.right);
        Gizmos.DrawMesh(arrowMesh, origin, transform.rotation * Quaternion.Euler(0, 0, -90), scale);

        // Draw the Y axis in green
        Gizmos.color = Color.green;
        // Gizmos.DrawLine(origin, origin + transform.up);
        Gizmos.DrawMesh(arrowMesh, origin, transform.rotation * Quaternion.Euler(0, 0, 0), scale);

        // Draw the Z axis in blue
        Gizmos.color = Color.blue;
        // Gizmos.DrawLine(origin, origin + transform.forward);
        Gizmos.DrawMesh(arrowMesh, origin, transform.rotation * Quaternion.Euler(90, 0, 0), scale);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}