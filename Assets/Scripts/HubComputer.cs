using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HubComputer : MonoBehaviour
{
    public Button arenasButton;
    public Button overviewButton;
    public Button upgradesButton;

    public GameObject arenasCore;
    public GameObject overviewCore;
    public GameObject upgradesCore;

    public GameObject arenasButtonActive;
    public GameObject overviewButtonActive;
    public GameObject upgradesButtonActive;

    public GameObject colliseumBestRound;
    public GameObject factoryBestRound;
    public GameObject forestBestRound;
    public GameObject rumbleBestRound;

    public GameObject colliseumKills;
    public GameObject factoryKills;
    public GameObject forestKills;
    public GameObject rumbleKills;

    public Canvas canvas;
    public GameObject hint;
    private Canvas pauseCanvas;
    private StarterAssets.ThirdPersonController player;

    void Start()
    {
        canvas = GetComponent<Canvas>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<StarterAssets.ThirdPersonController>();
        pauseCanvas = GameObject.FindGameObjectWithTag("PauseHUD").GetComponent<Canvas>();

        arenasButton.onClick.AddListener(OnClickArenas);
        overviewButton.onClick.AddListener(OnClickOverview);
        upgradesButton.onClick.AddListener(OnClickUpgrades);

        hint.SetActive(false);
        canvas.enabled = false;

        arenasCore.SetActive(false);
        upgradesCore.SetActive(false);
        overviewCore.SetActive(true); // Overview is the default tab

        arenasButtonActive.SetActive(false);
        upgradesButtonActive.SetActive(false);
        overviewButtonActive.SetActive(true); // Overview is the default tab

        colliseumBestRound.GetComponent<TextMeshProUGUI>().text = buildRoundString(PlayerPrefs.GetInt("ColliseumRounds"));
        factoryBestRound.GetComponent<TextMeshProUGUI>().text = buildRoundString(PlayerPrefs.GetInt("FactoryRounds"));
        forestBestRound.GetComponent<TextMeshProUGUI>().text = buildRoundString(PlayerPrefs.GetInt("ForestRounds"));
        rumbleBestRound.GetComponent<TextMeshProUGUI>().text = buildRoundString(PlayerPrefs.GetInt("RumbleRounds"));

        colliseumKills.GetComponent<TextMeshProUGUI>().text = buildKillsString(PlayerPrefs.GetInt("ColliseumKills"));
        factoryKills.GetComponent<TextMeshProUGUI>().text = buildKillsString(PlayerPrefs.GetInt("FactoryKills"));
        forestKills.GetComponent<TextMeshProUGUI>().text = buildKillsString(PlayerPrefs.GetInt("ForestKills"));
        rumbleKills.GetComponent<TextMeshProUGUI>().text = buildKillsString(PlayerPrefs.GetInt("RumbleKills"));

    }

    string buildRoundString(int numRounds)
    {
        if (numRounds == 1){
            return numRounds.ToString() + " round";
        } else {
            return numRounds.ToString() + " rounds";
        }
    }

    string buildKillsString(int numKills)
    {
        if (numKills == 1){
            return numKills.ToString() + " enemy";
        } else {
            return numKills.ToString() + " enemies";
        }
    }

    void Update()
    {
        if (IsWithinRange(player.transform.position))
        {
            if (canvas.enabled) HideHint();
            else ShowHint();
            if (WasPressed()) Toggle(!canvas.enabled);
        }
        else
        {
            HideHint();
        }

        if (pauseCanvas.enabled) HideAll();
        // if (Cursor.lockState != CursorLockMode.None) Cursor.lockState = CursorLockMode.None;
    }

    public void Toggle(bool value)
    {
        canvas.enabled = value;
        if (canvas.enabled)
        {
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0f;
            player.SwitchInputToUI();
            HideHint();
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1f;
            player.SwitchInputToPlayer();
            ShowHint();
        }
    }

    bool WasPressed()
    {
        return Input.GetKeyUp(KeyCode.E);
    }

    bool IsWithinRange(Vector3 pos)
    {
        float xMaxLeft = -9f, xMinLeft = -16.0f, zMaxLeft = -5.0f, zMinLeft = -9.0f;
        float xMaxRight = 0.0f, xMinRight = -7.5f, zMaxRight = -5.0f, zMinRight = -9.0f;

        bool left = (pos.x < xMaxLeft && pos.x > xMinLeft && pos.z < zMaxLeft && pos.z > zMinLeft);
        bool right = (pos.x < xMaxRight && pos.x > xMinRight && pos.z < zMaxRight && pos.z > zMinRight);

        return left || right;
    }

    void HideHint()
    {
        if (hint.activeInHierarchy) hint.SetActive(false);
    }

    void ShowHint()
    {
        if (!hint.activeInHierarchy) hint.SetActive(true);
    }

    void HideAll()
    {
        canvas.enabled = false;
        player.SwitchInputToUI();
        HideHint();
    }

    void OnClickOverview()
    {
        overviewButtonActive.SetActive(true);
        arenasButtonActive.SetActive(false);
        upgradesButtonActive.SetActive(false);

        overviewCore.SetActive(true);
        arenasCore.SetActive(false);
        upgradesCore.SetActive(false);
    }

    void OnClickArenas()
    {
        overviewButtonActive.SetActive(false);
        arenasButtonActive.SetActive(true);
        upgradesButtonActive.SetActive(false);

        overviewCore.SetActive(false);
        arenasCore.SetActive(true);
        upgradesCore.SetActive(false);
    }

    void OnClickUpgrades()
    {
        arenasButtonActive.SetActive(false);
        overviewButtonActive.SetActive(false);
        upgradesButtonActive.SetActive(true);

        overviewCore.SetActive(false);
        arenasCore.SetActive(false);
        upgradesCore.SetActive(true);
    }
}
