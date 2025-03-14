using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.Scripts.Model
{
    [System.Serializable]
    public class Resource
    {
        private ResourceType resourceType;
        private int amount;
        private static readonly string[] validResourceNames = new string[] { "Food", "Wood", "Stone", "Metal", "Fuel", "Ammo", "Medicine" };
        
        public class ResourceType {
            private int id;
            private string name;
            private string description;
            private int level;
        
            public ResourceType (int id, string name, string description, int level)
            {
                this.id = id;
                if (System.Array.IndexOf(Resource.validResourceNames, name) == -1)
                {
                    throw new System.ArgumentException("Invalid resource name: " + name);
                }
                this.name = name;
                this.description = description;
                this.level = level;
            }
        }

        public Resource (ResourceType resourceType, int amount)
        {
            this.resourceType = resourceType;
            this.amount = amount;
        }

        public ResourceType GetResourceType()
        {
            return resourceType;
        }

        public int GetAmount()
        {
            return amount;
        }

        public void SetAmount(int amount)
        {
            this.amount = amount;
        }

        public void AddAmount(int amount)
        {
            this.amount += amount;
        }

        public void SubtractAmount(int amount)
        {
            this.amount -= amount;
        }
    }
}


