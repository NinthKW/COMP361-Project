using Assets.Scripts.Controller;
using Assets.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingFunctionality : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    
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

    public static void hqFunctionality()
    {
        Debug.Log("HQ FUNC CALLED");
    }
}
