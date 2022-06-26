using UnityEngine;

public class HubComputer : MonoBehaviour
{
    public Canvas canvas;
    public GameObject hint;
    private Canvas pauseCanvas;
    private StarterAssets.ThirdPersonController player;

    void Start()
    {
        canvas = GetComponent<Canvas>();
        canvas.enabled = false;
        hint.SetActive(false);

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<StarterAssets.ThirdPersonController>();
        pauseCanvas = GameObject.FindGameObjectWithTag("PauseHUD").GetComponent<Canvas>();
    }

    void Update()
    {
        if (IsWithinRange(player.transform.position))
        {
            if (canvas.enabled) HideHint();
            else ShowHint();
            if (WasPressed()) Toggle(!canvas.enabled);
        }
        else
        {
            HideHint();
        }

        if (pauseCanvas.enabled) HideAll();
    }

    public void Toggle(bool value)
    {
        canvas.enabled = value;
        if (canvas.enabled)
        {
            player.SwitchInputToUI();
            HideHint();
        }
        else
        {
            player.SwitchInputToPlayer();
            ShowHint();
        }
    }

    bool WasPressed()
    {
        return Input.GetKeyUp(KeyCode.F);
    }

    bool IsWithinRange(Vector3 pos)
    {
        float xMaxLeft = -10.0f, xMinLeft = -16.0f, zMaxLeft = -6.0f, zMinLeft = -9.0f;
        float xMaxRight = -0.5f, xMinRight = -6.0f, zMaxRight = -6.0f, zMinRight = -9.0f;

        bool left = (pos.x < xMaxLeft && pos.x > xMinLeft && pos.z < zMaxLeft && pos.z > zMinLeft);
        bool right = (pos.x < xMaxRight && pos.x > xMinRight && pos.z < zMaxRight && pos.z > zMinRight);

        return left || right;
    }

    void HideHint() 
    {
        if (hint.activeInHierarchy) hint.SetActive(false);
    }

    void ShowHint() 
    {
        if (!hint.activeInHierarchy) hint.SetActive(true);
    }

    void HideAll()
    {
        canvas.enabled = false;
        player.SwitchInputToUI();
        HideHint();
    }
}
