using Assets.Scripts.Controller;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Model
{
    public class TechManager : MonoBehaviour
    {
        public static TechManager Instance { get; private set; }
        
        public List<Tech> availableTechs;
        public Resources availableResource;
        public List<Base> buildingList;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public bool UnlockTech(int techId)
        {
            Tech tech = availableTechs.Find(t => t.techId == techId);
            if (tech == null) return false;
            
            // Check if already unlocked
            if (tech.isUnlocked) return false;
            
            // Check if player has enough resources
            if (availableResource == null)
            {
                Debug.LogError("TechManager not found!");
                return false;
            }
            
            //Check if enough money
            if (availableResource.GetAmount(1) < tech.costMoney)
            {
                Debug.Log($"Not enough money! Need {tech.costMoney}, have {availableResource.GetAmount(1)}");
                return false;
            }
            
            //Check if enough needed resource
            if (availableResource.GetAmount(tech.costResourceId) < tech.costResourceAmount)
            {
                string resourceName = availableResource.GetName(tech.costResourceId);
                Debug.Log($"Not enough {resourceName}! Need {tech.costResourceAmount}, have {availableResource.GetAmount(tech.costResourceId)}");
                return false;
            }
            
            // If we have enough resources, deduct them and unlock
            availableResource.SetAmount(1, availableResource.GetAmount(1) - tech.costMoney);
            availableResource.SetAmount(tech.costResourceId, availableResource.GetAmount(tech.costResourceId) - tech.costResourceAmount);
            
            tech.isUnlocked = true;
            Base unlockedBuilding = buildingList.Find(t => t.name == tech.techName);
            unlockedBuilding.unlocked = true;

            // Add debug logs to show remaining resources after purchase
            Debug.Log($"Successfully unlocked {tech.techName}!");
            Debug.Log($"Remaining money: ${TechManager.Instance.availableResource.GetAmount(1)}");
            Debug.Log($"Remaining {TechManager.Instance.availableResource.GetAmount(tech.costResourceId)}: {TechManager.Instance.availableResource.GetAmount(tech.costResourceId)}");
            
            return true;
        }

        public bool IsTechUnlocked(int techId)
        {
            Tech tech = availableTechs.Find(t => t.techId == techId);
            return tech != null && tech.isUnlocked;
        }

        public List<Tech> GetAllTechs()
        {
            return availableTechs;
        }

        public void LoadTech()
        {
            availableTechs = GameManager.Instance.currentGame.techData;
            availableResource = GameManager.Instance.currentGame.resourcesData;
            buildingList = GameManager.Instance.currentGame.basesData;
        }
    }
} 