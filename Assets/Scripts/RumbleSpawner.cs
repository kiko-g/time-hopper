using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class RumbleSpawner : MonoBehaviour
{

    [SerializeField]
    private GameObject colliseumEnemy;
    [SerializeField]
    private GameObject forestEnemy;
    [SerializeField]
    private GameObject factoryEnemy;

    [SerializeField]
    private GameObject colEnemiesHolder;
    [SerializeField]
    private GameObject forEnemiesHolder;
    [SerializeField]
    private GameObject facEnemiesHolder;

    [SerializeField]
    public GameObject enterTooltipUI;

    [SerializeField]
    private Text roundUI;

    [SerializeField]
    private BackgroundMusicPlayer backgroundMusicPlayer;

    private float startTime;
    private bool startRoundText;

    public string currentScene;

    [SerializeField]
    private TextMeshProUGUI startRoundTextUI;

    [SerializeField]
    private GameObject enemiesLeftUI;
    
    private Text enemiesLeftText;

    private bool round_active = false;
    private bool spawning = false;
    private int totalEnemiesToDefeat;
    private int enemiesKilled = 0;
    private int enemiesLeft = 0;
    public int roundNr = 0;
    public int numKills = 0;
    public float timePlayed = 0f;
    private float roundTime = 0f, spawnEnemyTime = 0f;

    private bool startRoundFlag = false;
    private bool spawn_in_progress = false;

    private Vector3 colliseumSpawn;
    private Vector3 forestSpawn;
    private Vector3 factorySpawn;

    // Start is called before the first frame update
    void Start()
    {
        enemiesLeftText = enemiesLeftUI.GetComponent<Text>();
        //get all rumble planes
        GameObject[] rumblePlanes = GameObject.FindGameObjectsWithTag("RumblePlane");
        //get all spawnpositions from rumble planes
        foreach (GameObject rumblePlane in rumblePlanes)
        {
            if(rumblePlane.name == "ColliseumPlane")
            {
                colliseumSpawn = rumblePlane.GetComponent<RumblePlane>().getSpawnPosition();
                colliseumSpawn = new Vector3(colliseumSpawn.x, colliseumSpawn.y + 2f, colliseumSpawn.z);
            }
            else if(rumblePlane.name == "ForestPlane")
            {
                forestSpawn = rumblePlane.GetComponent<RumblePlane>().getSpawnPosition();
                forestSpawn = new Vector3(forestSpawn.x, forestSpawn.y + 2f, forestSpawn.z);
            }
            else if(rumblePlane.name == "FactoryPlane")
            {
                factorySpawn = rumblePlane.GetComponent<RumblePlane>().getSpawnPosition();
                factorySpawn = new Vector3(factorySpawn.x, factorySpawn.y + 2f, factorySpawn.z);
            }
        }
        enterTooltipUI.SetActive(true);
    }

    void Update()
    {
        timePlayed += Time.deltaTime;
        if (startRoundFlag && !round_active && enemiesLeft == 0)
        {
            round_active = true;
            StartRound();
        }
        if(spawning && !spawn_in_progress && (Time.time - spawnEnemyTime >= 2.0f)){
            spawn_in_progress = true;
            SpawnEnemy();
            spawn_in_progress = false;
        }
        if (startRoundText){
            if (startRoundTextUI.gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime == 1.0f){
                startRoundTextUI.gameObject.SetActive(false);
            }
        }
    }

    // euclidean distance function
    float Distance(Vector3 a, Vector3 b)
    {
        return Mathf.Sqrt(Mathf.Pow(a.x - b.x, 2) + Mathf.Pow(a.z - b.z, 2));
    }

    public void setCurrentScene(string sceneName)
    {
        backgroundMusicPlayer.startNewMusic(sceneName);
        currentScene = sceneName;
    }

    public void increaseEnemiesKilled(){
        enemiesKilled++;
        numKills++;
        enemiesLeft = totalEnemiesToDefeat - enemiesKilled;
        enemiesLeftText.text = enemiesLeft.ToString();
        if(!round_active && !spawning){
            enemiesLeft = 0;
            enemiesKilled = 0;
            enemiesLeftText.text = enemiesLeft.ToString();
        }
        if(enemiesLeft <= 0){
            spawning = false;
            round_active = false;
            foreach (Transform child in colEnemiesHolder.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in forEnemiesHolder.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in facEnemiesHolder.transform)
            {
                Destroy(child.gameObject);
            }
            roundTime = Time.time - startTime;
            enterTooltipUI.SetActive(true);
            enemiesKilled = 0;
            enemiesLeft = 0;
        }
    }

    public bool setStartRoundFlag(bool flag){
        if(startRoundFlag == false && flag == true){
            startRoundFlag = flag;
            return true;
        }
        else{
            startRoundFlag = flag;
            return false;
        }
    }

    public void StartRound()
    {
        if(PlayerPrefs.GetInt("RumbleUnlocked") == 1){
            PlayerPrefs.SetInt("RumbleUnlocked", 0);
        }
        PlayerPrefs.Save();
        enterTooltipUI.SetActive(false);
        startTime = Time.time;
        startRoundFlag = false;
        roundNr++;
        ShowRoundStartUI();
        roundUI.text = "Round: " + roundNr;
        totalEnemiesToDefeat = 10 + (5*roundNr);
        if(totalEnemiesToDefeat > 50){
            totalEnemiesToDefeat = 50;
        }
        enemiesLeft = totalEnemiesToDefeat;
        spawning = true;
        enemiesLeftText.text = enemiesLeft.ToString();
    }

    void SpawnEnemy(){
        switch(currentScene){
            case "ColliseumPlane":
                if(colEnemiesHolder.transform.childCount >= 15) {
                    return;
                }
                break;
            case "ForestPlane":
                if(forEnemiesHolder.transform.childCount >= 15) {
                    return;
                }
                break;
            case "FactoryPlane":
                if(facEnemiesHolder.transform.childCount >= 15) {
                    return;
                }
                break;
        }
        GameObject enemyPrefab = null;
        GameObject enemy = null;
        float random = Random.Range(0, 10);
        Vector3 auxSpawn = new Vector3(0, 0, 0);

        // 0 = zombie, 1 = gladiator, 2 = ranged
        int enemyType = 0;
        switch(currentScene){
            case "ColliseumPlane":
                auxSpawn = colliseumSpawn;
                if(random == 8){
                    enemyPrefab = factoryEnemy;
                    enemyType = 2;
                }
                else if(random == 9){
                    enemyPrefab = forestEnemy;
                    enemyType = 0;
                    auxSpawn = auxSpawn + new Vector3(0, -2f, 0);
                }
                else{
                    enemyType = 1;
                    enemyPrefab = colliseumEnemy;
                }
                enemy = Instantiate(enemyPrefab, auxSpawn, Quaternion.identity);
                enemy.transform.SetParent(colEnemiesHolder.transform);
                if (enemyType == 0){
                    enemy.transform.GetChild(0).GetComponent<EnemyBehaviour>().setType(enemyType);
                }
                break;
            case "ForestPlane":
                auxSpawn = forestSpawn;
                if(random == 8){
                    enemyPrefab = factoryEnemy;
                    enemyType = 2;
                }
                else if(random == 9){
                    enemyPrefab = colliseumEnemy;
                    enemyType = 1;
                }
                else{
                    enemyPrefab = forestEnemy;
                    enemyType = 0;
                    auxSpawn = auxSpawn + new Vector3(0, -2f, 0);
                }
                enemy = Instantiate(enemyPrefab, auxSpawn, Quaternion.identity);
                enemy.transform.SetParent(forEnemiesHolder.transform);
                if (enemyType == 0){
                    enemy.transform.GetChild(0).GetComponent<EnemyBehaviour>().setType(enemyType);
                }
                break;
            case "FactoryPlane":
                auxSpawn = factorySpawn;
                if(random == 8){
                    enemyPrefab = colliseumEnemy;
                    enemyType = 1;
                }
                else if(random == 9){
                    enemyPrefab = forestEnemy;
                    enemyType = 0;
                    auxSpawn = auxSpawn + new Vector3(0, -2f, 0);
                }
                else{
                    enemyPrefab = factoryEnemy;
                    enemyType = 2;
                }
                enemy = Instantiate(enemyPrefab, auxSpawn, Quaternion.identity);
                enemy.transform.SetParent(facEnemiesHolder.transform);
                if (enemyType == 0){
                    enemy.transform.GetChild(0).GetComponent<EnemyBehaviour>().setType(enemyType);
                }
                break;
        }

        EnemyBehaviour enemyBehaviour = enemy.transform.GetChild(0).GetComponent<EnemyBehaviour>();
        RangedEnemyBehaviour rangedEnemyBehaviour = enemy.transform.GetChild(0).GetComponent<RangedEnemyBehaviour>();
        if(enemyBehaviour != null){
            enemyBehaviour.setDropPercentage(50);
        }
        if(rangedEnemyBehaviour != null){
            rangedEnemyBehaviour.setDropPercentage(50);
        }
        spawnEnemyTime = Time.time;
    }

    int getRoundNr()
    {
        return roundNr;
    }

    void ShowRoundStartUI()
    {
        startRoundText = true;
        startRoundTextUI.text = "Round " + roundNr.ToString();
        startRoundTextUI.gameObject.GetComponent<Animator>().Play("RoundStartTextFadeOut", -1, 0f);
    }

}
