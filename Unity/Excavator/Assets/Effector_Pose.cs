using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



public class Effector_Pose : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject base_link,end_link;
    Transform base_transform, end_transform;
    
    // 6DOF
    Vector3 base_position = new Vector3(0,0,0);
    Vector3 base_orientation = new Vector3(0,0,0);

    Vector3 end_position = new Vector3(0,0,0);
    Vector3 end_orientation = new Vector3(0,0,0);

    Vector3 relative_position = new Vector3(0,0,0);
    Vector3 relative_orientation = new Vector3(0,0,0);

    Quaternion relative_rotation = new Quaternion(0,0,0,0);

    public Vector3 relative_position_E = new Vector3(0,0,0);
    // public Quaternion relative_rotation_E = new Quaternion(0,0,0,0);
    public Vector3 relative_rotation_E = new Vector3(0,0,0);

    public Vector3 base_position_E = new Vector3(0,0,0);
    public Vector3 base_orientation_E = new Vector3(0,0,0);

    public Vector3 end_position_E = new Vector3(0,0,0);
    public Vector3 end_orientation_E = new Vector3(0,0,0);


    void Start()
    {
        base_transform = base_link.GetComponent<Transform>();
        end_transform = end_link.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        base_position = base_transform.position;
        base_orientation = base_transform.eulerAngles;

        end_position = end_transform.position;
        end_orientation = end_transform.eulerAngles;

        GetRelativeTransform(base_position, end_position, base_orientation, end_orientation);


    }


    void GetRelativeTransform(Vector3 p1, Vector3 p2, Vector3 r1, Vector3 r2)
        {
            Matrix4x4 T1 = Matrix4x4.TRS(p1, Quaternion.Euler(r1), new Vector3(1,1,1));
            Matrix4x4 T2 = Matrix4x4.TRS(p2, Quaternion.Euler(r2), new Vector3(1,1,1));

            Matrix4x4 T = T1.inverse;

            Matrix4x4 relative_T = T * T2;

            relative_position_E = relative_T.GetColumn(3);
            relative_rotation_E = relative_T.rotation.eulerAngles;

            

        }   

}
