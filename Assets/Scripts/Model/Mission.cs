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
        public bool unlocked;

        // Terrain Effects
        public int terrainAtkEffect;
        public int terrainDefEffect;
        public int terrainHpEffect;

        // Weather Effects
        public int weatherAtkEffect;
        public int weatherDefEffect;
        public int weatherHpEffect;

        public List<Enemy> AssignedEnemies;  //enemy info

        public Mission(int id, string name, string description, int difficulty, int rewardMoney, int rewardAmount, int rewardResourceId, string terrain, string weather, bool isCompleted)
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
            this.isCompleted = isCompleted;
            this.AssignedEnemies = new List<Enemy>();
        }

        public void SetTerrainEffects(int atkEffect, int defEffect, int hpEffect)
        {
            this.terrainAtkEffect = atkEffect;
            this.terrainDefEffect = defEffect;
            this.terrainHpEffect = hpEffect;
        }

        public void SetWeatherEffects(int atkEffect, int defEffect, int hpEffect)
        {
            this.weatherAtkEffect = atkEffect;
            this.weatherDefEffect = defEffect;
            this.weatherHpEffect = hpEffect;
        }

    }
}