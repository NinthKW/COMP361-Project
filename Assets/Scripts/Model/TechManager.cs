using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Model
{
    public class TechManager : MonoBehaviour
    {
        public static TechManager Instance { get; private set; }
        
        [SerializeField]
        private List<Tech> availableTechs = new List<Tech>();
        
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
            if (PlayerResources.Instance == null)
            {
                Debug.LogError("PlayerResources not found!");
                return false;
            }
            
            if (PlayerResources.Instance.GetMoney() < tech.costMoney)
            {
                Debug.Log($"Not enough money! Need {tech.costMoney}, have {PlayerResources.Instance.GetMoney()}");
                return false;
            }
            
            if (PlayerResources.Instance.GetResource(tech.costResourceId) < tech.costResourceAmount)
            {
                string resourceName = new Resources().GetName(tech.costResourceId);
                Debug.Log($"Not enough {resourceName}! Need {tech.costResourceAmount}, have {PlayerResources.Instance.GetResource(tech.costResourceId)}");
                return false;
            }
            
            // If we have enough resources, deduct them and unlock
            PlayerResources.Instance.DeductMoney(tech.costMoney);
            PlayerResources.Instance.DeductResource(tech.costResourceId, tech.costResourceAmount);
            
            tech.isUnlocked = true;

            // Add debug logs to show remaining resources after purchase
            Debug.Log($"Successfully unlocked {tech.techName}!");
            Debug.Log($"Remaining money: ${PlayerResources.Instance.GetMoney()}");
            Debug.Log($"Remaining {PlayerResources.Instance.GetResourceName(tech.costResourceId)}: {PlayerResources.Instance.GetResource(tech.costResourceId)}");
            
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
    }
} 