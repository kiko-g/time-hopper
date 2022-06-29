using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class BackgroundMusicPlayer : MonoBehaviour
{

    private string musicBase;

    private int currentMusicClip;

    [SerializeField]
    private StarterAssets.ThirdPersonController player;

    private FMOD.Studio.PLAYBACK_STATE playbackState;

    private FMOD.Studio.EventInstance music;

    private bool stopping = false;
    private bool rumble = false;

    private string colliseumBase = "soundtrack_coliseu_music";
    private string factoryBase = "soundtrack_factory_music1";
    private string forestBase = "soundtrack_forest_music";
    private string hubBase = "soundtrack_hub_music1";

    private string currentPlane;

    [SerializeField]
    private RumbleSpawner rumbleSpawner;

    // Start is called before the first frame update
    void Start()
    {
        if (rumbleSpawner == null){
            switch(SceneManager.GetActiveScene().name)
            {
                case "Colliseum":
                    int id = Random.Range(1, 2);
                    musicBase = "soundtrack_coliseu_music" + id;
                    break;
                case "Factory":
                    musicBase = "soundtrack_factory_music1";
                    break;
                case "Forest":
                    int idx = Random.Range(1, 3);
                    musicBase = "soundtrack_neworld_music" + idx;
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
        } else {
            rumble = true;
        }
    }

    void getNewMusic(string plane){
        if (plane == "ColliseumPlane"){
            int id = Random.Range(1, 2);
            musicBase = "soundtrack_coliseu_music" + id;
        } else if (plane == "ForestPlane"){
            int idx = Random.Range(1, 3);
            musicBase = "soundtrack_neworld_music" + idx;
        } else if (plane == "FactoryPlane"){
            musicBase = "soundtrack_factory_music1";
        } else {
            musicBase = "soundtrack_hub_music1";
        }
    }

    public void startNewMusic(string plane){
        music.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        getNewMusic(plane);
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
