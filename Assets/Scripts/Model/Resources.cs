using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.Scripts.Model
{
    [System.Serializable]
    public class Resources
    {
        private ResourcesType resourcesType;
        private int amount;
        private static readonly string[] validResourceNames = new string[] { "Food", "Wood", "Stone", "Metal", "Fuel", "Ammo", "Medicine" };
        
        public class ResourcesType {
            private int id;
            private string name;
            private string description;
            private int level;
        
            public ResourcesType (int id, string name, string description, int level)
            {
                this.id = id;
                if (System.Array.IndexOf(Resources.validResourceNames, name) == -1)
                {
                    throw new System.ArgumentException("Invalid resource name: " + name);
                }
                this.name = name;
                this.description = description;
                this.level = level;
            }
        }

        public Resources (ResourcesType resourcesType, int amount)
        {
            this.resourcesType = resourcesType;
            this.amount = amount;
        }

        public ResourcesType GetResourcesType()
        {
            return resourcesType;
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


