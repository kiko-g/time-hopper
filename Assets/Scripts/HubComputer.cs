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

    // upgrade multipliers (upgrades tab)
    private float colliseumCostMultiplier;
    private float factoryCostMultiplier;
    private float forestCostMultiplier;
    private float rumbleCostMultiplier;

    // abilities (upgrades tab)
    public TMPro.TMP_Dropdown abilityDropdown;
    public TextMeshProUGUI abilitiesUpgradingText;
    public Button abilityUpgradeButton;
    public GameObject moneyUpgrade;
    public GameObject healthUpgrade;
    public TextMeshProUGUI moneyLevelText;
    public TextMeshProUGUI healthLevelText;
    public TextMeshProUGUI moneyValueText;
    public TextMeshProUGUI healthValueText;
    public TextMeshProUGUI abilitiesColiseumCostText;
    public TextMeshProUGUI abilitiesFactoryCostText;
    public TextMeshProUGUI abilitiesForestCostText;
    public TextMeshProUGUI abilitiesRumbleCostText;
    private int moneyLevel;
    private int healthLevel;

    // weapons (upgrades tab)
    public TMPro.TMP_Dropdown weaponDropdown;
    public TextMeshProUGUI weaponsUpgradingText;
    public TextMeshProUGUI weaponsCurrentLevelText;
    public Button weaponUpgradeButton;
    public TextMeshProUGUI weaponsColiseumCostText;
    public TextMeshProUGUI weaponsFactoryCostText;
    public TextMeshProUGUI weaponsForestCostText;
    public TextMeshProUGUI weaponsRumbleCostText;
    public GameObject[] weapons;
    private int[] weaponLevels;

    void Start()
    {
        canvas = GetComponent<Canvas>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<StarterAssets.ThirdPersonController>();
        pauseCanvas = GameObject.FindGameObjectWithTag("PauseHUD").GetComponent<Canvas>();

        arenasButton.onClick.AddListener(OnClickArenas);
        overviewButton.onClick.AddListener(OnClickOverview);
        upgradesButton.onClick.AddListener(OnClickUpgrades);
        unlockRumbleButton.onClick.AddListener(OnClickUnlockRumble);
        abilityUpgradeButton.onClick.AddListener(OnClickUpgradeAbility);
        weaponUpgradeButton.onClick.AddListener(OnClickUpgradeWeapon);

        abilityDropdown.onValueChanged.AddListener(delegate
        {
            OnAbilityDropdownValueChanged(abilityDropdown);
        });

        weaponDropdown.onValueChanged.AddListener(delegate
        {
            OnWeaponDropdownValueChanged(weaponDropdown);
        });

        // Initial Values
        hint.SetActive(false);
        canvas.enabled = false;

        // tabs
        overviewCore.SetActive(true); // Overview is the default tab
        arenasCore.SetActive(false);
        upgradesCore.SetActive(false);
        overviewButtonActive.SetActive(true); // Overview is the default tab
        arenasButtonActive.SetActive(false);
        upgradesButtonActive.SetActive(false);

        // rumble status
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

        // abilities upgrades
        factoryCostMultiplier = 1.0f;
        forestCostMultiplier = 1.25f;
        colliseumCostMultiplier = 1.50f;
        rumbleCostMultiplier = 0.25f;

        moneyLevel = PlayerPrefs.GetInt("MoneyLevel") == 0 ? 1 : PlayerPrefs.GetInt("MoneyLevel");
        healthLevel = PlayerPrefs.GetInt("HealthLevel") == 0 ? 1 : PlayerPrefs.GetInt("HealthLevel");

        abilitiesUpgradingText.text = "Upgrading " + abilityDropdown.options[0].text;
        healthUpgrade.SetActive(true);
        moneyUpgrade.SetActive(false);

        moneyValueText.text = (100 + 25 * (moneyLevel - 1)).ToString();
        healthValueText.text = (100 + 10 * (healthLevel - 1)).ToString();

        abilitiesColiseumCostText.text = buildCurrencyString(Mathf.FloorToInt(healthLevel * colliseumCostMultiplier));
        abilitiesFactoryCostText.text = buildCurrencyString(Mathf.FloorToInt(healthLevel * factoryCostMultiplier));
        abilitiesForestCostText.text = buildCurrencyString(Mathf.FloorToInt(healthLevel * forestCostMultiplier));
        abilitiesRumbleCostText.text = buildCurrencyString(Mathf.FloorToInt(healthLevel * rumbleCostMultiplier));
        abilityUpgradeButton.GetComponentInChildren<TextMeshProUGUI>().text = "Upgrade Health to Lvl " + (healthLevel + 1);

        // weapons upgrades
        foreach (GameObject weapon in weapons) weapon.SetActive(false);
        weapons[0].SetActive(true);
        weaponLevels = new int[4] {
            PlayerPrefs.GetInt("Weapon0Level") == 0 ? 1 : PlayerPrefs.GetInt("Weapon0Level"),
            PlayerPrefs.GetInt("Weapon1Level") == 0 ? 1 : PlayerPrefs.GetInt("Weapon1Level"),
            PlayerPrefs.GetInt("Weapon2Level") == 0 ? 1 : PlayerPrefs.GetInt("Weapon2Level"),
            PlayerPrefs.GetInt("Weapon3Level") == 0 ? 1 : PlayerPrefs.GetInt("Weapon3Level")
        };
        weaponsUpgradingText.text = "Upgrading " + weaponDropdown.options[0].text;
        weaponsCurrentLevelText.text = weaponDropdown.options[0].text + " current level: " + weaponLevels[0];
        weaponsColiseumCostText.text = buildCurrencyString(Mathf.FloorToInt(weaponLevels[0] * colliseumCostMultiplier));
        weaponsFactoryCostText.text = buildCurrencyString(Mathf.FloorToInt(weaponLevels[0] * factoryCostMultiplier));
        weaponsForestCostText.text = buildCurrencyString(Mathf.FloorToInt(weaponLevels[0] * forestCostMultiplier));
        weaponsRumbleCostText.text = buildCurrencyString(Mathf.FloorToInt(weaponLevels[0] * rumbleCostMultiplier));
        weaponUpgradeButton.GetComponentInChildren<TextMeshProUGUI>().text = "Upgrade Weapon to Lvl " + (weaponLevels[0] + 1);

        // Refreshable UI Values
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

        // upgrades buttons (upgrades core)
        // health upgrade
        if (abilityDropdown.value == 0)
        {
            bool cantPurchaseHealthUpgrade =
                (float)colliseumCurrencyNumber < (float)(healthLevel * colliseumCostMultiplier) ||
                (float)factoryCurrencyNumber < (float)(healthLevel * factoryCostMultiplier) ||
                (float)forestCurrencyNumber < (float)(healthLevel * forestCostMultiplier) ||
                (float)rumbleCurrencyNumber < (float)(healthLevel * rumbleCostMultiplier);

            if (cantPurchaseHealthUpgrade)
            {
                abilityUpgradeButton.interactable = false;
                abilityUpgradeButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
            }
            else
            {
                abilityUpgradeButton.interactable = true;
                abilityUpgradeButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
            }
        }

        // money upgrade
        if (abilityDropdown.value == 1)
        {
            bool cantPurchaseMoneyUpgrade =
                (float)colliseumCurrencyNumber < (float)(moneyLevel * colliseumCostMultiplier) ||
                (float)factoryCurrencyNumber < (float)(moneyLevel * factoryCostMultiplier) ||
                (float)forestCurrencyNumber < (float)(moneyLevel * forestCostMultiplier) ||
                (float)rumbleCurrencyNumber < (float)(moneyLevel * rumbleCostMultiplier);

            if (cantPurchaseMoneyUpgrade)
            {
                abilityUpgradeButton.interactable = false;
                abilityUpgradeButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
            }
            else
            {
                abilityUpgradeButton.interactable = true;
                abilityUpgradeButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
            }
        }

        // weapon upgrades
        bool cantPurchaseWeaponUpgrade =
                (float)colliseumCurrencyNumber < (float)(weaponLevels[weaponDropdown.value] * colliseumCostMultiplier) ||
                (float)factoryCurrencyNumber < (float)(weaponLevels[weaponDropdown.value] * factoryCostMultiplier) ||
                (float)forestCurrencyNumber < (float)(weaponLevels[weaponDropdown.value] * forestCostMultiplier) ||
                (float)rumbleCurrencyNumber < (float)(weaponLevels[weaponDropdown.value] * rumbleCostMultiplier);

        if (cantPurchaseWeaponUpgrade)
        {
            weaponUpgradeButton.interactable = false;
            weaponUpgradeButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
        }
        else
        {
            weaponUpgradeButton.interactable = true;
            weaponUpgradeButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
        }

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

    // On value changed functions
    void OnAbilityDropdownValueChanged(TMPro.TMP_Dropdown changed)
    {
        abilitiesUpgradingText.text = "Upgrading " + abilityDropdown.options[changed.value].text;
        healthUpgrade.SetActive(false);
        moneyUpgrade.SetActive(false);

        switch (changed.value)
        {
            case 0:
                healthUpgrade.SetActive(true);
                abilitiesColiseumCostText.text = buildCurrencyString(Mathf.FloorToInt(healthLevel * colliseumCostMultiplier));
                abilitiesFactoryCostText.text = buildCurrencyString(Mathf.FloorToInt(healthLevel * factoryCostMultiplier));
                abilitiesForestCostText.text = buildCurrencyString(Mathf.FloorToInt(healthLevel * forestCostMultiplier));
                abilitiesRumbleCostText.text = buildCurrencyString(Mathf.FloorToInt(healthLevel * rumbleCostMultiplier));
                abilityUpgradeButton.GetComponentInChildren<TextMeshProUGUI>().text = "Upgrade Health to Lvl " + (healthLevel + 1);
                break;
            case 1:
                moneyUpgrade.SetActive(true);
                abilitiesColiseumCostText.text = buildCurrencyString(Mathf.FloorToInt(moneyLevel * colliseumCostMultiplier));
                abilitiesFactoryCostText.text = buildCurrencyString(Mathf.FloorToInt(moneyLevel * factoryCostMultiplier));
                abilitiesForestCostText.text = buildCurrencyString(Mathf.FloorToInt(moneyLevel * forestCostMultiplier));
                abilitiesRumbleCostText.text = buildCurrencyString(Mathf.FloorToInt(moneyLevel * rumbleCostMultiplier));
                abilityUpgradeButton.GetComponentInChildren<TextMeshProUGUI>().text = "Upgrade Money to Lvl " + (moneyLevel + 1);
                break;
            default:
                break;
        }
    }

    void OnWeaponDropdownValueChanged(TMPro.TMP_Dropdown changed)
    {
        int index = changed.value;
        int level = weaponLevels[index];
        string name = weaponDropdown.options[index].text;

        weaponsUpgradingText.text = "Upgrading " + name;
        foreach (GameObject weapon in weapons) weapon.SetActive(false);

        weapons[index].SetActive(true);
        weaponsColiseumCostText.text = buildCurrencyString(Mathf.FloorToInt(level * colliseumCostMultiplier));
        weaponsFactoryCostText.text = buildCurrencyString(Mathf.FloorToInt(level * factoryCostMultiplier));
        weaponsForestCostText.text = buildCurrencyString(Mathf.FloorToInt(level * forestCostMultiplier));
        weaponsRumbleCostText.text = buildCurrencyString(Mathf.FloorToInt(level * rumbleCostMultiplier));
        weaponsCurrentLevelText.text = name + " current Level " + level;
        weaponUpgradeButton.GetComponentInChildren<TextMeshProUGUI>().text = "Upgrade Weapon to Lvl " + (level + 1);
    }

    void OnClickUpgradeAbility()
    {
        switch (abilityDropdown.value)
        {
            case 0:
                moneyLevel++;
                PlayerPrefs.SetInt("MoneyLevel", moneyLevel);
                PlayerPrefs.SetInt("FactoryCurrency", PlayerPrefs.GetInt("FactoryCurrency") - Mathf.FloorToInt(moneyLevel * colliseumCostMultiplier));
                PlayerPrefs.SetInt("ForestCurrency", PlayerPrefs.GetInt("ForestCurrency") - Mathf.FloorToInt(moneyLevel * factoryCostMultiplier));
                PlayerPrefs.SetInt("ColliseumCurrency", PlayerPrefs.GetInt("ColliseumCurrency") - Mathf.FloorToInt(moneyLevel * forestCostMultiplier));
                PlayerPrefs.SetInt("RumbleCurrency", PlayerPrefs.GetInt("RumbleCurrency") - Mathf.FloorToInt(moneyLevel * rumbleCostMultiplier));
                moneyValueText.text = (100 + 25 * (moneyLevel - 1)).ToString();
                break;
            case 1:
                healthLevel++;
                PlayerPrefs.SetInt("HealthLevel", healthLevel);
                PlayerPrefs.SetInt("FactoryCurrency", PlayerPrefs.GetInt("FactoryCurrency") - Mathf.FloorToInt(healthLevel * colliseumCostMultiplier));
                PlayerPrefs.SetInt("ForestCurrency", PlayerPrefs.GetInt("ForestCurrency") - Mathf.FloorToInt(healthLevel * factoryCostMultiplier));
                PlayerPrefs.SetInt("ColliseumCurrency", PlayerPrefs.GetInt("ColliseumCurrency") - Mathf.FloorToInt(healthLevel * forestCostMultiplier));
                PlayerPrefs.SetInt("RumbleCurrency", PlayerPrefs.GetInt("RumbleCurrency") - Mathf.FloorToInt(healthLevel * rumbleCostMultiplier));
                healthValueText.text = (100 + 10 * (healthLevel - 1)).ToString();
                break;
            default:
                break;
        }
    }

    void OnClickUpgradeWeapon()
    {
        weaponLevels[abilityDropdown.value]++;
        PlayerPrefs.SetInt("Weapon" + abilityDropdown.value + "Level", weaponLevels[abilityDropdown.value]);
        PlayerPrefs.SetInt("FactoryCurrency", PlayerPrefs.GetInt("FactoryCurrency") - Mathf.FloorToInt(weaponLevels[abilityDropdown.value] * colliseumCostMultiplier));
        PlayerPrefs.SetInt("ForestCurrency", PlayerPrefs.GetInt("ForestCurrency") - Mathf.FloorToInt(weaponLevels[abilityDropdown.value] * factoryCostMultiplier));
        PlayerPrefs.SetInt("ColliseumCurrency", PlayerPrefs.GetInt("ColliseumCurrency") - Mathf.FloorToInt(weaponLevels[abilityDropdown.value] * forestCostMultiplier));
        PlayerPrefs.SetInt("RumbleCurrency", PlayerPrefs.GetInt("RumbleCurrency") - Mathf.FloorToInt(weaponLevels[abilityDropdown.value] * rumbleCostMultiplier));
        weaponsCurrentLevelText.text = weaponDropdown.options[abilityDropdown.value].text + " current level: " + weaponLevels[abilityDropdown.value];
    }

    void OnClickUnlockRumble()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Project/Menu/button");
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
        FMODUnity.RuntimeManager.PlayOneShot("event:/Project/Menu/button");
        overviewButtonActive.SetActive(true);
        arenasButtonActive.SetActive(false);
        upgradesButtonActive.SetActive(false);

        overviewCore.SetActive(true);
        arenasCore.SetActive(false);
        upgradesCore.SetActive(false);
    }

    void OnClickArenas()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Project/Menu/button");
        overviewButtonActive.SetActive(false);
        arenasButtonActive.SetActive(true);
        upgradesButtonActive.SetActive(false);

        overviewCore.SetActive(false);
        arenasCore.SetActive(true);
        upgradesCore.SetActive(false);
    }

    void OnClickUpgrades()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Project/Menu/button");
        arenasButtonActive.SetActive(false);
        overviewButtonActive.SetActive(false);
        upgradesButtonActive.SetActive(true);

        overviewCore.SetActive(false);
        arenasCore.SetActive(false);
        upgradesCore.SetActive(true);
    }

    // Auxiliary functions
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
