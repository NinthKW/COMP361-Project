using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Model;
using Assets.Scripts.Controller;


public class MissionSelectUI : MonoBehaviour
{
    public Button mission1Button;
    public Button mission2Button;

    void Start()
    {
        mission1Button.onClick.AddListener(() => MissionManager.Instance.StartMission(1));
        mission2Button.onClick.AddListener(() => MissionManager.Instance.StartMission(2));
    }
}