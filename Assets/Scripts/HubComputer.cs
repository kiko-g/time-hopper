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
    public TextMeshProUGUI moneyCurrentLevelText;
    public TextMeshProUGUI healthCurrentLevelText;
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
        // Get elements
        canvas = GetComponent<Canvas>();
        player = GameObject.Find("PlayerArmature").GetComponent<StarterAssets.ThirdPersonController>();
        pauseCanvas = GameObject.FindGameObjectWithTag("PauseHUD").GetComponent<Canvas>();

        // Listeners
        arenasButton.onClick.AddListener(OnClickArenas);
        overviewButton.onClick.AddListener(OnClickOverview);
        upgradesButton.onClick.AddListener(OnClickUpgrades);
        unlockRumbleButton.onClick.AddListener(OnClickUnlockRumble);
        abilityUpgradeButton.onClick.AddListener(OnClickUpgradeAbility);
        weaponUpgradeButton.onClick.AddListener(OnClickUpgradeWeapon);
        abilityDropdown.onValueChanged.AddListener(delegate
        {
            OnDropdownValueChangedAbility(abilityDropdown);
        });
        weaponDropdown.onValueChanged.AddListener(delegate
        {
            OnDropdownValueChangedWeapon(weaponDropdown);
        });

        factoryCostMultiplier = 1.0f;
        forestCostMultiplier = 1.25f;
        colliseumCostMultiplier = 1.50f;
        rumbleCostMultiplier = 2.0f;

        InitialUIValues();
        UpdateUIValues();
    }

    void Update()
    {
        if (IsWithinRange(player.transform.position))
        {
            if (canvas.enabled) HideHint();
            else ShowHint();
            if (WasPressed()){
                Toggle(!canvas.enabled);
                player._input.interact = false;
            }
        }
        else
        {
            HideHint();
        }

        if (pauseCanvas.enabled) HideAll();
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

    void InitialUIValues()
    {
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
        healthLevel = PlayerPrefs.GetInt("HealthLevel") == 0 ? 1 : PlayerPrefs.GetInt("HealthLevel");
        healthUpgrade.SetActive(true);
        healthCurrentLevelText.text = buildLevelString("Health", healthLevel);
        healthValueText.text = buildAbilityValueString("Health", healthLevel);

        moneyLevel = PlayerPrefs.GetInt("MoneyLevel") == 0 ? 1 : PlayerPrefs.GetInt("MoneyLevel");
        moneyUpgrade.SetActive(false);
        moneyCurrentLevelText.text = buildLevelString("Money", moneyLevel);
        moneyValueText.text = buildAbilityValueString("Money", moneyLevel);

        UpdateCostsText("Ability", healthLevel);
        abilitiesUpgradingText.text = buildUpgradeString(abilityDropdown.options[0].text);
        abilityUpgradeButton.GetComponentInChildren<TextMeshProUGUI>().text = buildAbilityUpgradeButtonString("Health", healthLevel);

        // weapons upgrades
        foreach (GameObject weapon in weapons) weapon.SetActive(false);
        weapons[0].SetActive(true);

        weaponLevels = new int[4];
        for (int i = 0; i < weaponLevels.Length; i++)
            weaponLevels[i] = PlayerPrefs.GetInt("Weapon" + i + "Level") == 0 ? 1 : PlayerPrefs.GetInt("Weapon" + i + "Level");

        weaponsUpgradingText.text = buildUpgradeString(weaponDropdown.options[0].text);
        weaponsCurrentLevelText.text = buildLevelString(weaponDropdown.options[0].text, weaponLevels[0]);
        UpdateCostsText("Weapon", weaponLevels[0]);
        weaponUpgradeButton.GetComponentInChildren<TextMeshProUGUI>().text = buildWeaponUpgradeButtonString(weaponDropdown.options[0].text, weaponLevels[0]);
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

        // can purchase upgrade (upgrades core)
        UpdateUpgradeAvailable();

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
        if (colliseumCurrencyNumber >= 100 && factoryCurrencyNumber >= 100 && forestCurrencyNumber >= 100)
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
        return player._input.interact;
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
    void OnDropdownValueChangedAbility(TMPro.TMP_Dropdown changed)
    {
        abilitiesUpgradingText.text = "Upgrading " + abilityDropdown.options[changed.value].text;
        healthUpgrade.SetActive(false);
        moneyUpgrade.SetActive(false);

        switch (changed.value)
        {
            case 0:
                healthUpgrade.SetActive(true);
                UpdateCostsText("Ability", healthLevel);
                healthCurrentLevelText.text = buildLevelString("Health", healthLevel);
                abilityUpgradeButton.GetComponentInChildren<TextMeshProUGUI>().text = buildAbilityUpgradeButtonString("Health", healthLevel);
                break;
            case 1:
                moneyUpgrade.SetActive(true);
                UpdateCostsText("Ability", moneyLevel);
                moneyCurrentLevelText.text = buildLevelString("Money", moneyLevel);
                abilityUpgradeButton.GetComponentInChildren<TextMeshProUGUI>().text = buildAbilityUpgradeButtonString("Money", moneyLevel);
                break;
            default:
                break;
        }
    }

    void OnDropdownValueChangedWeapon(TMPro.TMP_Dropdown changed)
    {
        int index = changed.value;
        int level = weaponLevels[index];
        string name = weaponDropdown.options[index].text;

        weaponsUpgradingText.text = buildUpgradeString(name);
        foreach (GameObject weapon in weapons) weapon.SetActive(false);

        weapons[index].SetActive(true);
        UpdateCostsText("Weapon", level);
        weaponsCurrentLevelText.text = buildLevelString(name, level);
        weaponUpgradeButton.GetComponentInChildren<TextMeshProUGUI>().text = buildWeaponUpgradeButtonString(name, level);
    }

    void OnClickUpgradeAbility()
    {
        int[] costs;
        switch (abilityDropdown.value)
        {
            case 0:
                costs = getCosts(healthLevel);
                healthLevel++;
                PlayerPrefs.SetInt("HealthLevel", healthLevel);
                PlayerPrefs.SetInt("ColliseumCurrency", PlayerPrefs.GetInt("ColliseumCurrency") - costs[0]);
                PlayerPrefs.SetInt("FactoryCurrency", PlayerPrefs.GetInt("FactoryCurrency") - costs[1]);
                PlayerPrefs.SetInt("ForestCurrency", PlayerPrefs.GetInt("ForestCurrency") - costs[2]);
                PlayerPrefs.SetInt("RumbleCurrency", PlayerPrefs.GetInt("RumbleCurrency") - costs[3]);
                healthValueText.text = buildAbilityValueString("Health", healthLevel);
                healthCurrentLevelText.text = buildLevelString("Health", healthLevel);
                abilityUpgradeButton.GetComponentInChildren<TextMeshProUGUI>().text = buildAbilityUpgradeButtonString("Health", healthLevel);
                UpdateUpgradeAvailable();
                UpdateCostsText("Ability", healthLevel);
                UpdateUserMoney();
                break;
            case 1:
                moneyLevel++;
                costs = getCosts(moneyLevel);
                PlayerPrefs.SetInt("MoneyLevel", moneyLevel);
                PlayerPrefs.SetInt("ColliseumCurrency", PlayerPrefs.GetInt("ColliseumCurrency") - costs[0]);
                PlayerPrefs.SetInt("FactoryCurrency", PlayerPrefs.GetInt("FactoryCurrency") - costs[1]);
                PlayerPrefs.SetInt("ForestCurrency", PlayerPrefs.GetInt("ForestCurrency") - costs[2]);
                PlayerPrefs.SetInt("RumbleCurrency", PlayerPrefs.GetInt("RumbleCurrency") - costs[3]);
                moneyValueText.text = buildAbilityValueString("Money", moneyLevel);
                moneyCurrentLevelText.text = buildLevelString("Money", moneyLevel);
                abilityUpgradeButton.GetComponentInChildren<TextMeshProUGUI>().text = buildAbilityUpgradeButtonString("Money", moneyLevel);
                UpdateUpgradeAvailable();
                UpdateCostsText("Ability", moneyLevel);
                UpdateUserMoney();
                break;
            default:
                break;
        }
    }

    void OnClickUpgradeWeapon()
    {
        int level = weaponLevels[weaponDropdown.value];
        int[] costs = getCosts(level);
        weaponLevels[weaponDropdown.value]++;
        level = weaponLevels[weaponDropdown.value];
        string name = weaponDropdown.options[weaponDropdown.value].text;

        PlayerPrefs.SetInt("Weapon" + weaponDropdown.value + "Level", level);
        PlayerPrefs.SetInt("ColliseumCurrency", PlayerPrefs.GetInt("ColliseumCurrency") - costs[0]);
        PlayerPrefs.SetInt("FactoryCurrency", PlayerPrefs.GetInt("FactoryCurrency") - costs[1]);
        PlayerPrefs.SetInt("ForestCurrency", PlayerPrefs.GetInt("ForestCurrency") - costs[2]);
        PlayerPrefs.SetInt("RumbleCurrency", PlayerPrefs.GetInt("RumbleCurrency") - costs[3]);
        weaponsCurrentLevelText.text = buildLevelString(name, level);
        weaponUpgradeButton.GetComponentInChildren<TextMeshProUGUI>().text = buildWeaponUpgradeButtonString(name, level);
        UpdateUpgradeAvailable();
        UpdateCostsText("Weapon", level);
        UpdateUserMoney();
    }

    void OnClickUnlockRumble()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Project/Menu/button");
        rumbleUnlocked = true;
        PlayerPrefs.SetInt("RumbleUnlocked", 1);
        PlayerPrefs.SetInt("ColliseumCurrency", PlayerPrefs.GetInt("ColliseumCurrency") - 100);
        PlayerPrefs.SetInt("FactoryCurrency", PlayerPrefs.GetInt("FactoryCurrency") - 100);
        PlayerPrefs.SetInt("ForestCurrency", PlayerPrefs.GetInt("ForestCurrency") - 100);
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
        if (numRounds == 1) return numRounds.ToString() + " round";
        else return numRounds.ToString() + " rounds";
    }

    string buildKillsString(int numKills)
    {
        if (numKills == 1) return numKills.ToString() + " enemy";
        else return numKills.ToString() + " enemies";

    }

    string buildAttemptsString(int numAttempts)
    {
        if (numAttempts == 1) return numAttempts.ToString() + " attempt";
        else return numAttempts.ToString() + " attempts";
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

    /* Upgrades */
    int[] getCosts(int level)
    {
        int levelOffset = 4;
        int rumbleStartLevel = Mathf.Max(level - levelOffset, 0);

        int baseCostFac = 10;
        int baseCostFor = 15;
        int baseCostCol = 20;
        int baseCostRumble = rumbleStartLevel == 0 ? 0 : 10;

        return new int[] {
            Mathf.FloorToInt(baseCostCol + level * colliseumCostMultiplier),
            Mathf.FloorToInt(baseCostFac + level * factoryCostMultiplier),
            Mathf.FloorToInt(baseCostFor + level * forestCostMultiplier),
            Mathf.FloorToInt(baseCostRumble + rumbleStartLevel * rumbleCostMultiplier)
        };
    }

    void UpdateUpgradeAvailable()
    {
        // health upgrade
        int[] healthCosts = getCosts(healthLevel);
        bool cantPurchaseHealthUpgrade =
            PlayerPrefs.GetInt("FactoryCurrency") < healthCosts[0] ||
            PlayerPrefs.GetInt("ForestCurrency") < healthCosts[1] ||
            PlayerPrefs.GetInt("ColliseumCurrency") < healthCosts[2] ||
            PlayerPrefs.GetInt("RumbleCurrency") < healthCosts[3];

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

        // money upgrade
        int[] moneyCosts = getCosts(moneyLevel);
        bool cantPurchaseMoneyUpgrade =
            PlayerPrefs.GetInt("FactoryCurrency") < moneyCosts[0] ||
            PlayerPrefs.GetInt("ForestCurrency") < moneyCosts[1] ||
            PlayerPrefs.GetInt("ColliseumCurrency") < moneyCosts[2] ||
            PlayerPrefs.GetInt("RumbleCurrency") < moneyCosts[3];

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

        // weapon upgrades
        foreach (int level in weaponLevels)
        {
            int[] costs = getCosts(level);
            bool cantPurchaseWeaponUpgrade =
                PlayerPrefs.GetInt("FactoryCurrency") < costs[0] ||
                PlayerPrefs.GetInt("ForestCurrency") < costs[1] ||
                PlayerPrefs.GetInt("ColliseumCurrency") < costs[2] ||
                PlayerPrefs.GetInt("RumbleCurrency") < costs[3];

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
        }
    }

    void UpdateCostsText(string type, int level)
    {
        int[] costs = getCosts(level);

        switch (type)
        {
            case "Ability":
            case "Abilities":
            case "Health":
            case "Money":
                abilitiesColiseumCostText.text = buildCurrencyString(costs[0]);
                abilitiesFactoryCostText.text = buildCurrencyString(costs[1]);
                abilitiesForestCostText.text = buildCurrencyString(costs[2]);
                abilitiesRumbleCostText.text = buildCurrencyString(costs[3]);
                break;
            case "Weapon":
            case "Weapons":
                weaponsColiseumCostText.text = buildCurrencyString(costs[0]);
                weaponsFactoryCostText.text = buildCurrencyString(costs[1]);
                weaponsForestCostText.text = buildCurrencyString(costs[2]);
                weaponsRumbleCostText.text = buildCurrencyString(costs[3]);
                break;
            default:
                break;
        }
    }

    void UpdateUserMoney()
    {
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
    }

    string buildUpgradeString(string optionText)
    {
        return "Upgrading " + optionText;
    }

    string buildLevelString(string ability, int level)
    {
        return ability + " current level: " + level;
    }

    string buildAbilityValueString(string ability, int level)
    {
        int step = 0;
        switch (ability)
        {
            case "Health":
                step = 10;
                break;

            case "Money":
                step = 25;
                break;

            default:
                break;
        }

        return (100 + (step * Math.Max(0, level - 1))).ToString();
    }

    string buildAbilityUpgradeButtonString(string ability, int level)
    {
        return "Upgrade " + ability + " to Lvl " + (level + 1);
    }

    string buildWeaponUpgradeButtonString(string weapon, int level)
    {
        string acronym = weapon.Split('(')[1].Split(')')[0];
        return "Upgrade " + acronym + " to Lvl " + (level + 1);
    }
}
