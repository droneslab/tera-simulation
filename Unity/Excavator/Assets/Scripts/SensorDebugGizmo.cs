using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SensorDebugGizmo : MonoBehaviour
{
    public string label = "sensor";
    public Color color = Color.cyan;
    public float markerSize = 0.25f;
    public float frustumLength = 1.0f;

    void OnDrawGizmos()
    {
        DrawMarker();
    }

    void OnDrawGizmosSelected()
    {
        DrawMarker();
    }

    void DrawMarker()
    {
        float size = Mathf.Max(0.01f, markerSize);

        Gizmos.color = color;
        Gizmos.DrawWireSphere(transform.position, size);
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * size * 2.0f);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.right * size);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.up * size);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * size);

        Camera cameraComponent = GetComponent<Camera>();
        if (cameraComponent != null)
        {
            DrawCameraFrustum(cameraComponent, size);
        }

#if UNITY_EDITOR
        Handles.Label(transform.position + Vector3.up * size, label);
#endif
    }

    void DrawCameraFrustum(Camera cameraComponent, float size)
    {
        float length = Mathf.Max(size, frustumLength);
        float halfHeight = Mathf.Tan(cameraComponent.fieldOfView * 0.5f * Mathf.Deg2Rad) * length;
        float halfWidth = halfHeight * cameraComponent.aspect;

        Vector3 center = transform.position + transform.forward * length;
        Vector3 topLeft = center + transform.up * halfHeight - transform.right * halfWidth;
        Vector3 topRight = center + transform.up * halfHeight + transform.right * halfWidth;
        Vector3 bottomLeft = center - transform.up * halfHeight - transform.right * halfWidth;
        Vector3 bottomRight = center - transform.up * halfHeight + transform.right * halfWidth;

        Gizmos.color = color;
        Gizmos.DrawLine(transform.position, topLeft);
        Gizmos.DrawLine(transform.position, topRight);
        Gizmos.DrawLine(transform.position, bottomLeft);
        Gizmos.DrawLine(transform.position, bottomRight);
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
    }
}
