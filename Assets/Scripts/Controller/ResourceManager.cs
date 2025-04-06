using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Model;
using System.Data;
using Mono.Data.Sqlite;

namespace Assets.Scripts.Controller
{
    public class ResourceManager : MonoBehaviour
    {
        
        public static ResourceManager Instance { get; private set; }
        // Holds the resource data
        private Resources resources;
        // Database path/name
        public string dbName;

        void Awake()
        {
            // Ensure only one instance exists
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); 
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            
            resources = new Resources();
            
            // Database path from StreamingAssets
            if (string.IsNullOrEmpty(dbName))
            {
                dbName = "URI=file:" + Application.streamingAssetsPath + "/database.db";
            }
            
            // Load resources from the database
            LoadResources(dbName);
        }

        // Loads resource data from the database
        public void LoadResources(string dbName)
        {
            using (var connection = new SqliteConnection(dbName))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    // SQL query to get resource id and amount
                    command.CommandText = "SELECT resource_id, current_amount FROM Resource;";
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int resourceId = int.Parse(reader["resource_id"].ToString());
                            int currentAmount = int.Parse(reader["current_amount"].ToString());
                            try
                            {
                                // Update the resource amount
                                resources.SetAmount(resourceId, currentAmount);
                            }
                            catch (System.Exception ex)
                            {
                                Debug.LogWarning("Resource id " + resourceId + " not found: " + ex.Message);
                            }
                        }
                        reader.Close();
                    }
                }
                connection.Close();
            }
        }

        // Returns the current Resources instance
        public Resources GetResources()
        {
            return resources;
        }

        // Returns the name of a resource by id
        public string GetResourceName(int resourceId)
        {
            try
            {
                return resources.GetName(resourceId);
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("Error getting resource name for id " + resourceId + ": " + ex.Message);
                return "";
            }
        }

        // Returns the description of a resource by id
        public string GetResourceDescription(int resourceId)
        {
            try
            {
                return resources.GetDescription(resourceId);
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("Error getting resource description for id " + resourceId + ": " + ex.Message);
                return "";
            }
        }

        // Returns the current amount of a resource by id
        public int GetResourceAmount(int resourceId)
        {
            try
            {
                return resources.GetAmount(resourceId);
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("Error getting resource amount for id " + resourceId + ": " + ex.Message);
                return 0;
            }
        }

        // Updates the amount of a specific resource
        public void UpdateResourceAmount(int resourceId, int newAmount)
        {
            try
            {
                resources.SetAmount(resourceId, newAmount);
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("Error updating resource amount for id " + resourceId + ": " + ex.Message);
            }
        }

        // Updates all resource amounts
        public void UpdateAllResources(Dictionary<int, int> newAmounts)
        {
            resources.UpdateAllResources(newAmounts);
        }
    }
}
