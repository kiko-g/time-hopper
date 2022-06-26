using UnityEngine;

public class HubComputer : MonoBehaviour
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
        if (WasPressed() && WithinRange(player.transform.position))
        {
            Toggle(!canvas.enabled);
        }
    }

    bool WasPressed()
    {
        return Input.GetKeyUp(KeyCode.F);
    }

    bool WithinRange(Vector3 pos)
    {
        int xMax = -10, xMin = -16, zMax = -5, zMin = -9;
        return pos.x < xMax && pos.x > xMin && pos.z < zMax && pos.z > zMin;
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
        if (canvas.enabled) Hide();
        else Show();
    }
}
