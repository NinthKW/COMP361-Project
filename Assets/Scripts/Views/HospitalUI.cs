using Assets.Scripts;
using Assets.Scripts.Controller;
using Assets.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class HospitalUI : MonoBehaviour
{
    public static HospitalUI Instance;
    public Transform soldierGrid;
    public GameObject soldierPrefab;
    public Character currentSelectedSoldier;

    public GameObject backButton;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        populateSoldierGrid();
        backButton.GetComponent<Button>().onClick.AddListener(OnBackButtonClicked);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void populateSoldierGrid()
    {
        foreach (Character soldier in GameManager.Instance.currentGame.soldiersData)
        {
            Debug.Log("Adding soldier: " + soldier.Name);
            GameObject buttonGameObject = Instantiate(soldierPrefab, soldierGrid);
            Button button = buttonGameObject.GetComponent<Button>();    

            buttonGameObject.GetComponent<TextMeshProUGUI>().text = soldier.Name;
            buttonGameObject.GetComponent<HospitalSoldier>().soldier = soldier;
        }
    }

    void OnBackButtonClicked()
    {
        GameManager.Instance.ChangeState(GameState.BasePage);
        GameManager.Instance.LoadGameState(GameState.BasePage);
    }
}
