using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Model;
using Assets.Scripts.Controller;
using TMPro;

public class CombatUI : MonoBehaviour
{
    // UI elements
    public Transform combatUnitContainer;
    public GameObject soldierPrefab;
    public GameObject enemyPrefab;
    public TextMeshProUGUI turnText;
    public Transform actionPanel;
    public Button attackButton;
    public Button endTurnButton;
    public Button startCombatButton;
    public Button backButton;
    // Selection state variables
    private enum SelectionState { None, SelectingSoldier, SelectingEnemy }
    private SelectionState currentState = SelectionState.None;
    private GameObject selectedSoldier = null;
    
    // Method to create soldier and enemy game objects
    void CreateCombatUnits()
    {
        List<Soldier> soldiers = CombatManager.Instance.GetAvailableSoldiers();
        List<Enemy> enemies = CombatManager.Instance.GetAvailableEnemies();
        
        for (int i = 0; i < soldiers.Count; i++)
        {
            Vector3 position = new Vector3(-10.0f, i * 100, i * 5.0f);
            GameObject soldier = CreateSoldier(position);
            soldiers[i].SetGameObject(soldier);
        }
    }

    GameObject CreateSoldier(Vector3 position)
    {
        GameObject soldier = GameObject.Instantiate(soldierPrefab, combatUnitContainer);
        soldier.name = "Soldier_" + Random.Range(1000, 9999);
        soldier.tag = "Soldier";
        soldier.transform.position = position;
        
        return soldier;
    }

    GameObject CreateEnemy(Vector3 position)
    {
        GameObject enemy = GameObject.Instantiate(enemyPrefab, combatUnitContainer);
        enemy.name = "Enemy_" + Random.Range(1000, 9999);
        enemy.tag = "Enemy";
        enemy.transform.position = position;
        
        return enemy;
    }

    void Start()
    {
        if (CombatManager.Instance == null)
        {
            Debug.LogError("CombatManager.Instance is NULL, creating new instance");
            CombatManager.Instance = gameObject.AddComponent<CombatManager>();
        }
        // attackButton.gameObject.SetActive(false);
        // endTurnButton.gameObject.SetActive(false);
        // startCombatButton.gameObject.SetActive(true);
        // backButton.gameObject.SetActive(true);
        CreateCombatUnits();
        Debug.Log(message: "Combat UI initialized");

        startCombatButton.onClick.AddListener(OnStartCombatButtonClicked);
        backButton.onClick.AddListener(OnRetreatButtonClicked);
                
        UpdateUI();
    }
    void Update()
    {
        // Handle mouse input for selecting units
        if (Input.GetMouseButtonDown(0) && currentState != SelectionState.None)
        {
            HandleSelection();
        }
    }

    void OnStartCombatButtonClicked()
    {
        // Start the combat
        CombatManager.Instance.StartCombat();
        startCombatButton.gameObject.SetActive(false);
        endTurnButton.gameObject.SetActive(true);
        attackButton.gameObject.SetActive(true);
        UpdateUI();
    }
    
    void OnAttackButtonClicked()
    {
        // Enter soldier selection mode
        currentState = SelectionState.SelectingSoldier;
        Debug.Log("Select a soldier to attack with");
        HighlightSoldiers(true);
        UpdateUI();
    }

    void OnRetreatButtonClicked()
    {
        // Implement retreat logic
        Debug.Log("Retreat button clicked");
        CombatManager.Instance.EndCombat(false);
    }
    
    void HandleSelection()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit))
        {
            if (currentState == SelectionState.SelectingSoldier && IsSoldier(hit.collider.gameObject))
            {
                // Soldier selected
                selectedSoldier = hit.collider.gameObject;
                HighlightSoldiers(false);
                HighlightEnemies(true);
                currentState = SelectionState.SelectingEnemy;
                Debug.Log($"Selected {selectedSoldier.name}. Now select an enemy to attack.");
            }
            else if (currentState == SelectionState.SelectingEnemy && IsEnemy(hit.collider.gameObject))
            {
                // Enemy selected - perform attack
                GameObject enemy = hit.collider.gameObject;
                ExecuteAttack(selectedSoldier, enemy);
                
                // Reset selection state
                ResetSelectionState();
            }
        }
    }
    
    bool IsSoldier(GameObject obj)
    {
        // Implement logic to check if object is a soldier
        return obj.CompareTag("Soldier");
    }
    
    bool IsEnemy(GameObject obj)
    {
        // Implement logic to check if object is an enemy
        return obj.CompareTag("Enemy");
    }
    
    void HighlightSoldiers(bool highlight)
    {
        // Implement logic to visually highlight selectable soldiers
        GameObject[] soldiers = GameObject.FindGameObjectsWithTag("Soldier");
        foreach (GameObject soldier in soldiers)
        {
            // Add visual highlight effect
            SetHighlight(soldier, highlight);
        }
    }
    
    void HighlightEnemies(bool highlight)
    {
        // Implement logic to visually highlight selectable enemies
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            SetHighlight(enemy, highlight);
        }
    }
    
    void SetHighlight(GameObject obj, bool highlight)
    {
        // Implement your highlight effect (outline, glow, etc)
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            // Example: Change material or color
            renderer.material.color = highlight ? Color.yellow : Color.white;
        }
    }
    
    void ExecuteAttack(GameObject attacker, GameObject target)
    {
        Debug.Log($"{attacker.name} is attacking {target.name}");
        CombatManager.Instance.Attack(attacker, target);
    }
    
    void ResetSelectionState()
    {
        currentState = SelectionState.None;
        selectedSoldier = null;
        HighlightSoldiers(false);
        HighlightEnemies(false);
        UpdateUI();
    }
    
    void UpdateUI()
    {
        // Enable attack button only when it's the player's turn and not in selection mode
        attackButton.interactable = CombatManager.Instance.IsPlayerTurn() && currentState == SelectionState.None;
    }
    
    // Call this when the turn changes
    public void OnTurnChanged()
    {
        ResetSelectionState();
    }
}