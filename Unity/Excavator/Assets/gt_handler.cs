using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gt_handler : MonoBehaviour
{
    // Start is called before the first frame update
    public float cabin_angle;
    public float cabin_angular_velocity;
    public float boom_angle;
    public float boom_angular_velocity;
    public float arm_angle;
    public float arm_angular_velocity;
    public float bucket_angle;
    public float bucket_angular_velocity;

    float prev_cabin_angle;
    float prev_boom_angle;
    float prev_arm_angle;
    float prev_bucket_angle;

    bool prev_set = false;

    public GameObject cabin_link;
    public GameObject boom_link;
    public GameObject arm_link;
    public GameObject bucket_link;


    // Create 4 2 float arrays to store angle and angular velocity
    float[] cabin = new float[2];
    float[] boom = new float[2];
    float[] arm = new float[2];
    float[] bucket = new float[2];

    Transform cabin_transform, boom_transform, arm_transform, bucket_transform;

    void Start()
    {
        // public variables to store the angles and angular velocities
        // cabin_link = GameObject.Find("Excavator_W_Tracks/onshape/compact_excavator_cabin_body_cmpl/fixed_base/Cabin_link");
        // boom_link = GameObject.Find("Excavator_W_Tracks/onshape/compact_excavator_boom_body_cmpl/part01_pin_1/part02_cmpl/Boom_link");
        // arm_link = GameObject.Find("Excavator_W_Tracks/onshape/compact_excavator_boom_body_cmpl/part01_pin_1/part02_cmpl/part03/Arm_link");
        // bucket_link = GameObject.Find("Excavator_W_Tracks/onshape/compact_excavator_boom_body_cmpl/part01_pin_1/part02_cmpl/part03/part04/Bucket_link");

        cabin_transform = cabin_link.GetComponent<Transform>();
        boom_transform = boom_link.GetComponent<Transform>();
        arm_transform = arm_link.GetComponent<Transform>();
        bucket_transform = bucket_link.GetComponent<Transform>();

    }

    // Update is called once per frame
    void FixedUpdate()
    {
      
        // get the transform of the game objects
        // Transform cabin_transform = cabin_link.GetComponent<Transform>();
        // Transform boom_transform = boom_link.GetComponent<Transform>();
        // Transform arm_transform = arm_link.GetComponent<Transform>();
        // Transform bucket_transform = bucket_link.GetComponent<Transform>();

        // Debug.Log("Cabin: " + cabin_transform.rotation.eulerAngles);
        // Debug.Log("Boom: " + boom_transform.rotation.eulerAngles);
        // Debug.Log("Arm: " + arm_transform.rotation.eulerAngles);
        // Debug.Log("Bucket: " + bucket_transform.rotation.eulerAngles);
        cabin_angle = cabin_transform.rotation.eulerAngles.z;
        boom_angle = boom_transform.rotation.eulerAngles.z;
        arm_angle = arm_transform.rotation.eulerAngles.z;
        bucket_angle = bucket_transform.rotation.eulerAngles.z;

        if(!prev_set)
        {
            prev_cabin_angle = cabin_angle;
            prev_boom_angle = boom_angle;
            prev_arm_angle = arm_angle;
            prev_bucket_angle = bucket_angle;

            cabin_angular_velocity = 0.0f;
            boom_angular_velocity = 0.0f;
            arm_angular_velocity = 0.0f;
            bucket_angular_velocity = 0.0f;

            prev_set = true;
            return;
        }
        else{
            cabin_angular_velocity = (cabin_angle - prev_cabin_angle) * (Mathf.PI/180);
            boom_angular_velocity = (boom_angle - prev_boom_angle) * (Mathf.PI/180);
            arm_angular_velocity = (arm_angle - prev_arm_angle) * (Mathf.PI/180);
            bucket_angular_velocity = (bucket_angle - prev_bucket_angle) * (Mathf.PI/180);

            cabin_angular_velocity = cabin_angular_velocity / Time.fixedDeltaTime;
            boom_angular_velocity = boom_angular_velocity / Time.fixedDeltaTime;
            arm_angular_velocity = arm_angular_velocity / Time.fixedDeltaTime;
            bucket_angular_velocity = bucket_angular_velocity / Time.fixedDeltaTime;

            prev_cabin_angle = cabin_angle;
            prev_boom_angle = boom_angle;
            prev_arm_angle = arm_angle;
            prev_bucket_angle = bucket_angle;

        }

        if(cabin_angle > 180)
        {
            cabin_angle = cabin_angle - 360;
        }
        if(boom_angle > 180)
        {
            boom_angle = boom_angle - 360;
        }
        if(arm_angle > 180)
        {
            arm_angle = arm_angle - 360;
        }
        if(bucket_angle > 180)
        {
            bucket_angle = bucket_angle - 360;
        }

        // boom_angle = boom_angle - cabin_angle;
        // arm_angle = arm_angle - boom_angle;
        // bucket_angle = bucket_angle - arm_angle;

        bucket_angle = bucket_angle - arm_angle;
        arm_angle = arm_angle - boom_angle;
        boom_angle = boom_angle - cabin_angle;

        // Debug.Log($"Cabin: {cabin_angle}, Boom: {boom_angle}, Arm: {arm_angle}, Bucket: {bucket_angle}");
        // Debug.Log($"Cabin Angular Velocity: {cabin_angular_velocity}, Boom Angular Velocity: {boom_angular_velocity}, Arm Angular Velocity: {arm_angular_velocity}, Bucket Angular Velocity: {bucket_angular_velocity}");
        
    }

}
