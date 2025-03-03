using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public GameObject[] levels;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void LoadLevel(int levelIndex)
    {
        Instantiate(levels[levelIndex], Vector3.zero, Quaternion.identity);
        Debug.Log("Level " + levelIndex + " loaded.");
    }
}
