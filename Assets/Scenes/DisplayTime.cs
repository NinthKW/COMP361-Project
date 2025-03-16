using UnityEngine;
using TMPro;
using System;

public class DisplayTime : MonoBehaviour
{
    public TMP_Text timeText;

    void Update()
    {
        // Display system time in HH:mm:ss format
        timeText.text = DateTime.Now.ToString("HH:mm:ss");
    }
}
