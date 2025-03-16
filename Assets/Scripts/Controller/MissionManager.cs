using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Model;
using System.Data;
using Mono.Data.Sqlite;

namespace Assets.Scripts.Controller
{
    public class MissionManager : MonoBehaviour
    {
        public static MissionManager Instance;
        public List<Mission> missions = new List<Mission>();

        private string dbPath;

        void Awake()
        {
            Debug.Log("MissionManager Awake: " + gameObject.scene.name);

            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);

                dbPath = "URI=file:" + Application.streamingAssetsPath + "/database.db";
                Debug.Log("Database path: " + dbPath);
            }
            else
            {
                Debug.LogWarning("MissionManager duplicate detected, destroying this one");
                Destroy(gameObject);
            }
        }

        void Start()
        {
            Debug.Log("MissionManager Start in scene: " + gameObject.scene.name);
            Init();
        }

        private void Init()
        {
            Debug.Log("Loading Missions...");
            if (missions == null || missions.Count == 0)
            {
                LoadMissions();
            }
        }

        void LoadMissions()
        {
            missions.Clear();

            using (var connection = new SqliteConnection(dbPath))
            {
                try
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "SELECT * FROM Mission;";

                        using (IDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int id = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                string description = reader.GetString(2);
                                int difficulty = reader.GetInt32(3);
                                int rewardMoney = reader.GetInt32(4);
                                int rewardResourceId = reader.GetInt32(5);
                                int rewardTechId = reader.GetInt32(6);
                                int terrainId = reader.GetInt32(7);
                                int weatherId = reader.GetInt32(8);

                                Mission newMission = new Mission(
                                    id,
                                    name,
                                    description,
                                    difficulty,
                                    rewardMoney,
                                    rewardResourceId,
                                    rewardTechId,
                                    terrainId,
                                    weatherId
                                );

                                missions.Add(newMission);
                                Debug.Log($"Loaded Mission: {name}");
                            }

                            reader.Close();
                        }
                    }

                    connection.Close();
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"Failed to load missions from DB: {ex.Message}");
                }
            }

            Debug.Log("Missions Loaded from database: " + missions.Count);
        }

        public void StartMission(int missionID)
        {
            Mission selectedMission = missions.Find(m => m.id == missionID);
            if (selectedMission != null)
            {
                Debug.Log("Starting Mission: " + selectedMission.name);
                Invoke(nameof(StartCombat), 2f);
            }
        }

        void StartCombat()
        {
            if (!CombatManager.Instance.gameObject.activeSelf)
            {
                CombatManager.Instance.gameObject.SetActive(true);
            }

            GameManager.Instance.LoadGameState(GameState.CombatPage);
        }
    }
}