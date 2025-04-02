using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Model;


namespace Assets.Scripts.Controller
{
    public class InventoryManager : MonoBehaviour
    {
        // Instance for global access
        public static InventoryManager Instance;

        // Internal reference to the Inventory model
        private Inventory playerInventory;

        // Called once when the object is loaded
        void Awake()
        {
            // Only one InventoryManager should exist
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); // Keep across scenes
            }
            else
            {
                Destroy(gameObject); // Destroy duplicate
                return;
            }

            // Initialize the inventory
            playerInventory = new Inventory();
        }

        // Method to add an item by type and quantity
        public void AddItem(ItemType type, int quantity)
        {
            InventoryItem newItem = new InventoryItem(type, quantity);
            playerInventory.AddItem(newItem);
        }

        // Method to remove a certain quantity of an item
        public bool RemoveItem(ItemType type, int quantity)
        {
            return playerInventory.RemoveItem(type, quantity);
        }

        // Retrieve the full inventory
        public Inventory GetInventory()
        {
            return playerInventory;
        }

        // Retrieve a specific item from the inventory
        public InventoryItem GetItem(ItemType type)
        {
            return playerInventory.GetItem(type);
        }

        // Clear all items from the inventory
        public void ClearInventory()
        {
            playerInventory.Clear();
        }
    }
}

