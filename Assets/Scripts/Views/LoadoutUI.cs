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
        populateFields();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void populateFields()
    {
        //Soldier
        foreach (Character soldier in LoadoutManager.Instance.soldiers)
        {
            Debug.Log("Adding soldier: " + soldier.Name);
            GameObject buttonGameObject = Instantiate(buttonPrefab, soldierField);
            Button button = buttonGameObject.GetComponent<Button>();

            buttonGameObject.GetComponent<TextMeshProUGUI>().text = soldier.Name;
            buttonGameObject.GetComponent<LoadoutButton>().soldier = soldier;
        }

        //Weapon
        foreach (Weapon weapon in LoadoutManager.Instance.weapons)
        {
            Debug.Log("Adding weapon: " + weapon.name);
            GameObject buttonGameObject = Instantiate(buttonPrefab, weaponField);
            Button button = buttonGameObject.GetComponent<Button>();

            buttonGameObject.GetComponent<TextMeshProUGUI>().text = weapon.name;
            buttonGameObject.GetComponent<LoadoutButton>().weapon = weapon;
        }

        //Equipment
        foreach (Equipment equipment in LoadoutManager.Instance.equipments)
        {
            Debug.Log("Adding soldier: " + equipment.name);
            GameObject buttonGameObject = Instantiate(buttonPrefab, equipmentField);
            Button button = buttonGameObject.GetComponent<Button>();

            buttonGameObject.GetComponent<TextMeshProUGUI>().text = equipment.name;
            buttonGameObject.GetComponent<LoadoutButton>().equipment = equipment;
        }

    }
    void OnBackButtonClicked()
    {
        GameManager.Instance.ChangeState(GameState.BasePage);
        GameManager.Instance.LoadGameState(GameState.BasePage);
    }
}
