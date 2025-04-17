using Assets.Scripts;
using Assets.Scripts.Controller;
using Assets.Scripts.Model;
using Codice.Client.Common;
using Codice.Client.Common.FsNodeReaders;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using Unity.Plastic.Newtonsoft.Json;
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

    public TextMeshProUGUI atkDisplayText;
    public TextMeshProUGUI defDisplayText;

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

            button.name = "Soldier:" + soldier.Name;
        }

        //Weapon
        foreach (Weapon weapon in LoadoutManager.Instance.weapons)
        {
            //skip not unlocked weapons
            if (!weapon.isUnlocked)
            {
                continue;
            }

            Debug.Log("Adding weapon: " + weapon.name);
            GameObject buttonGameObject = Instantiate(buttonPrefab, weaponField);
            Button button = buttonGameObject.GetComponent<Button>();

            buttonGameObject.GetComponent<TextMeshProUGUI>().text = weapon.name;
            buttonGameObject.GetComponent<LoadoutButton>().weapon = weapon;

            button.onClick.AddListener(() => {onWeaponButtonClicked(buttonGameObject); });
            weapons.Add(buttonGameObject);

            button.name = "Weapon:" + weapon.name;

            //Disable if equipped 
            foreach(SoldierEquipment se in LoadoutManager.Instance.soldierEquipment)
            {
                if (se.weapon != null)
                {
                    if (se.weapon.name == weapon.name)
                    { 
                        button.interactable = false;
                    }
                }
            }
        }

        //Equipment
        foreach (Equipment equipment in LoadoutManager.Instance.equipments)
        {
            //skip not unlocked weapons
            if (!equipment.isUnlocked)
            {
                continue;
            }

            Debug.Log("Adding equipment: " + equipment.name);
            GameObject buttonGameObject = Instantiate(buttonPrefab, equipmentField);
            Button button = buttonGameObject.GetComponent<Button>();

            buttonGameObject.GetComponent<TextMeshProUGUI>().text = equipment.name;
            buttonGameObject.GetComponent<LoadoutButton>().equipment = equipment;

            button.onClick.AddListener(() => { onEquipmentButtonClicked(buttonGameObject); });
            equipments.Add(buttonGameObject);

            button.name = "Equipment:" + equipment.name;

            //Disable if equipped 
            foreach (SoldierEquipment se in LoadoutManager.Instance.soldierEquipment)
            {
                if (se.equipment != null)
                {
                    if (se.equipment.name ==  equipment.name)
                    {
                        button.interactable = false;
                    }
                }
            }
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

                        //Enable button
                        obj.GetComponent<Button>().interactable = true;

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

                        //Enable button
                        obj.GetComponent<Button>().interactable = true;

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

        int bonusAtk = 0;
        int bonusDef = 0;
        if (selectWeapon != null)
        {
            bonusAtk += selectWeapon.damage;
        }
        if (selectEquipment != null) {
            bonusAtk += selectEquipment.atk;
            bonusDef += selectEquipment.def;
        }

        atkDisplayText.text = "Bonus ATK: " + bonusAtk;
        defDisplayText.text = "Bonus DEF: " + bonusDef;
    }


    void onWeaponButtonClicked(GameObject button)
    {
        //Check if its button in select field is 
        Debug.Log("Clicked " +  button.name);
        bool inSelectField = false;
        if (button.GetComponent<LoadoutButton>().weapon != null && selectWeapon != null)
        {
            if (button.GetComponent<LoadoutButton>().weapon.name == selectWeapon.name)
            { 
                inSelectField = true;
            }
        }

        if (soldierSelectedField.childCount > 0)
        {
            bool hasPrevious = false;

            //Move old button back if exists
            try
            {
                Transform previous = weaponSelectedField.GetChild(0);
                previous.SetParent(weaponField, false);

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
                Debug.Log("no weapon was select previously");
                Debug.Log(e);
            }

            //Remove dmg buff from old
            if (hasPrevious)
            {
                selectSoldier.bonusStat.atk -= selectWeapon.damage;
            }

            //Update current selected weapon
            selectWeapon = null;


            //Run if not the same button
            if (!inSelectField)
            {
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
            //If clicked on same button branch
            else
            {
                //Check if need to remove from SoldierEquipment
                foreach (SoldierEquipment se in LoadoutManager.Instance.soldierEquipment)
                {
                    //Find right soldier loadout
                    if (selectSoldier.Name == se.soldier.Name)
                    {
                        //Remove from list if no wepaon or equipment
                        if (selectWeapon == null && selectEquipment == null)
                        {
                            LoadoutManager.Instance.soldierEquipment.Remove(se);
                        }
                        //Remove weapon from list entry
                        else
                        {
                            se.weapon = null;
                        }
                    }
                }
            }
        } 
        else
        {
            Debug.LogWarning("No soldier selected");
        }

        int bonusAtk = 0;
        int bonusDef = 0;
        if (selectWeapon != null)
        {
            bonusAtk += selectWeapon.damage;
        }
        if (selectEquipment != null)
        {
            bonusAtk += selectEquipment.atk;
            bonusDef += selectEquipment.def;
        }

        atkDisplayText.text = "Bonus ATK: " + bonusAtk;
        defDisplayText.text = "Bonus DEF: " + bonusDef;
    }


    void onEquipmentButtonClicked(GameObject button)
    {
        //Check if its button in select field is 
        Debug.Log("Clicked " + button.name);
        bool inSelectField = false;
        if (button.GetComponent<LoadoutButton>().equipment != null && selectEquipment != null)
        {
            if (button.GetComponent<LoadoutButton>().equipment.name == selectEquipment.name)
            {
                inSelectField = true;
            }
        }

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
                Debug.Log("no equipment was selected previously");
                Debug.Log(e);
            }

            ////Remove atk/def buff from old
            if (hasPrevious)
            {
                selectSoldier.bonusStat.atk -= selectEquipment.atk;
                selectSoldier.bonusStat.def -= selectEquipment.def;
            }

            if (!inSelectField)
            {

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
            } else
            {
                //Check if need to remove from SoldierEquipment
                foreach (SoldierEquipment se in LoadoutManager.Instance.soldierEquipment)
                {
                    //Find right soldier loadout
                    if (selectSoldier.Name == se.soldier.Name)
                    {
                        //Remove from list if no wepaon or equipment
                        if (selectWeapon == null && selectEquipment == null)
                        {
                            LoadoutManager.Instance.soldierEquipment.Remove(se);
                        }
                        //Remove weapon from list entry
                        else
                        {
                            se.equipment = null;
                        }
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("No soldier selected");
        }

        int bonusAtk = 0;
        int bonusDef = 0;
        if (selectWeapon != null)
        {
            bonusAtk += selectWeapon.damage;
        }
        if (selectEquipment != null)
        {
            bonusAtk += selectEquipment.atk;
            bonusDef += selectEquipment.def;
        }

        atkDisplayText.text = "Bonus ATK: " + bonusAtk;
        defDisplayText.text = "Bonus DEF: " + bonusDef;
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

            
            //Only disable if its not soldier
            if (previous.GetComponent<LoadoutButton>().soldier == null)
            { 
                previous.GetComponent<Button>().interactable = false;
            }
            

            hasPrevious = true;
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Move back failed: no children for " + selectedField.name);
            Debug.Log(e);
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
