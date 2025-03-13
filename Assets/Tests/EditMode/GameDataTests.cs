using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Model;

public class GameDataTests
{
    private GameData gameData;

    [SetUp]
    public void Setup()
    {
        gameData = new GameData();
    }

    [Test]
    public void Constructor_ShouldInitializeWithNullValues()
    {
        Assert.IsNull(gameData.staffData);
        Assert.IsNull(gameData.resourcesData);
        Assert.IsNull(gameData.missionData);
        Assert.IsNull(gameData.techData);
        Assert.IsNull(gameData.baseData);
    }

    [Test]
    public void StaffData_ShouldBeSettableAndGettable()
    {
        Staff staff = new Staff(new List<Staff>());
        gameData.staffData = staff;
        Assert.AreEqual(staff, gameData.staffData);
    }

    [Test]
    public void ResourcesData_ShouldBeSettableAndGettable()
    {
        Model.Resources.ResourcesType resourceType = new Model.Resources.ResourcesType(1, "Food", "Test food", 1);
        Model.Resources resources = new Model.Resources(resourceType, 100);
        gameData.resourcesData = resources;
        Assert.AreEqual(resources, gameData.resourcesData);
    }

    [Test]
    public void MissionData_ShouldBeSettableAndGettable()
    {
        Mission mission = new Mission(1, "Test Mission", "Test Description");
        gameData.missionData = mission;
        Assert.AreEqual(mission, gameData.missionData);
    }

    [Test]
    public void TechData_ShouldBeSettableAndGettable()
    {
        Tech tech = new Tech();
        gameData.techData = tech;
        Assert.AreEqual(tech, gameData.techData);
    }

    [Test]
    public void BaseData_ShouldBeSettableAndGettable()
    {
        Base baseData = new Base();
        gameData.baseData = baseData;
        Assert.AreEqual(baseData, gameData.baseData);
    }
} 