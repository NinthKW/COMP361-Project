using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Model
{
    // Inventory holds lists of weapons and equipment
    [System.Serializable]
    public class Inventory
    {
        public List<Weapon> weapons; // List of weapon items
        public List<Equipment> equipments; // List of equipment items

        // Constructor: Initialize the lists
        public Inventory()
        {
            weapons = new List<Weapon>(); // Create weapons list
            equipments = new List<Equipment>(); // Create equipments list
        }

        #region Weapon Methods
        // Add a weapon if it's not already in the list
        public void AddWeapon(Weapon weapon)
        {
            // Avoid duplicate entries
            if (!weapons.Exists(w => w.weapon_id == weapon.weapon_id))
            {
                weapons.Add(weapon);
            }
        }

        // Return a copy of the weapons list
        public List<Weapon> GetWeapons()
        {
            return new List<Weapon>(weapons);
        }

        // Remove a weapon by its id
        public void RemoveWeapon(int weaponId)
        {
            Weapon weapon = weapons.Find(w => w.weapon_id == weaponId);
            if (weapon != null)
            {
                weapons.Remove(weapon);
            }
        }
        #endregion

        #region Equipment Methods
        // Add equipment if it's not already in the list
        public void AddEquipment(Equipment equipment)
        {
            if (!equipments.Exists(e => e.equipment_id == equipment.equipment_id))
            {
                equipments.Add(equipment);
            }
        }

        // Return a copy of the equipments list
        public List<Equipment> GetEquipments()
        {
            return new List<Equipment>(equipments);
        }

        // Remove equipment by its id
        public void RemoveEquipment(int equipmentId)
        {
            Equipment equipment = equipments.Find(e => e.equipment_id == equipmentId);
            if (equipment != null)
            {
                equipments.Remove(equipment);
            }
        }
        #endregion

        // Clear all weapons and equipment from the inventory
        public void Clear()
        {
            weapons.Clear();
            equipments.Clear();
        }
    }

    // Class to store weapon information
    [System.Serializable]
    public class Weapon
    {
        public int weapon_id;
        public string name;
        public string description;
        public int damage;
        public int cost;
        public int resource_amount;
        public int resource_type;
        public bool isUnlocked;

        // Constructor for a weapon
        public Weapon(int weapon_id, string name, string description, int damage, int cost, int resource_amount, int resource_type, bool isUnlocked)
        {
            this.weapon_id = weapon_id;
            this.name = name;
            this.description = description;
            this.damage = damage;
            this.cost = cost;
            this.resource_amount = resource_amount;
            this.resource_type = resource_type;
            this.isUnlocked = isUnlocked;
        }
    }

    // Class to store equipment information
    [System.Serializable]
    public class Equipment
    {
        public int equipment_id;
        public string name;
        public int hp;
        public int def;
        public int atk;
        public int cost;
        public int resource_amount;
        public int resource_type;
        public bool isUnlocked;

        // Constructor for equipment
        public Equipment(int equipment_id, string name, int hp, int def, int atk, int cost, int resource_amount, int resource_type, bool isUnlocked)
        {
            this.equipment_id = equipment_id;
            this.name = name;
            this.hp = hp;
            this.def = def;
            this.atk = atk;
            this.cost = cost;
            this.resource_amount = resource_amount;
            this.resource_type = resource_type;
            this.isUnlocked = isUnlocked;
        }
    }
}
