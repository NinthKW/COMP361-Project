using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Model;

public class ResourcesTests
{
    private Model.Resources.ResourcesType resourceType;
    private Model.Resources resources;

    [SetUp]
    public void Setup()
    {
        resourceType = new Model.Resources.ResourcesType(1, "Food", "Basic food supply", 1);
        resources = new Model.Resources(resourceType, 100);
    }

    [Test]
    public void Constructor_ShouldInitializeWithGivenValues()
    {
        Assert.AreEqual(resourceType, resources.GetResourcesType());
        Assert.AreEqual(100, resources.GetAmount());
    }

    [Test]
    public void AddAmount_ShouldIncreaseAmount()
    {
        resources.AddAmount(50);
        Assert.AreEqual(150, resources.GetAmount());
    }

    [Test]
    public void SubtractAmount_ShouldDecreaseAmount()
    {
        resources.SubtractAmount(30);
        Assert.AreEqual(70, resources.GetAmount());
    }

    [Test]
    public void SetAmount_ShouldUpdateAmount()
    {
        resources.SetAmount(200);
        Assert.AreEqual(200, resources.GetAmount());
    }

    [Test]
    public void Constructor_ShouldThrowException_WhenResourceNameInvalid()
    {
        Assert.Throws<System.ArgumentException>(() => 
            new Model.Resources.ResourcesType(1, "InvalidResource", "Invalid", 1));
    }
} 