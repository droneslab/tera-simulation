using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using ExcavatorCommandMsg = RosMessageTypes.Deltacan.DeltaCanMsg;
using AGXUnity;

public class ExcavatorROSSubscriber : MonoBehaviour
{
    public ExcavatorScript excavatorScript;
    private LookUp_Operation lookupOperation;

    void Start()
    {
        lookupOperation = new LookUp_Operation();
        ROSConnection.GetOrCreateInstance().Subscribe<ExcavatorCommandMsg>("excavator_command_topic", OnExcavatorCommandReceived);
    }

    void OnExcavatorCommandReceived(ExcavatorCommandMsg commandMessage)
    {
        // Debug.Log(excavatorScript);
        if (excavatorScript != null)
        {
            // Debug.Log(commandMessage);
            float bucketCommand = lookupOperation.dictionary["mbucketcmd"](commandMessage.mbucketcmd);
            //Debug.Log(bucketCommand);
            float armCommand = lookupOperation.dictionary["marmcmd"](commandMessage.marmcmd);
            float boomCommand = lookupOperation.dictionary["mboomcmd"](commandMessage.mboomcmd);

            // Debug.Log(result);
            excavatorScript.MoveExcavatorTracks(commandMessage.mlefttravelcmd, commandMessage.mrighttravelcmd);
            excavatorScript.MoveExcavatorArm(commandMessage.mslewcmd, boomCommand, armCommand, bucketCommand);
        }
    }
}

