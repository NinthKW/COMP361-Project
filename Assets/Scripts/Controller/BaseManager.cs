using Assets.Scripts.Model;
using Mono.Data.Sqlite;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;

namespace Assets.Scripts.Controller
{
    public class BaseManager : MonoBehaviour
    {
        public static BaseManager Instance;
        public List<Base> buildingList = new List<Base>();

        private string dbPath;
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);

                dbPath = "URI=file:" + Application.streamingAssetsPath + "/database.db";
                Debug.Log("Database path: " + dbPath);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        // Start is called before the first frame update
        void Start()
        {
            LoadBase();
        }

        // Update is called once per frame
        void Update()
        {

        }

        void LoadBase()
        {
            buildingList.Clear();

            Debug.Log("Trying to open DB at: " + dbPath);

            using (var connection = new SqliteConnection(dbPath))
            {
                connection.Open();
                Debug.Log("Database Opened Successfully! For base loadout");

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM Infrastructure;";

                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            string buildingName = reader.GetString(1);
                            string description = reader.GetString(2);
                            int level = reader.GetInt32(3);
                            int cost = reader.GetInt32(4);
                            int resourceAmount = reader.GetInt32(5);
                            int resourceType = reader.GetInt32(6);


                            Base building = new Base(
                                id,
                                buildingName,
                                description,
                                level,
                                cost,
                                resourceAmount,
                                resourceType
                            ); 

                            buildingList.Add(building);

                            Debug.Log($"Loaded Mission: {buildingName}");
                        }

                        reader.Close();
                    }
                }

                connection.Close();
                Debug.Log($"Total buildingList Loaded: {buildingList.Count}");
            }
        }
    }

}
