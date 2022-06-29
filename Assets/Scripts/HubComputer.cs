using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class HubComputer : MonoBehaviour
{
    public Button arenasButton;
    public Button overviewButton;
    public Button upgradesButton;

    private bool rumbleUnlocked;
    private bool rumbleAvailable;
    public Button unlockRumbleButton;

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

    public GameObject colliseumTime;
    public GameObject factoryTime;
    public GameObject forestTime;
    public GameObject rumbleTime;

    public GameObject colliseumAttempts;
    public GameObject factoryAttempts;
    public GameObject forestAttempts;
    public GameObject rumbleAttempts;

    public GameObject colliseumCurrency;
    public GameObject factoryCurrency;
    public GameObject forestCurrency;
    public GameObject rumbleCurrency;

    public Slider colliseumCurrencySlider;
    public GameObject colliseumCurrencySliderText;
    public Slider factoryCurrencySlider;
    public GameObject factoryCurrencySliderText;
    public Slider forestCurrencySlider;
    public GameObject forestCurrencySliderText;
    public Slider rumbleCurrencySlider;
    public GameObject rumbleCurrencySliderText;

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
        unlockRumbleButton.onClick.AddListener(OnClickUnlockRumble);

        hint.SetActive(false);
        canvas.enabled = false;

        rumbleUnlocked = false; // FIXME: use player prefs value
        rumbleAvailable = false;

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

        colliseumTime.GetComponent<TextMeshProUGUI>().text = buildTimeString(PlayerPrefs.GetInt("ColliseumTimePlayed"));
        factoryTime.GetComponent<TextMeshProUGUI>().text = buildTimeString(PlayerPrefs.GetInt("FactoryTimePlayed"));
        forestTime.GetComponent<TextMeshProUGUI>().text = buildTimeString(PlayerPrefs.GetInt("ForestTimePlayed"));
        rumbleTime.GetComponent<TextMeshProUGUI>().text = buildTimeString(PlayerPrefs.GetInt("RumbleTimePlayed"));

        colliseumAttempts.GetComponent<TextMeshProUGUI>().text = buildAttemptsString(PlayerPrefs.GetInt("ColliseumAttempts"));
        factoryAttempts.GetComponent<TextMeshProUGUI>().text = buildAttemptsString(PlayerPrefs.GetInt("FactoryAttempts"));
        forestAttempts.GetComponent<TextMeshProUGUI>().text = buildAttemptsString(PlayerPrefs.GetInt("ForestAttempts"));
        rumbleAttempts.GetComponent<TextMeshProUGUI>().text = buildAttemptsString(PlayerPrefs.GetInt("RumbleAttempts"));

        colliseumCurrency.GetComponent<TextMeshProUGUI>().text = buildCurrencyString(PlayerPrefs.GetInt("ColliseumCurrency"));
        factoryCurrency.GetComponent<TextMeshProUGUI>().text = buildCurrencyString(PlayerPrefs.GetInt("FactoryCurrency"));
        forestCurrency.GetComponent<TextMeshProUGUI>().text = buildCurrencyString(PlayerPrefs.GetInt("ForestCurrency"));
        rumbleCurrency.GetComponent<TextMeshProUGUI>().text = buildCurrencyString(PlayerPrefs.GetInt("RumbleCurrency"));

        //colliseumCurrencySlider.value = Mathf.Min(PlayerPrefs.GetInt("ColliseumCurrency"), 100);
        colliseumCurrencySliderText.GetComponent<TextMeshProUGUI>().text = buildSliderCurrencyString(PlayerPrefs.GetInt("ColliseumCurrency"));
        //factoryCurrencySlider.value = Mathf.Min(PlayerPrefs.GetInt("FactoryCurrency"), 100);
        factoryCurrencySliderText.GetComponent<TextMeshProUGUI>().text = buildSliderCurrencyString(PlayerPrefs.GetInt("FactoryCurrency"));
        //forestCurrencySlider.value = Mathf.Min(PlayerPrefs.GetInt("ForestCurrency"), 100);
        forestCurrencySliderText.GetComponent<TextMeshProUGUI>().text = buildSliderCurrencyString(PlayerPrefs.GetInt("ForestCurrency"));
        //rumbleCurrencySlider.value = Mathf.Min(PlayerPrefs.GetInt("ColliseumCurrency"), 100);
        //rumbleCurrencySliderText.GetComponent<TextMeshProUGUI>().text = buildSliderCurrencyString(PlayerPrefs.GetInt("ColliseumCurrency"));
    }

    string buildRoundString(int numRounds)
    {
        if (numRounds == 1)
        {
            return numRounds.ToString() + " round";
        }
        else
        {
            return numRounds.ToString() + " rounds";
        }
    }

    string buildKillsString(int numKills)
    {
        if (numKills == 1)
        {
            return numKills.ToString() + " enemy";
        }
        else
        {
            return numKills.ToString() + " enemies";
        }
    }

    string buildAttemptsString(int numAttempts)
    {
        if (numAttempts == 1)
        {
            return numAttempts.ToString() + " attempt";
        }
        else
        {
            return numAttempts.ToString() + " attempts";
        }
    }

    string buildCurrencyString(int currency)
    {
        return currency.ToString("000");
    }

    string buildSliderCurrencyString(int currency)
    {
        return Math.Min(currency, 100).ToString() + "/100";
    }


    string buildTimeString(int time)
    {
        TimeSpan timeInfo = TimeSpan.FromSeconds(time);
        return "" + timeInfo.Hours + "h" + timeInfo.Minutes + "min Time Played";
    }

    void Update()
    {
        if (canvas.enabled)
        {
            UpdateRumbleAvailability();
            UpdateRumbleAvailability();
        }

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

    void UpdateRumbleAvailability()
    {
        if (rumbleAvailable || rumbleUnlocked) return;
        if (PlayerPrefs.GetInt("FactoryCurrency") >= 100 && PlayerPrefs.GetInt("ForestCurrency") >= 100 && PlayerPrefs.GetInt("ColliseumCurrency") >= 100)
        {
            rumbleAvailable = true;
            unlockRumbleButton.gameObject.SetActive(true);
        }
        else
        {
            rumbleAvailable = false;
            unlockRumbleButton.gameObject.SetActive(false);
        }
    }

    void OnClickUnlockRumble()
    {
        // FIXME: save to player prefs (subtract currency and save unlocked)
        rumbleUnlocked = false;
        rumbleAvailable = false;
        unlockRumbleButton.gameObject.SetActive(false);
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
