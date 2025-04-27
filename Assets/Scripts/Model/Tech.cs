using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Model {
    [System.Serializable]
    public class Tech {
        public int techId;
        public string techName;
        public string description;
        public int costMoney;
        public int costResourceId;
        public int costResourceAmount;
        public bool isUnlocked;

        public Tech()
        {
            techId = 0;
        }
        public Tech(int techId, string techName, string description, int costMoney, 
                   int costResourceId, int costResourceAmount, bool unlocked) 
        {
            this.techId = techId;
            this.techName = techName;
            this.description = description;
            this.costMoney = costMoney;
            this.costResourceId = costResourceId;
            this.costResourceAmount = costResourceAmount;
            this.isUnlocked = unlocked;
        }
    }
}