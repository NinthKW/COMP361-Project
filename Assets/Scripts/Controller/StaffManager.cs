using Assets.Scripts.Model;
using Assets.Scripts.Controller;
using System.Collections.Generic;
using UnityEngine;

public class StaffManager : MonoBehaviour
{
    public static StaffManager Instance;

    // This list will hold the loaded soldier data.
    // You can use this list in your UI to populate the grid.
    public List<Character> soldiers = new List<Character>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Persist this manager between scenes if needed.
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    
    public void Load()
    {
       
        soldiers = GameManager.Instance.currentGame.soldiersData;

   
        Debug.Log($"StaffManager loaded {soldiers.Count} soldiers.");
    }

}