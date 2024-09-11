using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using ExcavatorCommandMsg = RosMessageTypes.Deltacan.DeltaCanMsg;
using AGXUnity;
using System;

public class ExcavatorROSSubscriber : MonoBehaviour
{
    public ExcavatorScript excavatorScript;

    void Start()
    {
        ROSConnection.GetOrCreateInstance().Subscribe<ExcavatorCommandMsg>("excavator_command_topic", OnExcavatorCommandReceived);
    }

    void OnExcavatorCommandReceived(ExcavatorCommandMsg commandMessage)
    {
        if (excavatorScript != null)
        {
            // Debug.Log(commandMessage);
            excavatorScript.MoveExcavatorTracks(commandMessage.mlefttravelcmd, commandMessage.mrighttravelcmd);
            excavatorScript.MoveExcavatorArm(commandMessage.mslewcmd, commandMessage.mboomcmd, commandMessage.marmcmd, commandMessage.mbucketcmd);
        }
    }
}

