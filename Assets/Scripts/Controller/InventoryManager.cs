using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Model;
using System.Data;
using Mono.Data.Sqlite;

namespace Assets.Scripts.Controller
{
    public class InventoryManager : MonoBehaviour
    {
        public static InventoryManager Instance;
        private Inventory playerInventory;

        // Database connection string; can be changed in the Inspector.
        public string dbName;

        void Awake()
        {
            // Make sure only one InventoryManager exists.
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); // Keep this object between scenes.
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            // Set up the player's inventory.
            playerInventory = new Inventory();

            // Use a default database path if none is provided.
            if (string.IsNullOrEmpty(dbName))
            {
                dbName = "URI=file:" + Application.persistentDataPath + "/database.db";
            }

            // Load weapon and equipment data from the database.
            LoadWeapons(dbName);
            LoadEquipments(dbName);
        }

        // Return the player's inventory.
        public Inventory GetInventory()
        {
            return playerInventory;
        }

        #region Database Loading Methods
        // Read weapon data from the database and add each to the inventory.
        public void LoadWeapons(string dbName)
        {
            using (var connection = new SqliteConnection(dbName))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT weapon_id, name, description, damage, cost, resource_amount, resource_type FROM Weapon;";
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = int.Parse(reader["weapon_id"].ToString());
                            string name = reader["name"].ToString();
                            string description = reader["description"].ToString();
                            int damage = int.Parse(reader["damage"].ToString());
                            int cost = int.Parse(reader["cost"].ToString());
                            int resourceAmount = int.Parse(reader["resource_amount"].ToString());
                            int resourceType = int.Parse(reader["resource_type"].ToString());

                            // Make a new weapon and add it to the inventory.
                            Weapon weapon = new Weapon(id, name, description, damage, cost, resourceAmount, resourceType);
                            playerInventory.AddWeapon(weapon);
                        }
                    }
                }
                connection.Close();
            }
        }

        // Read equipment data from the database and add each to the inventory.
        public void LoadEquipments(string dbName)
        {
            using (var connection = new SqliteConnection(dbName))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT equipment_id, name, hp, def, atk, cost, resource_amount, resource_type FROM Equipment;";
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = int.Parse(reader["equipment_id"].ToString());
                            string name = reader["name"].ToString();
                            int hp = int.Parse(reader["hp"].ToString());
                            int def = int.Parse(reader["def"].ToString());
                            int atk = int.Parse(reader["atk"].ToString());
                            int cost = int.Parse(reader["cost"].ToString());
                            int resourceAmount = int.Parse(reader["resource_amount"].ToString());
                            int resourceType = int.Parse(reader["resource_type"].ToString());

                            // Create a new equipment and put it in the inventory.
                            Equipment equipment = new Equipment(id, name, hp, def, atk, cost, resourceAmount, resourceType);
                            playerInventory.AddEquipment(equipment);
                        }
                    }
                }
                connection.Close();
            }
        }
        #endregion

        // Empty the inventory of all weapons and equipment.
        public void ClearInventory()
        {
            playerInventory.Clear();
        }
    }
}
