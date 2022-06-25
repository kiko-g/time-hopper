using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HubComputer : MonoBehaviour
{
    public Canvas canvas;

    void Start()
    {
        canvas = GetComponent<Canvas>();
        canvas.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.F))
        {
            canvas.enabled = !canvas.enabled;
        }
    }
}
