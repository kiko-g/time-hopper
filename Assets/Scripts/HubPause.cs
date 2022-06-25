using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HubPause : MonoBehaviour
{
    public Canvas canvas;

    void Start()
    {
        canvas = GetComponent<Canvas>();
        canvas.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            canvas.enabled = !canvas.enabled;
        }
    }
}
