using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Model
{
    // Manages a collection of InventoryItem objects (the actual inventory logic)
    [System.Serializable]
    public class Inventory
    {
        private List<InventoryItem> items;

        public Inventory(){
            items = new List<InventoryItem>();
        }

        // Add an item to the inventory
        public void AddItem(InventoryItem newItem) {
            InventoryItem existingItem = items.Find(item => item.itemType == newItem.itemType);
            if (existingItem != null)
            {
                existingItem.AddQuantity(newItem.quantity);
            }
            else
            {
                items.Add(newItem);
            }
        }

         // Remove a certain quantity of an item
        public bool RemoveItem(ItemType itemType, int quantity)
        {
            InventoryItem item = items.Find(i => i.itemType == itemType);
            if (item != null && item.quantity >= quantity)
            {
                item.SubtractQuantity(quantity);
                if (item.quantity == 0)
                {
                    items.Remove(item);
                }
                return true;
            }
            return false;
        }

        // Get an item by its type
        public InventoryItem GetItem(ItemType itemType){
            return items.Find(i => i.itemType == itemType);
        }

        // Get a copy of the current inventory
        public List<InventoryItem> GetAllItems(){
            return new List<InventoryItem>(items);
        }

        // Clear all items from the inventory
        public void Clear()
        {
            items.Clear();
        }
    }

    // Represents a single item stored in the inventory
    [System.Serializable]
    public class InventoryItem
    {
        public ItemType itemType;
        public string itemName;
        public string description;
        public int quantity;

        public InventoryItem(ItemType type, int quantity)
        {
            this.itemType = type;
            this.itemName = type.ToString();
            this.description = GetItemDescription(type);
            this.quantity = quantity;
        }

        // Returns a simple description for each item type
        private string GetItemDescription(ItemType type)
        {
            switch (type)
            {
                case ItemType.Food: return "Used to feed your units.";
                case ItemType.Wood: return "Basic construction material.";
                case ItemType.Stone: return "Used to build strong structures.";
                case ItemType.Metal: return "Necessary for tech development.";
                case ItemType.Fuel: return "Powers vehicles and tools.";
                case ItemType.Ammo: return "Required for weapons.";
                case ItemType.Medicine: return "Used for healing.";
                case ItemType.Weapon: return "Used in combat.";
                case ItemType.Armor: return "Reduces damage taken.";
                case ItemType.Tool: return "Useful for crafting or repairs.";
                default: return "No description available.";
            }
        }

        public void AddQuantity(int amount)
        {
            quantity += amount;
        }

        public void SubtractQuantity(int amount)
        {
            quantity = Mathf.Max(0, quantity - amount);
        }
    }
}