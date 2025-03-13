using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Model
{
    [System.Serializable]
    public class Mission
    {
        public int id;
        public string name;
        public string description;
        public int difficulty;
        public int rewardMoney;
        public int rewardResourceId;
        public int rewardTechId;
        public int terrainId;
        public int weatherId;
        public bool isCompleted;

        public Mission(int id, string name, string description, int difficulty, int rewardMoney,
                       int rewardResourceId, int rewardTechId, int terrainId, int weatherId)
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.difficulty = difficulty;
            this.rewardMoney = rewardMoney;
            this.rewardResourceId = rewardResourceId;
            this.rewardTechId = rewardTechId;
            this.terrainId = terrainId;
            this.weatherId = weatherId;
            this.isCompleted = false;
        }
    }
}