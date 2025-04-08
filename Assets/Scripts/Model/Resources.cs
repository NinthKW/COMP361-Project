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
                { 0, ("Food", 1000, "Food is essential for Soldiers")},
                { 1, ("Money", 1000, "Money is used to purchase and unlock goods")},
                { 2, ("Iron", 1000, "Iron is used for building your base")},
                { 3, ("Wood", 800, "Wood is used for building your base")},
                { 4, ("Titanium", 350, "Titanium is used for building your base")},
                { 5, ("Medecine", 100, "Heal up your wounded soldiers")},
            };
        }

        public Resources(int food, int money, int iron, int wood, int titanium, int medecine)
        {
            resourceType = new Dictionary <int, (string, int, string)>
            {
                { 0, ("Food", food, "Food is essential for Soldiers")},
                { 1, ("Money", money, "Money is used to purchase and unlock goods")},
                { 2, ("Iron", iron, "Iron is used for building your base")},
                { 3, ("Wood", wood, "Wood is used for building your base")},
                { 4, ("Titanium", titanium, "Titanium is used for building your base")},
                { 5, ("Medecine", medecine, "Heal up your wounded soldiers")},
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

        public void UpdateAllResources(Dictionary<int, int> newAmounts)
        {
            foreach (var amount in newAmounts)
            {
                SetAmount(amount.Key, amount.Value);
            }
        }
    }
}