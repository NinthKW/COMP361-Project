using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Assets.Scripts.Model;

namespace Assets.Tests.EditMode
{
    public class CharacterTests
    {
        // Define TestCharacter first
        private class TestCharacter : Character
        {
            public TestCharacter(string name, int health, int level) : base(name, health, level) { }

            protected override int CalculateDamage()
            {
                return 10; // Fixed damage for testing
            }

            protected override void HandleDeath() { }
        }

        // Then use it
        private TestCharacter character;

        [SetUp]
        public void Setup()
        {
            character = new TestCharacter("Test Character", 100, 1);
        }

        [Test]
        public void Constructor_ShouldInitializeCorrectly()
        {
            Assert.AreEqual("Test Character", character.Name);
            Assert.AreEqual(100, character.Health);
            Assert.AreEqual(100, character.MaxHealth);
            Assert.AreEqual(1, character.Level);
        }

        [Test]
        public void TakeDamage_ShouldReduceHealth()
        {
            character.TakeDamage(30);
            Assert.AreEqual(70, character.Health);
        }

        [Test]
        public void TakeDamage_ShouldNotGoBelowZero()
        {
            character.TakeDamage(150);
            Assert.AreEqual(0, character.Health);
        }

        [Test]
        public void IsDead_ShouldReturnTrue_WhenHealthIsZero()
        {
            character.TakeDamage(100);
            Assert.IsTrue(character.IsDead());
        }

        [Test]
        public void IsDead_ShouldReturnFalse_WhenHealthAboveZero()
        {
            character.TakeDamage(50);
            Assert.IsFalse(character.IsDead());
        }
    }
} 