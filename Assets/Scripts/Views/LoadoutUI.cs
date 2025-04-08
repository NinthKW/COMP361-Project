using Assets.Scripts;
using Assets.Scripts.Controller;
using Assets.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadoutUI : MonoBehaviour
{
    public Button backButton;

    public Transform soldierField;
    public Transform weaponField;
    public Transform equipmentField;

    public Transform soldierSelectedField;
    public Transform weaponSelectedField;
    public Transform equipmentSelectedField;

    public Character selectSoldier;
    public Weapon selectWeapon;
    public Equipment selectEquipment;

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

            button.onClick.AddListener(() => { onSoldierButtonClicked(buttonGameObject);  });
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

    void onSoldierButtonClicked(GameObject button)
    {
        try
        {
            Transform previousSoldier = soldierSelectedField.GetComponent<Transform>().GetChild(0);
            previousSoldier.SetParent(soldierField);

            RectTransform previousTransform = previousSoldier.transform.GetComponent<RectTransform>();
            previousTransform.anchorMax = new Vector2(0f, 1f);
            previousTransform.anchorMin = new Vector2(0f, 1f);
            previousTransform.sizeDelta = new Vector2(150f, 150f);
            previousTransform.localScale = Vector3.one;
        }
        catch (System.Exception e)
        {
            Debug.Log("no children");
        }

        button.GetComponent<Transform>().SetParent(soldierSelectedField, false);
        selectSoldier = button.GetComponent<LoadoutButton>().soldier;

        RectTransform rectTransform = button.transform.GetComponent<RectTransform>();
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = new Vector2(75f, 75f);
        rectTransform.localScale = Vector3.one;

        rectTransform.anchoredPosition3D = Vector3.zero;

        if (selectSoldier == null) {
            Debug.LogError("Character obj not in LoadoutButton attribute");
        }
    }


    void OnBackButtonClicked()
    {
        GameManager.Instance.ChangeState(GameState.BasePage);
        GameManager.Instance.LoadGameState(GameState.BasePage);
    }
}
