using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public void OnYAMLButtonClick()
    {
        // Load the YAML scene
        // SceneManager.LoadScene("YAMLScene");
        Debug.Log("YAML Button Click");
    }

    public void OnPrefabButtonClick()
    {
        // Load the Prefab scene
        SceneManager.LoadScene("Sensor_Scene");
        Debug.Log("Sensor Prefab Click");
    }

    public void OnNoSensorButtonClick()
    {
        // Load the No Sensor scene
        // SceneManager.LoadScene("NoSensorScene");
        Debug.Log("Sensorless Prefab Click");
    }
}
