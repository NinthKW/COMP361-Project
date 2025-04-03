using NUnit.Framework;
using Assets.Scripts.Model;
using System;

namespace Assets.Tests.EditMode
{
    public class RoleTests
    {
        private Role armyRole;
        private Role sniperRole;

        [SetUp]
        public void Setup()
        {
            armyRole = new Role(RoleType.Army);
            sniperRole = new Role(RoleType.Sniper);
        }

        [Test]
        public void Constructor_Army_ShouldInitializeCorrectly()
        {
            Assert.AreEqual(RoleType.Army, armyRole.roleType);
            Assert.AreEqual(100, armyRole.maxHealth);
            Assert.AreEqual(10, armyRole.maxLevel);
            Assert.AreEqual(10, armyRole.base_attack);
            Assert.AreEqual(8, armyRole.base_defense);
        }

        [Test]
        public void Constructor_Sniper_ShouldInitializeCorrectly()
        {
            Assert.AreEqual(RoleType.Sniper, sniperRole.roleType);
            Assert.AreEqual(70, sniperRole.maxHealth);
            Assert.AreEqual(10, sniperRole.maxLevel);
            Assert.AreEqual(15, sniperRole.base_attack);
            Assert.AreEqual(5, sniperRole.base_defense);
        }

        [Test]
        public void Constructor_ShouldInitializeDefaultValues()
        {
            Assert.AreEqual(0, armyRole.exp);
            Assert.AreEqual(1, armyRole.level);
        }

        [Test]
        public void Constructor_WithInvalidRole_ShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() => new Role((RoleType)999));
        }

        [Test]
        public void RoleType_ShouldBeAssignedCorrectly()
        {
            var engineerRole = new Role(RoleType.Engineer);
            Assert.AreEqual(RoleType.Engineer, engineerRole.roleType);
        }

        [Test]
        public void DefaultExpAndLevel_ShouldBeZeroAndOne()
        {
            var medicRole = new Role(RoleType.Medic);
            Assert.AreEqual(0, medicRole.exp);
            Assert.AreEqual(1, medicRole.level);
        }

        [Test]
        public void Constructor_Medic_ShouldInitializeMaxHealthCorrectly()
        {
            var medicRole = new Role(RoleType.Medic);
            Assert.AreEqual(75, medicRole.maxHealth);
        }
    }
} 