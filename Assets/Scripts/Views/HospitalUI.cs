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

    public TextMeshProUGUI healsLeftDisplay;
    public TextMeshProUGUI healStatusDisplay;

    public TextMeshProUGUI soldierHealthDisplay;
    public TextMeshProUGUI soldierNameDisplay;
    public TextMeshProUGUI healAmountDisplay;
    public Button healSoldierButton;


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
        healSoldierButton.onClick.AddListener(OnHealButtonClicked);

        healsLeftDisplay.GetComponent<TextMeshProUGUI>().text = "Healing Remaining: " + GameManager.Instance.currentGame.resourcesData.GetAmount(6);
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

    void OnHealButtonClicked()
    {
        int healingLeft = GameManager.Instance.currentGame.resourcesData.GetAmount(6);
        int healAmount = 10;
        Character soldier = currentSelectedSoldier;
        if (soldier.MaxHealth - soldier.Health < 10)
        {
            healAmount = soldier.MaxHealth - soldier.Health;
        }

        if (healingLeft > 0 && healAmount != 0)
        {
            currentSelectedSoldier.Health += healAmount;

            GameManager.Instance.currentGame.resourcesData.SetAmount(6, healingLeft - 1);
            healsLeftDisplay.GetComponent<TextMeshProUGUI>().text = "Healing Remaining: " + GameManager.Instance.currentGame.resourcesData.GetAmount(6);

            healStatusDisplay.text = "STATUS: Healed " + currentSelectedSoldier.Name;

            updateMenu(currentSelectedSoldier);
        } 
        else if (healingLeft <= 0)
        {
            healStatusDisplay.text = "STATUS: No healing left";
        } else if (healAmount <= 0)
        {
           healStatusDisplay.text = "STATUS: Health already full";
        }
    }

    public void updateMenu(Character soldier)
    {
        soldierHealthDisplay.text = "Current HP: " + soldier.Health + "/" + soldier.MaxHealth;
        soldierNameDisplay.text = "Selected Soldier: " + soldier.Name;

        int healAmount = 10;
        if (soldier.MaxHealth - soldier.Health < 10)
        {
            healAmount = soldier.MaxHealth - soldier.Health;
        }

        healAmountDisplay.text = "Heal Amount: " + healAmount;
    }
}
