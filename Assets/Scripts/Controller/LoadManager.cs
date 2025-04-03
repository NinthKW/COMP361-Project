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
    public class LoadManager : MonoBehaviour
    {
        public static LoadManager Instance;

        void Awake()
        {

        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Load all that needs to be loaded and deletes itself
        void Update()
        {
            SaveManager.Instance.buildingList = BaseManager.Instance.buildingList;


            Destroy(this);
        }
    }
}