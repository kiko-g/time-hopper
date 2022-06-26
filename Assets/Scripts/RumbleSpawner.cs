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
    private GameObject enterTooltipUI;

    private float startTime;
    private bool startRoundText;

    private string currentScene;

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
    private int roundNr = 0;
    private float roundTime = 0f, spawnEnemyTime = 0f;
    //Vector2 xLimits = new Vector2(-30, 30);
    //Vector2 zLimits = new Vector2(-30, 30);

    private bool startRoundFlag = false;
    private bool spawn_in_progress = false;

    private Vector3 colliseumSpawn;
    private Vector3 forestSpawn;
    private Vector3 factorySpawn;

    // Start is called before the first frame update
    void Start()
    {
        /*colEnemiesAliveText = colEnemiesAliveUI.GetComponent<Text>();
        facEnemiesAliveText = facEnemiesAliveUI.GetComponent<Text>();
        forEnemiesAliveText = forEnemiesAliveUI.GetComponent<Text>();*/
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
        currentScene = sceneName;
    }

    public void increaseEnemiesKilled(){
        enemiesKilled++;
        enemiesLeft = totalEnemiesToDefeat - enemiesKilled;
        enemiesLeftText.text = enemiesLeft.ToString();
        if(enemiesLeft == 0){
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
        }
    }

    public bool setStartRoundFlag(bool flag){
        if(startRoundFlag == false && flag == true){
            startRoundFlag = flag;
            //Debug.Log("startRoundFlag set to true");
            return true;
        }
        else{
            startRoundFlag = flag;
            return false;
        }
    }

    public void StartRound()
    {
        enterTooltipUI.SetActive(false);
        startTime = Time.time;
        //Debug.Log("Start round!");
        startRoundFlag = false;
        roundNr++;
        ShowRoundStartUI();
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
        switch(currentScene){
            case "ColliseumPlane":
                auxSpawn = colliseumSpawn;
                if(random == 8){
                    enemyPrefab = factoryEnemy;
                }
                else if(random == 9){
                    enemyPrefab = forestEnemy;
                    auxSpawn = auxSpawn + new Vector3(0, -2f, 0);
                }
                else{
                    enemyPrefab = colliseumEnemy;
                }
                enemy = Instantiate(enemyPrefab, auxSpawn, Quaternion.identity);
                enemy.transform.SetParent(colEnemiesHolder.transform);
                break;
            case "ForestPlane":
                auxSpawn = forestSpawn;
                if(random == 8){
                    enemyPrefab = factoryEnemy;
                }
                else if(random == 9){
                    enemyPrefab = colliseumEnemy;
                }
                else{
                    enemyPrefab = forestEnemy;
                    auxSpawn = auxSpawn + new Vector3(0, -2f, 0);
                }
                enemy = Instantiate(enemyPrefab, auxSpawn, Quaternion.identity);
                enemy.transform.SetParent(forEnemiesHolder.transform);
                break;
            case "FactoryPlane":
                auxSpawn = factorySpawn;
                if(random == 8){
                    enemyPrefab = colliseumEnemy;
                }
                else if(random == 9){
                    enemyPrefab = forestEnemy;
                    auxSpawn = auxSpawn + new Vector3(0, -2f, 0);
                }
                else{
                    enemyPrefab = factoryEnemy;
                }
                enemy = Instantiate(enemyPrefab, auxSpawn, Quaternion.identity);
                enemy.transform.SetParent(facEnemiesHolder.transform);
                break;
        }

        EnemyBehaviour enemyBehaviour = enemy.GetComponent<EnemyBehaviour>();
        RangedEnemyBehaviour rangedEnemyBehaviour = enemy.GetComponent<RangedEnemyBehaviour>();
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
