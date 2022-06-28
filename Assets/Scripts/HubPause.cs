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
        }
    }

    void OnClickExit()
    {
        canvas.enabled = false;
        Time.timeScale = 1f;
        ExitApplication();
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
        Debug.Log("You have clicked the Settings button!");
    }

    void OnClickInstructions()
    {
        Debug.Log("You have clicked the Instructions button!");
    }

    void ExitApplication()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
