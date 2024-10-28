using UnityEngine;
using UnityEngine.UI;
using AGXUnity;
using AGXUnity.Utils;
using TMPro;
using System.Collections.Generic;
using System;
using UnityEngine.InputSystem;

public class Excavator_Initializing : MonoBehaviour
{
    public ExcavatorScript excavatorScript;
    private Camera_Controller camera_Controller;
    public string modeType;
    // Start is called before the first frame update
    void Start()
    {
        excavatorScript = GetComponent<ExcavatorScript>();
        camera_Controller = GetComponent<Camera_Controller>();

        excavatorScript.enabled = true;
        camera_Controller.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) toggleMode();
        if (Gamepad.current != null) dualMode();
    }

    void toggleMode() {
        camera_Controller.enabled = !camera_Controller.enabled;
        excavatorScript.enabled = !excavatorScript.enabled;
        Debug.Log("changed,....");
    }

    void dualMode() {
        camera_Controller.enabled = true;
        excavatorScript.enabled = true;
    }
}
