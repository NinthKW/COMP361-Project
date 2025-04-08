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

        //Load potential weapon and equipment buttons
        SoldierEquipment soldierEquipments = findSoldierWithEquipment(button.GetComponent<LoadoutButton>().soldier);
        if (soldierEquipments != null)
        {
            //Move weapon with soldier
            bool wepFound = false;
            foreach (GameObject obj in weapons)
            {
                if (soldierEquipments.weapon != null)
                {
                    if (soldierEquipments.weapon.name == obj.GetComponent<LoadoutButton>().weapon.name)
                    {
                        //attach to select field
                        obj.GetComponent<Transform>().SetParent(weaponSelectedField, false);
                        selectSoldier = button.GetComponent<LoadoutButton>().soldier;

                        //adjust size
                        RectTransform weaponRectTransform = obj.transform.GetComponent<RectTransform>();
                        weaponRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                        weaponRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                        weaponRectTransform.sizeDelta = new Vector2(75f, 75f);
                        weaponRectTransform.localScale = Vector3.one;
                        weaponRectTransform.anchoredPosition3D = Vector3.zero;

                        selectWeapon = obj.GetComponent<LoadoutButton>().weapon;
                        wepFound = true;
                        Debug.Log("Weapon placed");
                    }
                }
            }

            if (!wepFound) { 
                selectWeapon = null;
            }


            //Move equipment with soldier
            bool equipFound = false;
            foreach (GameObject obj in equipments)
            {
                if (soldierEquipments.equipment != null)
                {
                    if (soldierEquipments.equipment.name == obj.GetComponent<LoadoutButton>().equipment.name)
                    {
                        //Attach to selected field
                        obj.GetComponent<Transform>().SetParent(equipmentSelectedField, false);
                        selectSoldier = button.GetComponent<LoadoutButton>().soldier;

                        //Adjust size
                        RectTransform equipmentRectTransform = obj.transform.GetComponent<RectTransform>();
                        equipmentRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                        equipmentRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                        equipmentRectTransform.sizeDelta = new Vector2(75f, 75f);
                        equipmentRectTransform.localScale = Vector3.one;
                        equipmentRectTransform.anchoredPosition3D = Vector3.zero;

                        selectEquipment = obj.GetComponent<LoadoutButton>().equipment;
                        equipFound = true;
                        Debug.Log("Equipment placed");
                    }
                }
            }

            if (!equipFound)
            {
                selectEquipment = null;
            }
        }
        else
        {
            selectWeapon = null;
            selectEquipment = null;
        }
    }


    void onWeaponButtonClicked(GameObject button)
    {
        if (soldierSelectedField.childCount > 0)
        {
            bool hasPrevious = false;

            //Move old button back if exists
            try
            {
                Transform previous = weaponSelectedField.GetChild(0);
                previous.SetParent(weaponField);

                Debug.Log("Previous weapon found");

                RectTransform previousTransform = previous.transform.GetComponent<RectTransform>();
                previousTransform.anchorMax = new Vector2(0f, 1f);
                previousTransform.anchorMin = new Vector2(0f, 1f);
                previousTransform.sizeDelta = new Vector2(150f, 150f);
                previousTransform.localScale = Vector3.one;

                hasPrevious = true;
            }
            catch (System.Exception e)
            {
                Debug.LogError("no weapon was select previously");
            }

            ////Remove dmg buff from old
            if (hasPrevious)
            {
                selectSoldier.bonusStat.atk -= selectWeapon.damage;
            }

            //Move new button in and change current select
            button.GetComponent<Transform>().SetParent(weaponSelectedField, false);
            selectWeapon = button.GetComponent<LoadoutButton>().weapon;

            //Adjust size
            RectTransform weaponRectTransform = button.transform.GetComponent<RectTransform>();
            weaponRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            weaponRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            weaponRectTransform.sizeDelta = new Vector2(75f, 75f);
            weaponRectTransform.localScale = Vector3.one;
            weaponRectTransform.anchoredPosition3D = Vector3.zero;


            //Update dmg buffs on soldier
            selectSoldier.bonusStat.atk += button.GetComponent<LoadoutButton>().weapon.damage;

            //Update SoldierEquipment Obj
            bool found = false;
            foreach (SoldierEquipment se in LoadoutManager.Instance.soldierEquipment)
            {
                if (selectSoldier.Name == se.soldier.Name)
                {
                    se.weapon = selectWeapon;
                    found = true;
                }
            }

            //Add to soldierEquipment list if didn't have before
            if (!found)
            {
                SoldierEquipment newSoldierEquipment = new SoldierEquipment(selectSoldier, selectWeapon, selectEquipment);
                LoadoutManager.Instance.soldierEquipment.Add(newSoldierEquipment);

                Debug.Log("Created new SE for " + selectSoldier.Name);
            }
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
            bool hasPrevious = false;

            //Move old button back if exists
            try
            {
                Transform previous = equipmentSelectedField.GetChild(0);
                previous.SetParent(equipmentField);

                Debug.Log("Previous equipment found");

                RectTransform previousTransform = previous.transform.GetComponent<RectTransform>();
                previousTransform.anchorMax = new Vector2(0f, 1f);
                previousTransform.anchorMin = new Vector2(0f, 1f);
                previousTransform.sizeDelta = new Vector2(150f, 150f);
                previousTransform.localScale = Vector3.one;

                hasPrevious = true;
            }
            catch (System.Exception e)
            {
                Debug.LogError("no equipment was selected previously");
            }

            ////Remove atk/def buff from old
            if (hasPrevious)
            {
                selectSoldier.bonusStat.atk -= selectEquipment.atk;
                selectSoldier.bonusStat.def -= selectEquipment.def;
            }

            //Move new button in and change current select
            button.GetComponent<Transform>().SetParent(equipmentSelectedField, false);
            selectEquipment = button.GetComponent<LoadoutButton>().equipment;

            //Adjust size
            RectTransform equipmentRectTransform = button.transform.GetComponent<RectTransform>();
            equipmentRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            equipmentRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            equipmentRectTransform.sizeDelta = new Vector2(75f, 75f);
            equipmentRectTransform.localScale = Vector3.one;
            equipmentRectTransform.anchoredPosition3D = Vector3.zero;


            //Update dmg buffs on soldier
            selectSoldier.bonusStat.atk += button.GetComponent<LoadoutButton>().equipment.atk;
            selectSoldier.bonusStat.def += button.GetComponent<LoadoutButton>().equipment.def;

            //Update SoldierEquipment Obj
            bool found = false;
            foreach (SoldierEquipment se in LoadoutManager.Instance.soldierEquipment)
            {
                if (selectSoldier.Name == se.soldier.Name)
                {
                    se.equipment = selectEquipment;
                }
            }

            //Add to soldierEquipment list if didn't have before
            if (!found)
            {
                SoldierEquipment newSoldierEquipment = new SoldierEquipment(selectSoldier, selectWeapon, selectEquipment);
                LoadoutManager.Instance.soldierEquipment.Add(newSoldierEquipment);

                Debug.Log("Created new SE for " + selectSoldier.Name);
            }
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
            Debug.Log("Child found" + previous.name);

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
            Debug.LogWarning("Move back failed: no children for " + selectedField.name);
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

        Debug.Log(soldier.Name + "does not have equipment");

        return null;
    }

    void OnBackButtonClicked()
    {
        GameManager.Instance.ChangeState(GameState.BasePage);
        GameManager.Instance.LoadGameState(GameState.BasePage);
    }
}
