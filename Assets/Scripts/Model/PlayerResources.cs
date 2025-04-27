using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
    public class PlayerResources : MonoBehaviour
    {
        public static PlayerResources Instance { get; private set; }
        
        private Dictionary<int, int> currentResources = new Dictionary<int, int>();
        private float currentMoney = 5000f;
        private Resources resourcesReference; // Keep for name/description lookups
        
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeResources();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeResources()
        {
            resourcesReference = new Resources(); // For getting names/descriptions
            
            // Initialize with default values
            currentResources[0] = 100; // Food
            currentResources[1] = 500; // Wood
            currentResources[2] = 100; // Stone
            currentResources[3] = 100; // Metal
            currentResources[4] = 50;  // Fuel
            currentResources[5] = 50;  // Ammo
            currentResources[6] = 0;   // Medicine
        }

        public float GetMoney() => currentMoney;
        public void DeductMoney(float amount) => currentMoney -= amount;
        public void AddMoney(float amount) => currentMoney += amount;
        
        public int GetResource(int resourceId)
        {
            return currentResources.TryGetValue(resourceId, out int amount) ? amount : 0;
        }

        public void DeductResource(int resourceId, int amount)
        {
            if (currentResources.ContainsKey(resourceId))
            {
                currentResources[resourceId] -= amount;
            }
        }

        public void AddResource(int resourceId, int amount)
        {
            if (currentResources.ContainsKey(resourceId))
            {
                currentResources[resourceId] += amount;
            }
        }

        public string GetResourceName(int resourceId)
        {
            return resourcesReference.GetName(resourceId);
        }
    }
} 