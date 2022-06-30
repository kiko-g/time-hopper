using UnityEngine;
using UnityEngine.SceneManagement;

public class Footsteps : MonoBehaviour
{

    private int lowerId  = 1;
    private int higherId = 7;
    private string hub_base = "footstep_hub_";
    private string colliseum_base = "footstep_coliseu_";
    private string factory_base = "footstep_factory_1_";
    private string forest_base = "footstep_coliseu_";
    private string rumble_base = "footstep_coliseu_";

    private string footstepsBase;

    // Start is called before the first frame update
    void Start()
    {
        switch(SceneManager.GetActiveScene().name)
        {
            case "Hub":
                footstepsBase = hub_base;
                break;
            case "Colliseum":
                footstepsBase = colliseum_base;
                break;
            case "Factory":
                higherId = 8;
                footstepsBase = factory_base;
                break;
            case "Forest":
                footstepsBase = forest_base;
                break;
            case "Rumble":
                footstepsBase = rumble_base;
                break;
        }
    }

    void Step()
    {
        int id = Random.Range(lowerId, higherId+1);
        FMODUnity.RuntimeManager.PlayOneShot("event:/Project/Character Related/Footstep/Character/" + footstepsBase + id, transform.position);
    }

}
