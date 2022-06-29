using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundMusicPlayer : MonoBehaviour
{

    private string musicBase;
    private int lowBound = 1;
    private int highBound = 6;

    private int currentMusicClip;

    private FMOD.Studio.PLAYBACK_STATE playbackState;

    //private FMOD.Studio.EventInstance music;

    // Start is called before the first frame update
    void Start()
    {
        currentMusicClip = lowBound;
        switch(SceneManager.GetActiveScene().name)
        {
            case "Colliseum":
                musicBase = "soundtrack_coliseu_music1_";
                highBound = 2;
                break;
            case "Factory":
                musicBase = "soundtrack_factory_music1_";
                highBound = 6;
                break;
            case "Forest":
                musicBase = "soundtrack_newworld_music1_";
                highBound = 6;
                break;
            case "Hub":
                musicBase = "soundtrack_hub_music1_";
                highBound = 6;
                break;
            default:
                musicBase = "soundtrack_hub_music1_";
                highBound = 6;
                break;
        }

        //music = FMODUnity.RuntimeManager.CreateInstance("event:/Project/Soundtrack/" + musicBase + currentMusicClip);
        //music.start();
        //music.release();
        //FMODUnity.RuntimeManager.PlayOneShot("event:/Project/Soundtrack/" + musicBase + currentMusicClip);
        Debug.Log("Started Playing Music: " + "event:/Project/Soundtrack/" + musicBase + currentMusicClip);

    }

    // Update is called once per frame
    void Update()
    {
        //music.getPlaybackState(out playbackState);
        Debug.Log("Playback State: " + playbackState);
    }
}
