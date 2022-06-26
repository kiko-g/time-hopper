using UnityEngine;
using StarterAssets;
using UnityEngine.InputSystem;

public class HubPause : MonoBehaviour
{
    public Canvas canvas;
    private StarterAssets.ThirdPersonController player;

    void Start()
    {
        canvas = GetComponent<Canvas>();
        canvas.enabled = false;

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<StarterAssets.ThirdPersonController>();
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            canvas.enabled = !canvas.enabled;
            if (canvas.enabled) Hide();
            else Show();
        }
    }

    public void Hide()
    {
        player.SwitchInputToUI();
    }

    public void Show()
    {
        player.SwitchInputToPlayer();
    }
}
