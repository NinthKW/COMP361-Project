using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace Assets.Scripts.Model
{
    [System.Serializable]
    public class Resources 
    {
        Dictionary <int, (string name, int amount, string description)> resourceType;

        public Resources()
        {
            resourceType = new Dictionary <int, (string, int, string)>
            {
                { 0, ("Food", 100, "Food is essential for Soldiers")},
                { 1, ("Wood", 100, "Wood is used for building your base")},
                { 2, ("Stone", 100, "Stone is used for building your base")},
                { 3, ("Metal", 100, "Metal is used for building your base and ammo")},
                { 4, ("Fuel", 50, "Fuel is used for missions")},
                { 5, ("Ammo", 50, "Ammo is used for combat.")},
                { 6, ("Medicine", 0, "Medicine is used to heal soldiers")}
            };
        }

        public Resources(int food, int wood, int stone, int metal, int fuel, int ammo, int medecine)
        {
            resourceType = new Dictionary <int, (string, int, string)>
            {
                { 0, ("Food", food, "Food is essential for Soldiers")},
                { 1, ("Wood", wood, "Wood is used for building your base")},
                { 2, ("Stone", stone, "Stone is used for building your base")},
                { 3, ("Metal", metal, "Metal is used for building your base and ammo")},
                { 4, ("Fuel", fuel, "Fuel is used for missions")},
                { 5, ("Ammo", ammo, "Ammo is used for combat.")},
                { 6, ("Medicine", medecine, "Medicine is used to heal soldiers")}
            };
        }

        public int GetAmount (int id)
        {
            if (resourceType.TryGetValue(id , out var resource))
            {
                return resource.amount;
            }
            throw new ArgumentException("Invalid resource id: " + id);
        }
 
        public string GetDescription (int id)
        {
            if (resourceType.TryGetValue(id , out var resource))
            {
                return resource.description;
            }
            throw new ArgumentException("Invalid resource id: " + id);
        }
 
        public string GetName (int id)
        {
            if (resourceType.TryGetValue(id , out var resource))
            {
                return resource.name;
            }
            throw new ArgumentException("Invalid resource id: " + id);
        }

        public void SetAmount (int id, int newAmount)
        {
            if (resourceType.TryGetValue(id, out var resource))
            {
                resourceType[id] = (resource.name, newAmount, resource.description);
            }
            else
            {
                throw new ArgumentException("Invalid resource id: " + id);
            }
        }
    }
}