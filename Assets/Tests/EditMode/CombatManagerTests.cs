using NUnit.Framework;
using UnityEngine;
using Assets.Scripts.Model;
using Assets.Scripts.Controller;
using System.Collections.Generic;

namespace Assets.Tests.EditMode
{
    public class CombatManagerTests
    {
        private CombatManager combatManager;
        private GameObject gameObject;

        [SetUp]
        public void Setup()
        {
            gameObject = new GameObject();
            combatManager = gameObject.AddComponent<CombatManager>();
        }

        [TearDown]
        public void Teardown()
        {
            Object.DestroyImmediate(gameObject);
        }

        [Test]
        public void StartCombat_ShouldInitializeCorrectly()
        {
            var soldiers = new List<Soldier> { new Soldier(new Role(RoleType.Army)) };
            var enemies = new List<Enemy> { new Enemy("Test Enemy", 100, 10, 1, 50) };
            combatManager.StartCombat(soldiers, enemies);
            Assert.IsTrue(combatManager.IsCombatActive);
            Assert.IsTrue(combatManager.IsPlayerTurn);
        }

        [Test]
        public void ProcessAttack_ShouldDamageTarget()
        {
            var soldier = new Soldier(new Role(RoleType.Army));
            var enemy = new Enemy("Test Enemy", 100, 10, 1, 50);
            var soldiers = new List<Soldier> { soldier };
            var enemies = new List<Enemy> { enemy };
            combatManager.StartCombat(soldiers, enemies);
            combatManager.ProcessAttack(soldier, enemy);
            Assert.Less(enemy.Health, enemy.MaxHealth);
        }

        [Test]
        public void ProcessAttack_ShouldEndCombat_WhenAllEnemiesDefeated()
        {
            var soldier = new Soldier(new Role(RoleType.Army));
            var enemy = new Enemy("Test Enemy", 1, 10, 1, 50); // Enemy with 1 HP
            var soldiers = new List<Soldier> { soldier };
            var enemies = new List<Enemy> { enemy };
            combatManager.StartCombat(soldiers, enemies);
            combatManager.ProcessAttack(soldier, enemy);
            Assert.IsFalse(combatManager.IsCombatActive);
        }
    }
}