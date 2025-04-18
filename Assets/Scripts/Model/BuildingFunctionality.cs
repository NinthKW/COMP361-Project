using Assets.Scripts;
using Assets.Scripts.Controller;
using Assets.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingFunctionality
{   
    public static void hospitalFunctionality()
    {
        Debug.Log("HOSPITAL FUNC CALLED");
        GameManager.Instance.LoadGameState(GameState.HospitalPage);
    }

    public static void trainingFunctionality()
    {
        Debug.Log("TRAINING FUNC CALLED");
        GameManager.Instance.LoadGameState(GameState.TrainingPage);
    }

    public static void loadoutFunctionality()
    {
        Debug.Log("TRAINING FUNC CALLED");
        GameManager.Instance.LoadGameState(GameState.LoadoutPage);
    }

    public static void hqFunctionality(GameObject button)
    {
        Debug.Log("HQ FUNC CALLED");

        Debug.Log("Current money: " + GameManager.Instance.currentGame.resourcesData.GetAmount(0));
        Debug.Log("Current money: " + GameManager.Instance.currentGame.resourcesData.GetAmount(1));
        Debug.Log("Current money: " + GameManager.Instance.currentGame.resourcesData.GetAmount(2));
        Debug.Log("Current money: " + GameManager.Instance.currentGame.resourcesData.GetAmount(3));
        Debug.Log("Current money: " + GameManager.Instance.currentGame.resourcesData.GetAmount(4));
        Debug.Log("Current money: " + GameManager.Instance.currentGame.resourcesData.GetAmount(5));
    }
}
