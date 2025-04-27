using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Controller
{
    public class ResourceGenerationManager : MonoBehaviour
    {
        public static ResourceGenerationManager Instance;
        public List<ResourceGenerationBuilding> Buildings;
        private float timer = 0f;
        [SerializeField] private float generationInterval = 60.0f;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                Buildings = new List<ResourceGenerationBuilding>();
                populate();

                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // Update is called once per frame
        void Update()
        {
            timer += Time.deltaTime;
            if (timer >= generationInterval)
            {
                timer = 0f;
                
                foreach (ResourceGenerationBuilding building in Buildings)
                {
                    if (building.active && building.resourceID != -1)
                    {
                        int cur = GameManager.Instance.currentGame.resourcesData.GetAmount(building.resourceID);
                        GameManager.Instance.currentGame.resourcesData.SetAmount(building.resourceID, cur + building.increaseAmount);
                    }
                }
            }
        }

        //Add all resource generation amounts to list
        void populate()
        {
            Buildings.Add(new ResourceGenerationBuilding(0, 10));
            Buildings.Add(new ResourceGenerationBuilding(1, 20));
            Buildings.Add(new ResourceGenerationBuilding(2, 30));
            Buildings.Add(new ResourceGenerationBuilding(3, 40));
            Buildings.Add(new ResourceGenerationBuilding(4, 50));
            Buildings.Add(new ResourceGenerationBuilding(5, 60));
        }

    }
}
