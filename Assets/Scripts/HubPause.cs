using UnityEngine;

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
            Toggle(!canvas.enabled);
        }
    }

    void Hide()
    {
        player.SwitchInputToUI();
    }

    void Show()
    {
        player.SwitchInputToPlayer();
    }

    public void Toggle(bool value)
    {
        canvas.enabled = value;
        if (value) Hide();
        else Show();
    }
}
