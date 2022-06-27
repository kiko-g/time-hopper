using UnityEngine;
using UnityEngine.SceneManagement;

public class ArenaTrigger : MonoBehaviour
{
    public StarterAssets.ThirdPersonController player;

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

        sceneSwitch.setArenaName("Hub");
        sceneSwitch.LoadArenaScene(death);
    }
 
}
