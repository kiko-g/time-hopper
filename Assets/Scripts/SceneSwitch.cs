using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using System.Threading.Tasks;

public class SceneSwitch : MonoBehaviour
{
    public static SceneSwitch Instance;

    [SerializeField]
    private GameObject _loaderCanvas;

    [SerializeField]
    private Slider _progressBar;

    public string ArenaName;

    void Awake()
    {
        if (!PlayerPrefs.HasKey("ColliseumCurrency")) PlayerPrefs.SetInt("ColliseumCurrency", 0);
        if (!PlayerPrefs.HasKey("FactoryCurrency")) PlayerPrefs.SetInt("FactoryCurrency", 0);
        if (!PlayerPrefs.HasKey("ForestCurrency")) PlayerPrefs.SetInt("ForestCurrency", 0);
        if (!PlayerPrefs.HasKey("RumbleCurrency")) PlayerPrefs.SetInt("RumbleCurrency", 0);
        if (!PlayerPrefs.HasKey("ColliseumRounds")) PlayerPrefs.SetInt("ColliseumRounds", 0);
        if (!PlayerPrefs.HasKey("FactoryRounds")) PlayerPrefs.SetInt("FactoryRounds", 0);
        if (!PlayerPrefs.HasKey("ForestRounds")) PlayerPrefs.SetInt("ForestRounds", 0);
        if (!PlayerPrefs.HasKey("RumbleRounds")) PlayerPrefs.SetInt("RumbleRounds", 0);
        if (!PlayerPrefs.HasKey("ColliseumKills")) PlayerPrefs.SetInt("ColliseumKills", 0);
        if (!PlayerPrefs.HasKey("FactoryKills")) PlayerPrefs.SetInt("FactoryKills", 0);
        if (!PlayerPrefs.HasKey("ForestKills")) PlayerPrefs.SetInt("ForestKills", 0);
        if (!PlayerPrefs.HasKey("RumbleKills")) PlayerPrefs.SetInt("RumbleKills", 0);
        if (!PlayerPrefs.HasKey("ColliseumTimePlayed")) PlayerPrefs.SetInt("ColliseumTimePlayed", 0);
        if (!PlayerPrefs.HasKey("FactoryTimePlayed")) PlayerPrefs.SetInt("FactoryTimePlayed", 0);
        if (!PlayerPrefs.HasKey("ForestTimePlayed")) PlayerPrefs.SetInt("ForestTimePlayed", 0);
        if (!PlayerPrefs.HasKey("RumbleTimePlayed")) PlayerPrefs.SetInt("RumbleTimePlayed", 0);
        if (!PlayerPrefs.HasKey("ColliseumAttempts")) PlayerPrefs.SetInt("ColliseumAttempts", 0);
        if (!PlayerPrefs.HasKey("FactoryAttempts")) PlayerPrefs.SetInt("FactoryAttempts", 0);
        if (!PlayerPrefs.HasKey("ForestAttempts")) PlayerPrefs.SetInt("ForestAttempts", 0);
        if (!PlayerPrefs.HasKey("RumbleAttempts")) PlayerPrefs.SetInt("RumbleAttempts", 0);

        PlayerPrefs.Save();

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowLoadingScreen()
    {
    }

    public async void LoadArenaScene(bool death = false)
    {
        if (SceneManager.GetActiveScene().name == "Hub")
        {
            _loaderCanvas.SetActive(true);
            foreach (Transform child in _loaderCanvas.transform)
            {
                if (child.name == ArenaName)
                {
                    child.gameObject.SetActive(true);
                    break;
                }
            }
        }
        var scene = SceneManager.LoadSceneAsync(ArenaName);
        scene.allowSceneActivation = false;
        if (ArenaName != "Hub")
        {
            do
            {
                await Task.Delay(100);
                _progressBar.value += 0.05f;
            } while (scene.progress < 0.9f || _progressBar.value < 0.9f);

            await Task.Delay(1000);
        }
        if (death)
        {
            await Task.Delay(3000);
        }
        scene.allowSceneActivation = true;
    }

    void Update()
    {
        if (GameObject.FindWithTag("LoadingCanvas") == null || GameObject.FindWithTag("LoadingBar") == null)
            return;

        _loaderCanvas = GameObject.FindWithTag("LoadingCanvas");
        _progressBar = GameObject.FindWithTag("LoadingBar").GetComponent<Slider>();

        switch (SceneManager.GetActiveScene().name)
        {
            case "Factory":
            case "Colliseum":
            case "Forest":
            case "Rumble":
                if (_loaderCanvas.activeSelf)
                {
                    _loaderCanvas.GetComponent<Animator>().Play("LoadingFadeOut");
                }
                break;

            default:
                break;
        }
    }

    IEnumerator LoadingScreen()
    {
        yield return null;
    }

    public void setArenaName(string name)
    {
        ArenaName = name;
    }
}