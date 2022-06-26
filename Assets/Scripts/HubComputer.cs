using UnityEngine;

public class HubComputer : MonoBehaviour
{
    public Canvas canvas;
    public GameObject hint;
    private bool withinRange;
    private StarterAssets.ThirdPersonController player;

    void Start()
    {
        canvas = GetComponent<Canvas>();
        canvas.enabled = false;

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<StarterAssets.ThirdPersonController>();
    }

    void Update()
    {
        UpdateWithinRange(player.transform.position);
        if (withinRange)
        {
            if (!hint.activeInHierarchy) hint.SetActive(true);
            if (WasPressed()) Toggle(!canvas.enabled);
        }
        else
        {
            if (hint.activeInHierarchy) hint.SetActive(false);
        }
    }

    bool WasPressed()
    {
        return Input.GetKeyUp(KeyCode.F);
    }

    void UpdateWithinRange(Vector3 pos)
    {
        int xMax = -10, xMin = -16, zMax = -5, zMin = -9;
        withinRange = (pos.x < xMax && pos.x > xMin && pos.z < zMax && pos.z > zMin);
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
