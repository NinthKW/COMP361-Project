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
        // The player's inventory holding weapons and equipment
        public Inventory playerInventory;

        // Database connection string
        public string dbName;

        void Awake()
        {
            // Check if an instance already exists
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); 
            }
            else
            {
                Destroy(gameObject); // Remove duplicate instance
                return;
            }
        }

        // Returns the player's inventory
        public Inventory GetInventory()
        {
            return playerInventory;
        }

        // Shows the list of loaded weapons
        public List<Weapon> GetWeapons()
        {
            return playerInventory.GetWeapons();
        }

        // Shows the list of loaded equipment
        public List<Equipment> GetEquipments()
        {
            return playerInventory.GetEquipments();
        }

        //Links inventory to game model
        public void loadInventory()
        {
            playerInventory = GameManager.Instance.currentGame.inventory;
        }
  

        // Clears the inventory of all weapons and equipment
        public void ClearInventory()
        {
            playerInventory.Clear();
        }
    }
}
