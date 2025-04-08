using Assets.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class LoadoutButton : MonoBehaviour
    {
        public static Model.Character dummyS = new Model.Soldier("dummy", new Role("recon"), 1, 1, 1, 1, 1, 1, new EquipmentBonus(0, 0));
        public static Weapon dummyW = new Weapon(1, "dummy", "dummy", 1, 1, 1, 1);
        public static Equipment dummyE = new Equipment(1, "dummy", 1, 1, 1, 1, 1, 1);

        public Model.Character soldier;
        public Weapon weapon;
        public Equipment equipment;

        public bool isEquipped;
        

        void Awake()
        {
            soldier = dummyS;
            weapon = dummyW;
            equipment = dummyE;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}