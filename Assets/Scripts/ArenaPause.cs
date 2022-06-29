using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ArenaPause : MonoBehaviour
{
    // public Button backButton;
    public Button resumeButton;
    public Button settingsButton;
    public Button instructionsButton;
    public Button backSettingsButton;
    public Button backInstructionsButton;

    public GameObject core;
    public GameObject settingsCore;
    public GameObject instructionsCore;

    public Slider sliderSFX;
    public Slider sliderMusic;
    public TextMeshProUGUI textSFX;
    public TextMeshProUGUI textMusic;

    public Canvas canvas;
    private StarterAssets.ThirdPersonController player;

    void Start()
    {
        canvas = GetComponent<Canvas>();
        canvas.enabled = false;

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<StarterAssets.ThirdPersonController>();

        // backButton.onClick.AddListener(OnClickBack);
        resumeButton.onClick.AddListener(OnClickResume);
        settingsButton.onClick.AddListener(OnClickSettings);
        instructionsButton.onClick.AddListener(OnClickInstructions);
        backSettingsButton.onClick.AddListener(OnClickBackSettings);
        backInstructionsButton.onClick.AddListener(OnClickBackInstructions);

        core.SetActive(true); // Main is the default core
        settingsCore.SetActive(false);
        instructionsCore.SetActive(false);

        sliderSFX.value = 0.7f;
        sliderMusic.value = 0.7f;
    }

    void Update()
    {
        UpdateSliders();
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Toggle(!canvas.enabled);
        }
    }

    void UpdateSliders()
    {
        // TODO: Apply slider value to audio values
        AudioListener.volume = sliderSFX.value;

        textSFX.text = "Sound Effects Volume (" + Mathf.RoundToInt(sliderSFX.value * 100) + "%)";
        textMusic.text = "Music Volume (" + Mathf.RoundToInt(sliderMusic.value * 100) + "%)";
    }

    public void Toggle(bool value)
    {
        canvas.enabled = value;
        if (value)
        {
            Time.timeScale = 0f;
            player.SwitchInputToUI();
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Time.timeScale = 1f;
            player.SwitchInputToPlayer();
            Cursor.lockState = CursorLockMode.Locked;
            core.SetActive(true); // Main is the default core
            settingsCore.SetActive(false);
            instructionsCore.SetActive(false);
        }
    }

    void OnClickResume()
    {
        canvas.enabled = false;
        Time.timeScale = 1f;
        player.SwitchInputToPlayer();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void OnClickSettings()
    {
        core.SetActive(false);
        settingsCore.SetActive(true);
        instructionsCore.SetActive(false);
    }

    void OnClickInstructions()
    {
        core.SetActive(false);
        settingsCore.SetActive(false);
        instructionsCore.SetActive(true);
    }

    void OnClickBackSettings()
    {
        core.SetActive(true);
        settingsCore.SetActive(false);
        instructionsCore.SetActive(false);
    }

    void OnClickBackInstructions()
    {
        core.SetActive(true);
        settingsCore.SetActive(false);
        instructionsCore.SetActive(false);
    }

    // void OnClickBack()
    // {
    //     canvas.enabled = false;
    //     Time.timeScale = 1f;
    //     player.SwitchInputToPlayer();
    //     Cursor.lockState = CursorLockMode.Locked;
    // }
}
