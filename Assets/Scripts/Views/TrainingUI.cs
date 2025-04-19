using Assets.Scripts;
using Assets.Scripts.Controller;
using Assets.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class TrainingUI : MonoBehaviour
{
    public static TrainingUI Instance;

    public Transform soldierGrid;
    public GameObject soldierPrefab;
    public Character currentSelectedSoldier;

    public TextMeshProUGUI foodLeftDisplay;
    public TextMeshProUGUI levelStatusDisplay;

    public TextMeshProUGUI soldierLevelDisplay;
    public TextMeshProUGUI soldierNameDisplay;
    public TextMeshProUGUI levelUpCostDisplay;
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

        foodLeftDisplay.GetComponent<TextMeshProUGUI>().text = "Food Remaining: " + GameManager.Instance.currentGame.resourcesData.GetAmount(0);
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

        // your existing name & model wiring
        buttonGameObject.GetComponent<TextMeshProUGUI>().text = soldier.Name;
        buttonGameObject.GetComponent<TrainingSoldier>().soldier = soldier;

        // —— new: load & assign role sprite ——
        if (soldier is Soldier soldierData)
        {
            string roleName = soldierData.GetRoleName();              // e.g. "Tank"
            Sprite roleSprite = UnityEngine.Resources.Load<Sprite>(roleName);     // looks in Assets/Resources/

            if (roleSprite != null)
            {
                // try to grab an Image on the prefab or its child
                Image characterImage = buttonGameObject.GetComponent<Image>()
                                       ?? buttonGameObject.GetComponentInChildren<Image>();
                if (characterImage != null)
                    characterImage.sprite = roleSprite;
                else
                    Debug.LogWarning($"[TrainingUI] No Image component on '{buttonGameObject.name}' to show sprite.");
            }
            else
            {
                Debug.LogWarning($"[TrainingUI] Sprite not found for role '{roleName}'.");
            }
        }
    }
}


    void OnBackButtonClicked()
    {
        GameManager.Instance.ChangeState(GameState.BasePage);
        GameManager.Instance.LoadGameState(GameState.BasePage);
    }

    void OnHealButtonClicked()
    {
        int foodLeft = GameManager.Instance.currentGame.resourcesData.GetAmount(0);

        Debug.Log(foodLeft);

        Character soldier = currentSelectedSoldier;
        int levelUpCost = calculateCostFunction(soldier.Level + 1);

        if (foodLeft >= levelUpCost)
        {
            currentSelectedSoldier.Level += 1;

            GameManager.Instance.currentGame.resourcesData.SetAmount(0, foodLeft - levelUpCost);
            foodLeftDisplay.GetComponent<TextMeshProUGUI>().text = "Food Remaining: " + GameManager.Instance.currentGame.resourcesData.GetAmount(0);

            levelStatusDisplay.text = "STATUS: " + currentSelectedSoldier.Name + " has leveled up";

            updateMenu(currentSelectedSoldier);
        } 
        else
        {
            levelStatusDisplay.text = "STATUS: Not enough food to level up";
        } 
    }

    public void updateMenu(Character soldier)
    {
        soldierLevelDisplay.text = "Current Level: " + soldier.Level;
        soldierNameDisplay.text = "Selected Soldier: " + soldier.Name;

        levelUpCostDisplay.text = "Level Up Cost: " + calculateCostFunction(soldier.Level + 1);
    }

    private int calculateCostFunction(int currentLevel)
    {
        return (int) System.Math.Round(System.Math.Log10(currentLevel + 1 ) * 50);
    }
}
