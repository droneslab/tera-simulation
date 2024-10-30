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

    bool sameValue = false;

    float omega_n = 2.0f;
    
    float zeta = 0.8f;
    public float delay = 5.0f;

    public float B = 10.0f;
    public float C = 5.0f;

    ExcavatorCommandMsg previousCommandMessage;

    // Time variables
    DateTime startTime;
    DateTime currentTime;



    void Start()
    {
        lookupOperation = new LookUp_Operation();
        ROSConnection.GetOrCreateInstance().Subscribe<ExcavatorCommandMsg>("joy_deltacan", OnExcavatorCommandReceived);
    }

    float SystemDynamics()
    {
        // Time difference
        TimeSpan timeDifference = currentTime - startTime;
        float deltaTime = (float)timeDifference.TotalSeconds;
        float response;


        // omega_d = omega_n * sqrt(1 - zeta^2);
        // float omega_d = omega_n * Mathf.Sqrt(1 - zeta * zeta);
        // response = omega_n .* (1 - exp(-zeta * omega_n.* t) .* (cos(omega_d .* t) + (zeta./sqrt(1 - zeta^2)) * sin(omega_d .* t)));
        //response = omega_n * (1 - Mathf.Exp(-zeta * omega_n * deltaTime) * (Mathf.Cos(omega_d * deltaTime) + (zeta / Mathf.Sqrt(1 - zeta * zeta)) * Mathf.Sin(omega_d * deltaTime)));

        // wUnity = (1+sin(B.tvec).exp(-C.tvec))
        response = (1 + Mathf.Sin(B * deltaTime) * Mathf.Exp(-C * deltaTime));

        return response;
    }

    void OnExcavatorCommandReceived(ExcavatorCommandMsg commandMessage)
    {
        // Debug.Log(excavatorScript);
        if (excavatorScript != null)
        {
            if (previousCommandMessage != null)
            {
                if (previousCommandMessage.mlefttravelcmd == commandMessage.mlefttravelcmd &&
                    previousCommandMessage.mrighttravelcmd == commandMessage.mrighttravelcmd &&
                    previousCommandMessage.mslewcmd == commandMessage.mslewcmd &&
                    previousCommandMessage.mboomcmd == commandMessage.mboomcmd &&
                    previousCommandMessage.marmcmd == commandMessage.marmcmd &&
                    previousCommandMessage.mbucketcmd == commandMessage.mbucketcmd)
                {
                    currentTime = DateTime.Now;
                    sameValue = true;
                }
                else
                {
                    previousCommandMessage = null;
                    sameValue = false;
                }
            }
            else{
                previousCommandMessage = commandMessage;
                startTime = DateTime.Now;
                return;
            }

            float result = SystemDynamics();

            TimeSpan timeDifference = currentTime - startTime;
            float deltaTime = (float)timeDifference.TotalSeconds;
            
            if (deltaTime > delay)
            {
                result = 1.0f;
            }

            float bucketCommand = lookupOperation.dictionary["mbucketcmd"](commandMessage.mbucketcmd);
            float armCommand = lookupOperation.dictionary["marmcmd"](commandMessage.marmcmd);
            float boomCommand = lookupOperation.dictionary["mboomcmd"](commandMessage.mboomcmd);
            
            bucketCommand = result * bucketCommand;
            armCommand = result * armCommand;
            boomCommand = result * boomCommand;
            
            if (deltaTime < delay)
            {
                Debug.Log($"Bucket Command: {bucketCommand} Result: {result} Current Time: {currentTime} Start Time: {startTime} Delta Time: {deltaTime}");
            }
            

            // Debug.Log(result);
            excavatorScript.MoveExcavatorTracks(commandMessage.mlefttravelcmd, commandMessage.mrighttravelcmd);
            excavatorScript.MoveExcavatorArm(commandMessage.mslewcmd, boomCommand, armCommand, bucketCommand);
        }
    }
}

