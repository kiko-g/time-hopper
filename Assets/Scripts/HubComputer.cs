using UnityEngine;

public class HubComputer : MonoBehaviour
{
    public Canvas canvas;
    public GameObject hint;
    private StarterAssets.ThirdPersonController player;

    void Start()
    {
        canvas = GetComponent<Canvas>();
        canvas.enabled = false;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<StarterAssets.ThirdPersonController>();
    }

    void Update()
    {
        if (IsWithinRange(player.transform.position))
        {
            if (!hint.activeInHierarchy) hint.SetActive(true);
            if (WasPressed()) Toggle(!canvas.enabled);
        }
        else
        {
            //Debug.Log(player.transform.position);
            if (hint.activeInHierarchy) hint.SetActive(false);
        }
    }

    public void Toggle(bool value)
    {
        canvas.enabled = value;
        if (canvas.enabled) player.SwitchInputToUI();
        else player.SwitchInputToPlayer();
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
}
