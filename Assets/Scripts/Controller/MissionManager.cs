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

        // 数据库文件名
        private string dbPath;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);

                // 这里是重点：StreamingAssets 路径
                dbPath = "URI=file:" + Application.streamingAssetsPath + "/database.db";
                Debug.Log("Database path: " + dbPath);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            LoadMissionsFromDatabase();
        }

        void LoadMissionsFromDatabase()
        {
            missions.Clear();

            Debug.Log("Trying to open DB at: " + dbPath);

            using (var connection = new SqliteConnection(dbPath))
            {
                connection.Open();
                Debug.Log("Database Opened Successfully!");

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM Mission;";

                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            string missionName = reader.GetString(1);
                            string description = reader.GetString(2);
                            int difficulty = reader.GetInt32(3);
                            int rewardMoney = reader.GetInt32(4);
                            int rewardAmount = reader.GetInt32(5);
                            int rewardResourceId = reader.GetInt32(6);
                            string terrain = reader.GetString(7);
                            string weather = reader.GetString(8);

                            // 创建 Mission 对象
                            Mission mission = new Mission(
                                id,
                                missionName,
                                description,
                                difficulty,
                                rewardMoney,
                                rewardAmount,
                                rewardResourceId,
                                terrain,
                                weather
                            );

                            missions.Add(mission);

                            Debug.Log($"Loaded Mission: {missionName} (Difficulty: {difficulty}, Terrain: {terrain}, Weather: {weather})");
                        }

                        reader.Close();
                    }
                }

                connection.Close();
                Debug.Log($"Total Missions Loaded: {missions.Count}");
            }
        }

        public void StartMission(int missionID)
        {
            Mission selectedMission = missions.Find(m => m.id == missionID);
            if (selectedMission != null)
            {
                Debug.Log("Starting Mission: " + selectedMission.name);
                Invoke(nameof(StartCombat), 2f);
            }
            else
            {
                Debug.LogWarning("Mission not found: " + missionID);
            }
        }

        void StartCombat()
        {
            if (!CombatManager.Instance.gameObject.activeSelf)
            {
                CombatManager.Instance.gameObject.SetActive(true);
            }

            GameManager.Instance.LoadGameState(GameState.MissionPreparationPage);
        }
    }
}
