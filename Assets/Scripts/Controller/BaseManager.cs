using Assets.Scripts.Model;
using Mono.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using UnityEngine;

namespace Assets.Scripts.Controller
{
    public class BaseManager : MonoBehaviour
    {
        public static BaseManager Instance;
        public List<Base> buildingList;

        void Awake()
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

        // Start is called before the first frame update
        void Start()
        {
 
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void LoadBase()
        {
            buildingList = Game.Instance.basesData;

            HospitalManager.Instance.soldiers = Game.Instance.soldiersData;
            TrainingManager.Instance.soldiers = Game.Instance.soldiersData;
        }
    }

}