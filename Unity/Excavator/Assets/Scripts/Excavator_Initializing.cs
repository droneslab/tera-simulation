using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Excavator_Initializing : MonoBehaviour
{
    private ExcavatorScript excavatorScript;
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
    }

    void toggleMode() {
        camera_Controller.enabled = !camera_Controller.enabled;
        excavatorScript.enabled = !excavatorScript.enabled;
        Debug.Log("changed,....");
    }
}
