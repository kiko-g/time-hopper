using UnityEngine;
using UnityEngine.UI;

public class HubPause : MonoBehaviour
{
    public Canvas canvas;
    public Button exitButton;
    public Button resumeButton;
    public Button settingsButton;
    public Button instructionsButton;
    private StarterAssets.ThirdPersonController player;

    void Start()
    {
        canvas = GetComponent<Canvas>();
        canvas.enabled = false;

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<StarterAssets.ThirdPersonController>();

        exitButton.onClick.AddListener(OnClickExit);
        resumeButton.onClick.AddListener(OnClickResume);
        settingsButton.onClick.AddListener(OnClickSettings);
        instructionsButton.onClick.AddListener(OnClickInstructions);
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Toggle(!canvas.enabled);
        }
    }

    void Hide()
    {
        Time.timeScale = 0f;
        player.SwitchInputToUI();
    }

    void Show()
    {
        Time.timeScale = 1f;
        player.SwitchInputToPlayer();
    }

    public void Toggle(bool value)
    {
        canvas.enabled = value;
        if (value) Hide();
        else Show();
    }

    void OnClickExit()
    {
        Debug.Log("You have clicked the Exit button!");
    }

    void OnClickResume()
    {
        Debug.Log("You have clicked the Resume button!");
    }

    void OnClickSettings()
    {
        Debug.Log("You have clicked the Settings button!");
    }

    void OnClickInstructions()
    {
        Debug.Log("You have clicked the Instructions button!");
    }
}
