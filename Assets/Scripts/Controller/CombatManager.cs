using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Model;
namespace Assets.Scripts.Controller 
{
    public class CombatManager : MonoBehaviour
    {
        public static CombatManager Instance;
        public Combat combatData;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            // TODO: Player select soldiers

            // TODO: Select enemies based on Player select Mission

            List<Soldier> soldiers = new();
            List<Enemy> enemies = new();

            soldiers.Add(new Soldier(new Role(RoleType.Snipper)));
            soldiers.Add(new Soldier(new Role(RoleType.Medic)));
            soldiers.Add(new Soldier(new Role(RoleType.Army)));
            soldiers.Add(new Soldier(new Role(RoleType.Engineer)));
            //soldiers.Add(new Soldier(new Role(RoleType.Scott)));

            enemies.Add(new Enemy("Goblin", 10, 2, 1));
            enemies.Add(new Enemy("Orc", 20, 5, 2));
            enemies.Add(new Enemy("Dragon", 50, 10, 3));

            combatData = new Combat(enemies, soldiers);
        }

        public bool StartCombat()
        {
            if (combatData != null)
            {
                Debug.Log("Starting Combat");
                return combatData.CombatStart();
            }
            throw new System.Exception("Combat data is null");
        }
    }
}