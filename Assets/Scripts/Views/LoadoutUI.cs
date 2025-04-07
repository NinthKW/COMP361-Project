using Assets.Scripts;
using Assets.Scripts.Controller;
using Assets.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadoutUI : MonoBehaviour
{
    public Button backButton;

    public Transform soldierField;
    public Transform weaponField;
    public Transform equipmentField;

    public GameObject buttonPrefab;

    // Start is called before the first frame update
    void Start()
    {
        backButton.GetComponent<Button>().onClick.AddListener(OnBackButtonClicked);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void populateFields()
    {
        //foreach (Character soldier in LoadoutManager.Instance.soldiers)
        //{
        //    Debug.Log("Adding soldier: " + soldier.Name);
        //    GameObject buttonGameObject = Instantiate(soldierPrefab, soldierGrid);
        //    Button button = buttonGameObject.GetComponent<Button>();

        //    buttonGameObject.GetComponent<TextMeshProUGUI>().text = soldier.Name;
        //    buttonGameObject.GetComponent<HospitalSoldier>().soldier = soldier;
        //}

    }
    void OnBackButtonClicked()
    {
        GameManager.Instance.ChangeState(GameState.BasePage);
        GameManager.Instance.LoadGameState(GameState.BasePage);
    }
}
