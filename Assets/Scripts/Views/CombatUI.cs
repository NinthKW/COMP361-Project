using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Model;
using Assets.Scripts.Controller;

public class CombatUI : MonoBehaviour
{
    public Button attackButton;
    
    // Selection state variables
    private enum SelectionState { None, SelectingSoldier, SelectingEnemy }
    private SelectionState currentState = SelectionState.None;
    private GameObject selectedSoldier = null;
    
    void Start()
    {
        // 检查CombatManager是否存在，不存在则创建
        if (CombatManager.Instance == null)
        {
            GameObject combatManagerObj = new GameObject("CombatManager");
            combatManagerObj.AddComponent<CombatManager>();
        }
        
        attackButton.name = "Attack";
        attackButton.onClick.AddListener(OnAttackButtonClicked);
        CombatManager.Instance.StartCombat();
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
    
    void OnAttackButtonClicked()
    {
        // Enter soldier selection mode
        currentState = SelectionState.SelectingSoldier;
        Debug.Log("Select a soldier to attack with");
        HighlightSoldiers(true);
        UpdateUI();
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