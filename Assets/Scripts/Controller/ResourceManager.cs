using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Model;
using System.Data;
using Mono.Data.Sqlite;

namespace Assets.Scripts.Controller
{
    public class ResourceManager : MonoBehaviour
    {
        
        public static ResourceManager Instance { get; private set; }
        // Holds the resource data
        private Model.Resources resources;

        void Awake()
        {
            // Ensure only one instance exists
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); 
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            
            resources = Game.Instance.resourcesData;
        }

        // Returns the current Resources instance
        public Model.Resources GetResources()
        {
            return resources;
        }

        // Returns the name of a resource by id
        public string GetResourceName(int resourceId)
        {
            try
            {
                return resources.GetName(resourceId);
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("Error getting resource name for id " + resourceId + ": " + ex.Message);
                return "";
            }
        }

        // Returns the description of a resource by id
        public string GetResourceDescription(int resourceId)
        {
            try
            {
                return resources.GetDescription(resourceId);
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("Error getting resource description for id " + resourceId + ": " + ex.Message);
                return "";
            }
        }

        // Returns the current amount of a resource by id
        public int GetResourceAmount(int resourceId)
        {
            try
            {
                return resources.GetAmount(resourceId);
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("Error getting resource amount for id " + resourceId + ": " + ex.Message);
                return 0;
            }
        }

        // Updates the amount of a specific resource
        public void UpdateResourceAmount(int resourceId, int newAmount)
        {
            try
            {
                resources.SetAmount(resourceId, newAmount);
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("Error updating resource amount for id " + resourceId + ": " + ex.Message);
            }
        }

        // Updates all resource amounts
        public void UpdateAllResources(Dictionary<int, int> newAmounts)
        {
            resources.UpdateAllResources(newAmounts);
        }
    }
}
