using UnityEngine;

public class Trigger : MonoBehaviour
{
    public StarterAssets.ThirdPersonController player;

    private bool triggerActive;

    public SceneSwitch sceneSwitch;

    public string ArenaName;

    public void performAction(bool death = false)
    {
        sceneSwitch.setArenaName(ArenaName);
        sceneSwitch.LoadArenaScene(death);
    }

}
