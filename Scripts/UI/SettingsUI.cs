using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Audio;



public class SettingsUI : MonoBehaviour
{
    public GameObject GameUI;
    public GameObject SettingsUIObject;
    private bool settingsOpen;


    public Volume mainVolume;  // drag your Main Volume here
    private DepthOfField dof;

    public Slider musicSlider;
    public Slider sensitivitySlider;

    public AudioMixer audioMixer;   // drag your mixer here
    public string musicVolumeParam = "MusicVolume"; // expose param name

    private PlayerInput GetPlayerInput()
    {
        // Find the active player (only one should be active)
        var player = GameObject.FindWithTag("Player");
        if (player == null) return null;

        return player.GetComponent<PlayerInput>();
    }


    void Start()
    {
        // existing code...

        if (mainVolume != null)
            mainVolume.profile.TryGet(out dof);
    }




    private int saveSlot;

    public void OnESC()
    {
        if (settingsOpen)
        {
            // CLOSE SETTINGS
            GameUI.SetActive(true);
            SettingsUIObject.SetActive(false);

            var input = GetPlayerInput();
            if (input != null)
                input.SwitchCurrentActionMap("Player");

            settingsOpen = false;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // remove blur
            if (dof != null)
                dof.active = false;

            // unpause
            SimulationManager.Instance.isPaused = false;
        }
        else
        {
            // OPEN SETTINGS
            GameUI.SetActive(false);
            SettingsUIObject.SetActive(true);

            var input = GetPlayerInput();
            if (input != null)
                input.SwitchCurrentActionMap("UI");

            settingsOpen = true;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // apply blur
            if (dof != null)
                dof.active = true;

            // pause
            SimulationManager.Instance.isPaused = true;
        }
    }

    // ----------------------- MUSIC VOLUME -----------------------
    public void OnMusicVolumeChanged(float value)
    {
        // Slider gives linear 0..1 but AudioMixer expects decibels
        float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat(musicVolumeParam, dB);
    }

    // ---------------------- MOUSE SENSITIVITY ----------------------
    public void OnSensitivityChanged(float value)
    {
        var player = GameObject.FindWithTag("Player");
        if (player == null) return;

        var controller = player.GetComponent<PlayerController>();
        if (controller == null) return;

        controller.lookSensitivity = value;
    }



    public void OnSaveGame()
    {
        GameManager.Instance.SaveGame();
    }

    public void OnExitGame()
    {
        GameManager.Instance.ExitGame();
    }

}

