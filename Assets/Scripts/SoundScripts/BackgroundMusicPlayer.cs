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

    private FMOD.Studio.EventInstance music1;

    private FMOD.Studio.EventInstance music2;

    private FMOD.Studio.Bus masterBus;

    private int currentMusic = 0;

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
        masterBus = FMODUnity.RuntimeManager.GetBus("bus:/");
        if (rumbleSpawner == null){
            switch(SceneManager.GetActiveScene().name)
            {
                case "Colliseum":
                    int id = Random.Range(1, 3);
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
            }

            music = FMODUnity.RuntimeManager.CreateInstance("event:/Project/Soundtrack/" + musicBase);
            music.start();
            music.release();
        } else {
            rumble = true;
        }
        if (rumble){
            int id = Random.Range(1, 3);
            musicBase = "soundtrack_coliseu_music" + id;
            music = FMODUnity.RuntimeManager.CreateInstance("event:/Project/Soundtrack/" + musicBase);
            music.setVolume(0.0f);
            music.start();
            music.release();
            musicBase = "soundtrack_factory_music1";
            music1 = FMODUnity.RuntimeManager.CreateInstance("event:/Project/Soundtrack/" + musicBase);
            music1.setVolume(0.0f);
            music1.start();
            music1.release();
            int idx = Random.Range(1, 3);
            musicBase = "soundtrack_neworld_music" + idx;
            music2 = FMODUnity.RuntimeManager.CreateInstance("event:/Project/Soundtrack/" + musicBase);
            music2.setVolume(0.0f);
            music2.start();
            music2.release();
        }
    }

    void stopCurrentMusic(){
        if (currentMusic == 0){
            music.setVolume(0.0f);
        } else if (currentMusic  == 1){
            music1.setVolume(0.0f);
        } else {
            music2.setVolume(0.0f);
        }
    }

    void startCurrentMusic(string plane){
        stopCurrentMusic();
        float volume = PlayerPrefs.GetFloat("sfxVolume");
        if (plane == "ColliseumPlane"){
            currentMusic = 0;
            music.setVolume(volume);
        } else if (plane == "FactoryPlane"){
            currentMusic = 1;
            music1.setVolume(volume);
        } else if (plane == "ForestPlane"){
            currentMusic = 2;
            music2.setVolume(volume);
        }

        music.getVolume(out volume);
        music1.getVolume(out volume);
        music2.getVolume(out volume);
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
        startCurrentMusic(plane);
    }

    public void startBossIntro(){
        music.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        music = FMODUnity.RuntimeManager.CreateInstance("event:/Project/Soundtrack/soundtrack_boss_introduction");
        music.start();
        music.release();
    }

    public void startBossMusic(){
        music.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        music = FMODUnity.RuntimeManager.CreateInstance("event:/Project/Soundtrack/soundtrack_boss_music1");
        music.start();
        music.release();
    }

    public void stopBossMusic(){
        music.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
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

    public void StopMusic(){
        masterBus.stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    void OnDestroy(){
        music.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }
}
