using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance;
    public List<Mission> missions = new List<Mission>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        LoadMissions();
    }

    void LoadMissions()
    {
        missions.Add(new Mission(1, "Rescue Scientists", "Save the captured scientists."));
        missions.Add(new Mission(2, "Destroy Alien Base", "Eliminate all alien forces in the area."));
    }

    public void StartMission(int missionID)
    {
        Mission selectedMission = missions.Find(m => m.id == missionID);
        if (selectedMission != null)
        {
            Debug.Log("Starting Mission: " + selectedMission.name);
            GameManager.Instance.StartMission(missionID);
            Invoke("StartCombat", 2f);  // 模拟任务进入战斗
        }
    }

    void StartCombat()
    {
        GameManager.Instance.StartCombat();
    }
}
