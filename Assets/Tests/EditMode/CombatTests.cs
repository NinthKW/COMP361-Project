using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Model;

public class CombatTests
{
    private Combat combat;

    [SetUp]
    public void Setup()
    {
        combat = new Combat();
    }

    [Test]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        Assert.AreEqual(100, combat.playerHealth);
        Assert.AreEqual(50, combat.enemyHealth);
    }

    [Test]
    public void AttackEnemy_ShouldReduceEnemyHealth()
    {
        combat.AttackEnemy(20);
        Assert.AreEqual(30, combat.enemyHealth);
    }

    [Test]
    public void AttackEnemy_ShouldNotGoBelowZero()
    {
        combat.AttackEnemy(100);
        Assert.AreEqual(0, combat.enemyHealth);
    }

    [Test]
    public void ReceiveDamage_ShouldReducePlayerHealth()
    {
        combat.ReceiveDamage(30);
        Assert.AreEqual(70, combat.playerHealth);
    }

    [Test]
    public void ReceiveDamage_ShouldNotGoBelowZero()
    {
        combat.ReceiveDamage(150);
        Assert.AreEqual(0, combat.playerHealth);
    }
} 