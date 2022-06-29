using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundMusicPlayer : MonoBehaviour
{

    private string musicBase;

    private int currentMusicClip;

    [SerializeField]
    private StarterAssets.ThirdPersonController player;

    private FMOD.Studio.PLAYBACK_STATE playbackState;

    private FMOD.Studio.EventInstance music;

    private bool stopping = false;

    // Start is called before the first frame update
    void Start()
    {
        switch(SceneManager.GetActiveScene().name)
        {
            case "Colliseum":
                musicBase = "soundtrack_coliseu_music1";
                break;
            case "Factory":
                musicBase = "soundtrack_factory_music1";
                break;
            case "Forest":
                musicBase = "soundtrack_neworld_music1";
                break;
            case "Hub":
                musicBase = "soundtrack_hub_music1";
                break;
            default:
                musicBase = "soundtrack_hub_music1";
                break;
        }

        music = FMODUnity.RuntimeManager.CreateInstance("event:/Project/Soundtrack/" + musicBase);
        music.start();
        music.release();
    }

    // Update is called once per frame
    void Update()
    {
        if (player.changingScene && !stopping){
            stopping = true;
            music.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }
}
