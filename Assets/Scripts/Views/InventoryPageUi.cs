using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Model;
using Assets.Scripts.Controller;
using TMPro;

public class InventoryPageUI : MonoBehaviour
{
    // Back button to return to the Main Menu
    public Button backButton;
    
    // These are the containers for the two panels (for weapons and equipments)
    public Transform weaponsContainer;
    public Transform equipmentsContainer;
    
    // Prefab references for headers and list items
    public GameObject headerPrefab;    // A prefab that displays header text
    public GameObject listItemPrefab;  // A prefab that displays a list inventory item
    
    // Header texts for each section
    public string weaponsHeaderText = "Weapons";
    public string equipmentsHeaderText = "Equipments";

    void Start()
    {
        backButton.onClick.AddListener(OnBackButtonClicked);

        // Inventory items
        PopulateInventory();
    }


    void OnBackButtonClicked()
    {
        GameManager.Instance.ChangeState(GameState.MainMenuPage);
        GameManager.Instance.LoadGameState(GameState.MainMenuPage);
    }

    void PopulateInventory()
    {
        // Clear existing items from both containers
        foreach (Transform child in weaponsContainer)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in equipmentsContainer)
        {
            Destroy(child.gameObject);
        }

        
        if (headerPrefab != null)
        {
            GameObject weaponsHeader = Instantiate(headerPrefab, weaponsContainer);
            SetHeaderText(weaponsHeader, weaponsHeaderText);

            GameObject equipmentsHeader = Instantiate(headerPrefab, equipmentsContainer);
            SetHeaderText(equipmentsHeader, equipmentsHeaderText);
        }

        // Populate Weapons
        List<Weapon> weapons = InventoryManager.Instance.GetWeapons();
        foreach (Weapon weapon in weapons)
        {
            GameObject newItem = Instantiate(listItemPrefab, weaponsContainer);
            TextMeshProUGUI tmp = newItem.GetComponent<TextMeshProUGUI>();
            if (tmp != null)
            {
                // Display the weapon's properties
                tmp.text = $"{weapon.name} | Damage: {weapon.damage} | Cost: {weapon.cost}";
            }
            else
            {
                Text txt = newItem.GetComponent<Text>();
                if (txt != null)
                {
                    txt.text = $"{weapon.name} | Damage: {weapon.damage} | Cost: {weapon.cost}";
                }
            }
        }

        // Populate Equipments
        List<Equipment> equipments = InventoryManager.Instance.GetEquipments();
        foreach (Equipment equipment in equipments)
        {
            GameObject newItem = Instantiate(listItemPrefab, equipmentsContainer);
            TextMeshProUGUI tmp = newItem.GetComponent<TextMeshProUGUI>();
            if (tmp != null)
            {
                // Display the equipment's properties
                tmp.text = $"{equipment.name} | HP: {equipment.hp} | DEF: {equipment.def} | ATK: {equipment.atk} | Cost: {equipment.cost}";
            }
            else
            {
                Text txt = newItem.GetComponent<Text>();
                if (txt != null)
                {
                    txt.text = $"{equipment.name} | HP: {equipment.hp} | DEF: {equipment.def} | ATK: {equipment.atk} | Cost: {equipment.cost}";
                }
            }
        }

    
    }

    // Utility method to set header text
    void SetHeaderText(GameObject headerObject, string headerText)
    {
        TextMeshProUGUI tmp = headerObject.GetComponent<TextMeshProUGUI>();
        if (tmp != null)
        {
            tmp.text = headerText;
        }
        else
        {
            Text txt = headerObject.GetComponent<Text>();
            if (txt != null)
            {
                txt.text = headerText;
            }
        }
    }
}
