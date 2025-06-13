using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // Ensure you have this for Gamepad input
using UnityEngine.UI; // For Toggle UI components

public class MainMenu : MonoBehaviour
{
    public bool sensorEnabled = false;
    public bool loadCustomTerrain = false;
    public bool loadCustomYAML = false;

    private Gamepad gamepad;

    // References to UI Toggles
    public Toggle sensorToggleButton;
    public Toggle terrainToggleButton;
    public Toggle yamlToggleButton;

    // References to UI Buttons
    public Button playButton;
    public Button quitButton;
    public Button optionsButton;
    public GameObject optionsMenu;

    void Start()
    {
        // Initialize the gamepad (if available)
        gamepad = Gamepad.current;

        // Optionally set the default button color when the game starts
        SetButtonDefaultColor(playButton);
        SetButtonDefaultColor(quitButton);
        SetButtonDefaultColor(optionsButton);

        sensorToggleButton.isOn = false;
        terrainToggleButton.isOn = false;
        yamlToggleButton.isOn = false;  
    }

    void Update()
    {
        if (gamepad != null) {
            HandleGamepadInput();
        }
    }

    // This function handles gamepad input to navigate and interact with the menu
    void HandleGamepadInput()
    {
        // Check for gamepad button presses
        if (gamepad.buttonSouth.wasPressedThisFrame) // A button
        {
            playGame();
            SetButtonPressedColor(playButton);
        }

        if (gamepad.buttonEast.wasPressedThisFrame) // B button
        {
            quitGame();
            SetButtonPressedColor(quitButton);
        }

        if (gamepad.buttonWest.wasPressedThisFrame) // X button
        {
            OpenOptionsMenu();  // Trigger the desired action when "X" is pressed
            Debug.Log("X button pressed, opening options menu.");
        }

        if (gamepad.dpad.up.wasPressedThisFrame) // D-pad Up
        {
            ToggleSensor();
            SetToggleColor(sensorToggleButton);
        }

        if (gamepad.dpad.down.wasPressedThisFrame) // D-pad Down
        {
            ToggleCustomTerrain();
            SetToggleColor(terrainToggleButton);
        }

        if (gamepad.dpad.left.wasPressedThisFrame) // D-pad Left
        {
            ToggleCustomYAML();
            SetToggleColor(yamlToggleButton);
        }

        if(gamepad.startButton.wasPressedThisFrame) {
            SceneManager.LoadSceneAsync("MenuScene");
        }
    }

    // Set the button's default color (not pressed)
    void SetButtonDefaultColor(Button button)
    {
        ColorBlock colorBlock = button.colors;
        colorBlock.normalColor = Color.white; // Normal state color
        colorBlock.highlightedColor = Color.green; // Highlighted state color
        colorBlock.pressedColor = Color.red; // Pressed state color
        button.colors = colorBlock;
    }

    // Change the color when the button is pressed
    void SetButtonPressedColor(Button button)
    {
        Color lightBlue = new Color(0.678f, 0.847f, 0.902f);
        ColorBlock colorBlock = button.colors;
        colorBlock.normalColor = lightBlue; // Color when pressed
        button.colors = colorBlock;

        // Optionally reset the color to normal after a brief delay
        StartCoroutine(ResetButtonColor(button));
    }

    // Reset the button's color to the default after a short delay
    IEnumerator ResetButtonColor(Button button)
    {
        yield return new WaitForSeconds(0.2f); // Delay before resetting
        SetButtonDefaultColor(button); // Reset to default color
    }

    // Change the color when a toggle is pressed
    void SetToggleColor(Toggle toggle)
    {
        ColorBlock colorBlock = toggle.GetComponentInChildren<Button>().colors;
        colorBlock.normalColor = toggle.isOn ? Color.green : Color.red; // Green if toggled on, red if off
        toggle.GetComponentInChildren<Button>().colors = colorBlock;
    }

    // Function to toggle the sensorEnabled state
    public void ToggleSensor()
    {
        sensorEnabled = !sensorEnabled;
        sensorToggleButton.isOn = sensorEnabled; // Update the toggle state
        Debug.Log("Sensor Enabled: " + sensorEnabled);
    }

    // Function to toggle the loadCustomTerrain state
    public void ToggleCustomTerrain()
    {
        loadCustomTerrain = !loadCustomTerrain;
        terrainToggleButton.isOn = loadCustomTerrain; // Update the toggle state
        Debug.Log("Load Custom Terrain: " + loadCustomTerrain);
    }

    // Function to toggle the loadCustomYAML state
    public void ToggleCustomYAML()
    {
        loadCustomYAML = !loadCustomYAML;
        yamlToggleButton.isOn = loadCustomYAML; // Update the toggle state
        Debug.Log("Load Custom YAML: " + loadCustomYAML);
    }

    // Existing functions to interact with the menu
    public void playGame() {
        
        if (sensorToggleButton.isOn) {
            SceneManager.LoadSceneAsync("Sensor_Scene");
        } 
        else if (terrainToggleButton.isOn) {
            SceneManager.LoadSceneAsync("CustomTerrainScene");
        }
        else if (yamlToggleButton.isOn) {
            SceneManager.LoadSceneAsync("YAML_Scene");
        }
        else {
            SceneManager.LoadSceneAsync("SampleScene");
        }
    }

    // Function to open options menu
    public void OpenOptionsMenu() {
        if (optionsMenu != null)
        {
            // Toggle the active state of the optionsMenu GameObject
            optionsMenu.SetActive(!optionsMenu.activeSelf);
            Debug.Log("Options Menu Toggled. Active: " + optionsMenu.activeSelf);
        }
        else
        {
            Debug.LogError("Options Menu GameObject is not assigned.");
        }
    }

    public void quitGame() {
        Debug.Log("Close");
        Application.Quit();
    }
}
