using UnityEngine;

public class ArenaTrigger : MonoBehaviour
{
    public StarterAssets.ThirdPersonController player;

    public WaveSpawner waveSpawner;

    public SceneSwitch sceneSwitch;

    private bool triggerActive;

    public void performAction(bool death = false)
    {
        if(player.currencyCounter != 0){
            string arenaCurrency = sceneSwitch.ArenaName+"Currency";
            int currentCurrency = PlayerPrefs.GetInt(arenaCurrency);
            
            PlayerPrefs.SetInt(arenaCurrency, currentCurrency + player.currencyCounter);
            Debug.Log("Successfully extracted extra " + player.currencyCounter + " " + arenaCurrency);
            Debug.Log("Current " + arenaCurrency + ": " + PlayerPrefs.GetInt(arenaCurrency));
        }
        if(player.colCurrency != 0){
            PlayerPrefs.SetInt("ColliseumCurrency", PlayerPrefs.GetInt("ColliseumCurrency") + player.colCurrency);
            Debug.Log("Successfully extracted extra " + player.colCurrency + " ColliseumCurrency");
        }
        if(player.forCurrency != 0){
            PlayerPrefs.SetInt("ForestCurrency", PlayerPrefs.GetInt("ForestCurrency") + player.colCurrency);
            Debug.Log("Successfully extracted extra " + player.forCurrency + " ForestCurrency");
        }
        if(player.facCurrency != 0){
            PlayerPrefs.SetInt("FactoryCurrency", PlayerPrefs.GetInt("FactoryCurrency") + player.colCurrency);
            Debug.Log("Successfully extracted extra " + player.facCurrency + " FactoryCurrency");
        }
        if (waveSpawner.roundNr != 0){
            string arenaRounds = sceneSwitch.ArenaName+"Rounds";
            int currentRounds = PlayerPrefs.GetInt(arenaRounds);
            PlayerPrefs.SetInt(arenaRounds, Mathf.Max(currentRounds, waveSpawner.roundNr));
        }
        if (waveSpawner.numKills != 0){
            string arenaKills = sceneSwitch.ArenaName+"Kills";
            int currentKills = PlayerPrefs.GetInt(arenaKills);
            PlayerPrefs.SetInt(arenaKills, Mathf.Max(currentKills, waveSpawner.numKills));
        }
        if (waveSpawner.timePlayed != 0f){
            string arenaTime = sceneSwitch.ArenaName+"TimePlayed";
            int currentTimePlayed = PlayerPrefs.GetInt(arenaTime);
            PlayerPrefs.SetInt(arenaTime, currentTimePlayed + ((int)waveSpawner.timePlayed));
            int currentTimePlayedNew = PlayerPrefs.GetInt("ColliseumTimePlayed");
        }

        string arenaAttempts = sceneSwitch.ArenaName+"Attempts";
        PlayerPrefs.SetInt(arenaAttempts, PlayerPrefs.GetInt(arenaAttempts) + 1);

        sceneSwitch.setArenaName("Hub");
        sceneSwitch.LoadArenaScene(death);
    }
 
}
