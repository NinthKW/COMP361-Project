using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    public Button startButton;

    void Start()
    {
        startButton.onClick.AddListener(() => GameManager.Instance.LoadMissionSelect());
    }
}