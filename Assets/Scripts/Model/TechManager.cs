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
            
            // Here you would add checks for resources/money
            // For example:
            // if (PlayerResources.Money < tech.costMoney) return false;
            // if (PlayerResources.GetResource(tech.costResourceId) < tech.costResourceAmount) return false;
            
            // Deduct resources
            // PlayerResources.DeductMoney(tech.costMoney);
            // PlayerResources.DeductResource(tech.costResourceId, tech.costResourceAmount);
            
            tech.isUnlocked = true;
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