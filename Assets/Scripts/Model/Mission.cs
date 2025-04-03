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
        public int rewardAmount;
        public int rewardResourceId;
        public string terrain;
        public string weather;
        public bool isCompleted;

        public List<Enemy> AssignedEnemies;  //enemy info

        public Mission(int id, string name, string description, int difficulty, int rewardMoney, int rewardAmount, int rewardResourceId, string terrain, string weather)
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.difficulty = difficulty;
            this.rewardMoney = rewardMoney;
            this.rewardAmount = rewardAmount;
            this.rewardResourceId = rewardResourceId;
            this.terrain = terrain;
            this.weather = weather;
            this.isCompleted = false;
            this.AssignedEnemies = new List<Enemy>();  // 初始化敌人列表
        }
    }
}