using UnityEngine;
using AGXUnity;

public class Excavator_JointState : MonoBehaviour
{
    private agxROS2.ROS2ControlInterface ros2ControlInterface;
    public string jointCommandTopic = "/bucket_joint_command_topic";
    public string jointStateTopic = "/bucket_joint_state_topic";
    public AGXUnity.Constraint bucketJoint;  // Reference to the bucket joint (could be Hinge or Prismatic)

    void Start()
    {
        // Initialize ROS2ControlInterface
        ros2ControlInterface = new agxROS2.ROS2ControlInterface(jointCommandTopic, jointStateTopic, true);

        // Add the bucket joint to the control interface
        ros2ControlInterface.addJoint(bucketJoint.Native as agx.Constraint1DOF, agxROS2.ROS2ControlInterface.ROS2ControlCommandInterface.VELOCITY);

        // Add the control interface to the simulation
        Simulation.Instance.Native.add(ros2ControlInterface);
    }

    void FixedUpdate()
    {
        // You can log the bucket joint's current state here if needed
        // Debug.Log("Bucket joint position: " + bucketJoint);
        ros2ControlInterface.post(Time.fixedTime);
    }

    void OnDestroy()
    {
        // Clean up
        // ros2ControlInterface.removeJoint(bucketJoint.Native as agx.Constraint1DOF);
        // Simulation.Instance.Native.remove(ros2ControlInterface);
    }
}
