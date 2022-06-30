using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class HubComputer : MonoBehaviour
{
    public Canvas canvas;
    public GameObject hint;
    private Canvas pauseCanvas;
    private StarterAssets.ThirdPersonController player;

    // switch tabs buttons
    public Button arenasButton;
    public Button overviewButton;
    public Button upgradesButton;
    public GameObject arenasButtonActive;
    public GameObject overviewButtonActive;
    public GameObject upgradesButtonActive;

    // menu tabs content
    public GameObject arenasCore;
    public GameObject overviewCore;
    public GameObject upgradesCore;

    // sidebar currency (all tabs)
    public GameObject colliseumCurrency;
    public GameObject factoryCurrency;
    public GameObject forestCurrency;
    public GameObject rumbleCurrency;

    // rumble (overview tab)
    private bool rumbleUnlocked;
    private bool rumbleAvailable;
    public TextMeshProUGUI rumbleProgress;
    public Button unlockRumbleButton;
    public GameObject RumblePortal;

    // progress bars (overview tab)
    public GameObject colliseumCurrencyDone;
    public GameObject colliseumCurrencySliderText;
    public GameObject factoryCurrencyDone;
    public GameObject factoryCurrencySliderText;
    public GameObject forestCurrencyDone;
    public GameObject forestCurrencySliderText;

    // best round (arena tab)
    public GameObject colliseumBestRound;
    public GameObject factoryBestRound;
    public GameObject forestBestRound;
    public GameObject rumbleBestRound;

    // total kills (arena tab)
    public GameObject colliseumKills;
    public GameObject factoryKills;
    public GameObject forestKills;
    public GameObject rumbleKills;

    // total time (arena tab)
    public GameObject colliseumTime;
    public GameObject factoryTime;
    public GameObject forestTime;
    public GameObject rumbleTime;

    // total attempts (arena tab)
    public GameObject colliseumAttempts;
    public GameObject factoryAttempts;
    public GameObject forestAttempts;
    public GameObject rumbleAttempts;

    // abilities (upgrades tab)
    public GameObject moneyUpgrade;
    public GameObject healthUpgrade;

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

        overviewCore.SetActive(true); // Overview is the default tab
        arenasCore.SetActive(false);
        upgradesCore.SetActive(false);

        overviewButtonActive.SetActive(true); // Overview is the default tab
        arenasButtonActive.SetActive(false);
        upgradesButtonActive.SetActive(false);

        if (PlayerPrefs.GetInt("RumbleUnlocked") == 1)
        {
            rumbleUnlocked = true;
            RumblePortal.SetActive(true);
        }
        else
        {
            rumbleUnlocked = false;
            RumblePortal.SetActive(false);
        }

        UpdateUIValues();
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
            UpdateUIValues();
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

    void UpdateUIValues()
    {
        // arena best rounds (arenas core)
        colliseumBestRound.GetComponent<TextMeshProUGUI>().text = buildRoundString(PlayerPrefs.GetInt("ColliseumRounds"));
        factoryBestRound.GetComponent<TextMeshProUGUI>().text = buildRoundString(PlayerPrefs.GetInt("FactoryRounds"));
        forestBestRound.GetComponent<TextMeshProUGUI>().text = buildRoundString(PlayerPrefs.GetInt("ForestRounds"));
        rumbleBestRound.GetComponent<TextMeshProUGUI>().text = buildRoundString(PlayerPrefs.GetInt("RumbleRounds"));

        // arena total kills (arenas core)
        colliseumKills.GetComponent<TextMeshProUGUI>().text = buildKillsString(PlayerPrefs.GetInt("ColliseumKills"));
        factoryKills.GetComponent<TextMeshProUGUI>().text = buildKillsString(PlayerPrefs.GetInt("FactoryKills"));
        forestKills.GetComponent<TextMeshProUGUI>().text = buildKillsString(PlayerPrefs.GetInt("ForestKills"));
        rumbleKills.GetComponent<TextMeshProUGUI>().text = buildKillsString(PlayerPrefs.GetInt("RumbleKills"));

        // arena time played strings (arenas core)
        colliseumTime.GetComponent<TextMeshProUGUI>().text = buildTimeString(PlayerPrefs.GetInt("ColliseumTimePlayed"));
        factoryTime.GetComponent<TextMeshProUGUI>().text = buildTimeString(PlayerPrefs.GetInt("FactoryTimePlayed"));
        forestTime.GetComponent<TextMeshProUGUI>().text = buildTimeString(PlayerPrefs.GetInt("ForestTimePlayed"));
        rumbleTime.GetComponent<TextMeshProUGUI>().text = buildTimeString(PlayerPrefs.GetInt("RumbleTimePlayed"));

        // arena attempt strings (arenas core)
        colliseumAttempts.GetComponent<TextMeshProUGUI>().text = buildAttemptsString(PlayerPrefs.GetInt("ColliseumAttempts"));
        factoryAttempts.GetComponent<TextMeshProUGUI>().text = buildAttemptsString(PlayerPrefs.GetInt("FactoryAttempts"));
        forestAttempts.GetComponent<TextMeshProUGUI>().text = buildAttemptsString(PlayerPrefs.GetInt("ForestAttempts"));
        rumbleAttempts.GetComponent<TextMeshProUGUI>().text = buildAttemptsString(PlayerPrefs.GetInt("RumbleAttempts"));

        // sidebar currency strings (all cores)
        int colliseumCurrencyNumber = PlayerPrefs.GetInt("ColliseumCurrency");
        int factoryCurrencyNumber = PlayerPrefs.GetInt("FactoryCurrency");
        int forestCurrencyNumber = PlayerPrefs.GetInt("ForestCurrency");
        int rumbleCurrencyNumber = PlayerPrefs.GetInt("RumbleCurrency");
        colliseumCurrency.GetComponent<TextMeshProUGUI>().text = buildCurrencyString(colliseumCurrencyNumber);
        factoryCurrency.GetComponent<TextMeshProUGUI>().text = buildCurrencyString(factoryCurrencyNumber);
        forestCurrency.GetComponent<TextMeshProUGUI>().text = buildCurrencyString(forestCurrencyNumber);
        rumbleCurrency.GetComponent<TextMeshProUGUI>().text = buildCurrencyString(rumbleCurrencyNumber);

        // rumble unlock progress bars (overview core)
        colliseumCurrencyDone.transform.localScale = new Vector3((float)(0.01f * Mathf.Min(colliseumCurrencyNumber, 100)), 1f, 1f);
        colliseumCurrencySliderText.GetComponent<TextMeshProUGUI>().text = buildSliderCurrencyString(colliseumCurrencyNumber);
        factoryCurrencyDone.transform.localScale = new Vector3((float)(0.01f * Mathf.Min(factoryCurrencyNumber, 100)), 1f, 1f);
        factoryCurrencySliderText.GetComponent<TextMeshProUGUI>().text = buildSliderCurrencyString(factoryCurrencyNumber);
        forestCurrencyDone.transform.localScale = new Vector3((float)(0.01f * Mathf.Min(forestCurrencyNumber, 100)), 1f, 1f);
        forestCurrencySliderText.GetComponent<TextMeshProUGUI>().text = buildSliderCurrencyString(forestCurrencyNumber);

        int newRumbleProgressNumber = 0;
        if (colliseumCurrencyDone.transform.localScale.x == 1f) newRumbleProgressNumber++;
        if (factoryCurrencyDone.transform.localScale.x == 1f) newRumbleProgressNumber++;
        if (forestCurrencyDone.transform.localScale.x == 1f) newRumbleProgressNumber++;
        rumbleProgress.text = newRumbleProgressNumber.ToString() + "/3";

        if (rumbleUnlocked) return;
        if (factoryCurrencyNumber >= 100 && forestCurrencyNumber >= 100 && colliseumCurrencyNumber >= 100)
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

    void OnClickUnlockRumble()
    {
        rumbleUnlocked = true;
        PlayerPrefs.SetInt("RumbleUnlocked", 1);
        PlayerPrefs.SetInt("FactoryCurrency", PlayerPrefs.GetInt("FactoryCurrency") - 100);
        PlayerPrefs.SetInt("ForestCurrency", PlayerPrefs.GetInt("ForestCurrency") - 100);
        PlayerPrefs.SetInt("ColliseumCurrency", PlayerPrefs.GetInt("ColliseumCurrency") - 100);
        RumblePortal.SetActive(true);
        unlockRumbleButton.gameObject.SetActive(false);

        UpdateUIValues();
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
}
