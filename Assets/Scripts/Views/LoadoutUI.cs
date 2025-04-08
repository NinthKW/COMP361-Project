using Assets.Scripts;
using Assets.Scripts.Controller;
using Assets.Scripts.Model;
using Codice.Client.Common;
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

    public List<GameObject> soldiers = new List<GameObject>();
    public List<GameObject> weapons = new List<GameObject>();
    public List<GameObject> equipments = new List<GameObject>();

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

            button.onClick.AddListener(() => { onSoldierButtonClicked(buttonGameObject); });
            soldiers.Add(buttonGameObject);
        }

        //Weapon
        foreach (Weapon weapon in LoadoutManager.Instance.weapons)
        {
            Debug.Log("Adding weapon: " + weapon.name);
            GameObject buttonGameObject = Instantiate(buttonPrefab, weaponField);
            Button button = buttonGameObject.GetComponent<Button>();

            buttonGameObject.GetComponent<TextMeshProUGUI>().text = weapon.name;
            buttonGameObject.GetComponent<LoadoutButton>().weapon = weapon;

            button.onClick.AddListener(() => {onWeaponButtonClicked(buttonGameObject); });
            weapons.Add(buttonGameObject);
        }

        //Equipment
        foreach (Equipment equipment in LoadoutManager.Instance.equipments)
        {
            Debug.Log("Adding soldier: " + equipment.name);
            GameObject buttonGameObject = Instantiate(buttonPrefab, equipmentField);
            Button button = buttonGameObject.GetComponent<Button>();

            buttonGameObject.GetComponent<TextMeshProUGUI>().text = equipment.name;
            buttonGameObject.GetComponent<LoadoutButton>().equipment = equipment;

            button.onClick.AddListener(() => { onEquipmentButtonClicked(buttonGameObject); });
            equipments.Add(buttonGameObject);
        }
    }

    void onSoldierButtonClicked(GameObject button)
    {
        //Reset buttons
        bool hasPrevious = moveSelectedBackToGrid(soldierSelectedField, soldierField);
        if (hasPrevious)
        {
            moveSelectedBackToGrid(weaponSelectedField, weaponField);
            moveSelectedBackToGrid(equipmentSelectedField, equipmentField);
        }

        //Move selected soldier button
        button.GetComponent<Transform>().SetParent(soldierSelectedField, false);
        selectSoldier = button.GetComponent<LoadoutButton>().soldier;

        RectTransform rectTransform = button.transform.GetComponent<RectTransform>();
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = new Vector2(75f, 75f);
        rectTransform.localScale = Vector3.one;
        rectTransform.anchoredPosition3D = Vector3.zero;

        SoldierEquipment soldierEquipments = findSoldierWithEquipment(button.GetComponent<LoadoutButton>().soldier);
        if (soldierEquipments != null)
        {
            //Move weapon with soldier
            foreach (GameObject obj in weapons)
            {
                if (!(soldierEquipments.weapon.name == "dummy"))
                {
                    if (soldierEquipments.weapon.name == obj.GetComponent<LoadoutButton>().weapon.name)
                    {
                        obj.GetComponent<Transform>().SetParent(weaponSelectedField, false);
                        selectSoldier = button.GetComponent<LoadoutButton>().soldier;

                        RectTransform weaponRectTransform = obj.transform.GetComponent<RectTransform>();
                        weaponRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                        weaponRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                        weaponRectTransform.sizeDelta = new Vector2(75f, 75f);
                        weaponRectTransform.localScale = Vector3.one;
                        weaponRectTransform.anchoredPosition3D = Vector3.zero;

                        weaponSelectedField = obj.GetComponent<Transform>();
                    }
                }
            }

            //Move equipment with soldier
            foreach (GameObject obj in equipments)
            {
                if (!(soldierEquipments.equipment.name == "dummy"))
                {
                    if (soldierEquipments.equipment.name == obj.GetComponent<LoadoutButton>().equipment.name)
                    {
                        obj.GetComponent<Transform>().SetParent(equipmentSelectedField, false);
                        selectSoldier = button.GetComponent<LoadoutButton>().soldier;

                        RectTransform equipmentRectTransform = obj.transform.GetComponent<RectTransform>();
                        equipmentRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                        equipmentRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                        equipmentRectTransform.sizeDelta = new Vector2(75f, 75f);
                        equipmentRectTransform.localScale = Vector3.one;
                        equipmentRectTransform.anchoredPosition3D = Vector3.zero;

                        equipmentSelectedField = obj.GetComponent<Transform>();
                    }
                }
            }
        }
    }


    void onWeaponButtonClicked(GameObject button)
    {
        if (soldierSelectedField.childCount > 0)
        {
            bool hasPrevious = false;
            Weapon previousWeapon;

            //Move old button back if exists
            try
            {
                Transform previous = weaponSelectedField.GetChild(0);
                previous.SetParent(weaponField);

                RectTransform previousTransform = previous.transform.GetComponent<RectTransform>();
                previousTransform.anchorMax = new Vector2(0f, 1f);
                previousTransform.anchorMin = new Vector2(0f, 1f);
                previousTransform.sizeDelta = new Vector2(150f, 150f);
                previousTransform.localScale = Vector3.one;

                hasPrevious = true;
            }
            catch (System.Exception e)
            {
                Debug.Log("no children");
            }

            ////Remove dmg buff from old
            //if (hasPrevious)
            //{
            //    previousWeapon = weaponSelectedField.GetComponent<LoadoutButton>().weapon;
            //    soldierSelectedField.GetComponent<LoadoutButton>().soldier.bonusStat.atk -= previousWeapon.damage;
            //}

            ////Move new button in
            //button.GetComponent<Transform>().SetParent(weaponSelectedField, false);

            //RectTransform weaponRectTransform = button.transform.GetComponent<RectTransform>();
            //weaponRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            //weaponRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            //weaponRectTransform.sizeDelta = new Vector2(75f, 75f);
            //weaponRectTransform.localScale = Vector3.one;
            //weaponRectTransform.anchoredPosition3D = Vector3.zero;

            //weaponSelectedField = button.GetComponent<Transform>();

            ////Update dmg buffs on soldier
            //Character currentSoldier = soldierSelectedField.GetComponent<LoadoutButton>().soldier;
            //currentSoldier.bonusStat.atk += button.GetComponent<LoadoutButton>().weapon.damage;

            ////Update SoldierEquipment Obj
            //bool found = false;
            //foreach (SoldierEquipment se in LoadoutManager.Instance.soldierEquipment)
            //{
            //    if (currentSoldier.Name == se.soldier.Name)
            //    {
            //        se.weapon = weaponSelectedField.GetComponent<LoadoutButton>().weapon;
            //    }
            //}
            
            //if (!found)
            //{
               
            //}
        } 
        else
        {
            Debug.LogWarning("No soldier selected");
        } 
    }


    void onEquipmentButtonClicked(GameObject button)
    {
        if (soldierSelectedField.childCount > 0)
        {

        }
        else
        {
            Debug.LogWarning("No soldier selected");
        }
    }

    public bool moveSelectedBackToGrid(Transform selectedField, Transform previousGrid) {
        bool hasPrevious = false;
        try
        {
            Transform previous = selectedField.GetChild(0);
            previous.SetParent(previousGrid);

            RectTransform previousTransform = previous.transform.GetComponent<RectTransform>();
            previousTransform.anchorMax = new Vector2(0f, 1f);
            previousTransform.anchorMin = new Vector2(0f, 1f);
            previousTransform.sizeDelta = new Vector2(150f, 150f);
            previousTransform.localScale = Vector3.one;

            hasPrevious = true;
        }
        catch (System.Exception e)
        {
            Debug.Log("no children");
        }

        return hasPrevious;
    }


    public SoldierEquipment findSoldierWithEquipment(Character soldier)
    {
        foreach(SoldierEquipment bonus in LoadoutManager.Instance.soldierEquipment)
        {
            if (bonus.soldier.Name == soldier.Name)
            {
                return bonus;
            }
        }

        return null;
    }

    void OnBackButtonClicked()
    {
        GameManager.Instance.ChangeState(GameState.BasePage);
        GameManager.Instance.LoadGameState(GameState.BasePage);
    }
}
