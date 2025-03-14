using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Assets.Scripts.Model;
using Assets.Scripts.Controller;

public class CombatUI : MonoBehaviour, IPointerClickHandler
{
    // UI Elements
    [Header("UI Components")]
    public Transform combatUnitContainer;
    public GameObject characterUIPrefab;
    public TextMeshProUGUI turnText;
    public TextMeshProUGUI combatLog;
    public Button attackButton;
    public Button endTurnButton;
    public GameObject selectionFramePrefab;

    [Header("Settings")]
    [SerializeField] private float attackDelay = 0.5f;
    [SerializeField] private Color exhaustedColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

    // Selection System
    private Character selectedAlly;
    private Character selectedEnemy;
    private GameObject selectionFrame;
    private bool isAttackExecuting;

    void Start()
    {
        if (CombatManager.Instance == null)
        {
            Debug.Log("Debugging Scene");
            CombatManager.Instance = new GameObject().AddComponent<CombatManager>();
        }
        CombatManager.Instance.OnCombatEnd += OnCombatEnd;
        InitializeUI();
    }

    void InitializeUI()
    {
        CreateCharacterDisplays();
        UpdateButtonStates();
        combatLog.text = "Combat Ready!";
    }

    void CreateCharacterDisplays()
    {
        float allyY = 800; // Starting Y position for allies
        foreach (var soldier in CombatManager.Instance.GetAvailableSoldiers())
        {
            CreateCharacterCard(soldier, isAlly: true, allyY);
            allyY -= 175; // Decrease Y for next ally
        }

        float enemyY = 600; // Starting Y position for enemies
        foreach (var enemy in CombatManager.Instance.GetAvailableEnemies())
        {
            CreateCharacterCard(enemy, isAlly: false, enemyY);
            enemyY -= 175; // Decrease Y for next enemy
        }
    }

    GameObject CreateCharacterCard(Character character, bool isAlly, float yPosition)
    {
        var card = Instantiate(characterUIPrefab, combatUnitContainer);
        float xPosition = isAlly ? 200 : 700; // Allies at x=200, enemies at x=700
        card.transform.position = new Vector3(xPosition, yPosition, 0);
        var ui = card.GetComponent<CharacterUI>();
        ui.Initialize(character, isAlly);
        
        // Add click handler
        var button = card.GetComponent<Button>();
        button.onClick.AddListener(() => OnCharacterClicked(character));
        return card;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject == null)
        {
            ClearSelection();
        }
    }

    void OnCharacterClicked(Character character)
    {
        if (isAttackExecuting) return;

        if (CombatManager.Instance.IsAlly(character))
        {
            HandleAllySelection(character as Soldier);
        }
        else if (CombatManager.Instance.IsEnemy(character))
        {
            HandleEnemySelection(character);
        }
    }

    void HandleAllySelection(Soldier soldier)
    {
        if (soldier == null || soldier.IsDead() || soldier.AttackChances <= 0) return;

        selectedAlly = soldier;
        selectedEnemy = null;
        UpdateSelectionVisual();
        combatLog.text = $"Selected {soldier.Name}";
    }

    void HandleEnemySelection(Character enemy)
    {
        if (selectedAlly == null || enemy.IsDead()) return;

        selectedEnemy = enemy;
        UpdateSelectionVisual();
        combatLog.text = $"Targeting {enemy.Name}";
    }

    void UpdateSelectionVisual()
    {
        Destroy(selectionFrame);
        
        var target = selectedEnemy != null ? selectedEnemy : selectedAlly;
        if (target != null && target.GameObject != null)
        {
            selectionFrame = Instantiate(selectionFramePrefab, target.GameObject.transform);
        }
        
        attackButton.interactable = CanAttack();
    }

    bool CanAttack()
    {
        return selectedAlly != null && 
               selectedEnemy != null &&
               (selectedAlly as Soldier).AttackChances > 0;
    }

    public void OnAttackButton()
    {
        if (!CanAttack()) return;
        
        StartCoroutine(ExecuteAttackRoutine());
    }

    IEnumerator ExecuteAttackRoutine()
    {
        isAttackExecuting = true;
        UpdateButtonStates();

        var soldier = selectedAlly as Soldier;
        soldier.AttackChances--;
        
        // Show attack animation
        yield return StartCoroutine(PlayAttackAnimation(soldier, selectedEnemy));
        
        CombatManager.Instance.ProcessAttack(soldier, selectedEnemy);
        combatLog.text = $"{soldier.Name} attacks {selectedEnemy.Name}!";
        
        ClearSelection();
        isAttackExecuting = false;
        UpdateButtonStates();
        CheckTurnEnd();
    }

    IEnumerator PlayAttackAnimation(Character attacker, Character target)
    {
        var originalPos = attacker.GameObject.transform.position;
        Vector3 targetPos = target.GameObject.transform.position;

        // Move towards target
        float duration = 0.2f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            attacker.GameObject.transform.position = Vector3.Lerp(
                originalPos, 
                targetPos - new Vector3(1,0,0), 
                elapsed/duration
            );
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Return to original position
        elapsed = 0f;
        while (elapsed < duration)
        {
            attacker.GameObject.transform.position = Vector3.Lerp(
                targetPos - new Vector3(1,0,0),
                originalPos,
                elapsed/duration
            );
            elapsed += Time.deltaTime;
            yield return null;
        }

        attacker.GameObject.transform.position = originalPos;
    }

    void CheckTurnEnd()
    {
        bool allExhausted = true;
        foreach (var soldier in CombatManager.Instance.GetAvailableSoldiers())
        {
            if (soldier.AttackChances > 0 && !soldier.IsDead())
            {
                allExhausted = false;
                break;
            }
        }

        endTurnButton.image.color = allExhausted ? Color.red : Color.white;
    }

    public void OnEndTurnButton()
    {
        StartCoroutine(EndTurnRoutine());
    }

    IEnumerator EndTurnRoutine()
    {
        isAttackExecuting = true;
        UpdateButtonStates();
        
        yield return new WaitForSeconds(attackDelay);
        
        CombatManager.Instance.EndCurrentTurn();
        ResetAttackChances();
        ClearSelection();
        
        isAttackExecuting = false;
        UpdateButtonStates();
    }

    void ResetAttackChances()
    {
        foreach (var soldier in CombatManager.Instance.GetAvailableSoldiers())
        {
            soldier.AttackChances = soldier.MaxAttacksPerTurn;
        }
    }

    void UpdateButtonStates()
    {
        attackButton.interactable = CanAttack() && !isAttackExecuting;
        endTurnButton.interactable = !isAttackExecuting;
        
        bool isPlayerTurn = CombatManager.Instance.IsPlayerTurn;
        turnText.text = isPlayerTurn ? "Player's Turn" : "Enemy's Turn";
        attackButton.gameObject.SetActive(isPlayerTurn);
        endTurnButton.gameObject.SetActive(isPlayerTurn);
        turnText.color = isPlayerTurn ? Color.blue : Color.red;
    }

    void ClearSelection()
    {
        selectedAlly = null;
        selectedEnemy = null;
        Destroy(selectionFrame);
        UpdateButtonStates();
    }

    void OnCombatEnd(bool victory)
    {
        combatLog.text = victory ? "Victory!" : "Defeat!";
        StartCoroutine(ReturnToBaseAfterDelay(2f));
    }

    IEnumerator ReturnToBaseAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        // Load other scene or reset UI
    }

    void Update()
    {
        // Update character UI states
        foreach (Transform child in combatUnitContainer)
        {
            var ui = child.GetComponent<CharacterUI>();
            if (ui != null)
            {
                ui.UpdateState(
                    isSelected: ui.Character == selectedAlly || ui.Character == selectedEnemy,
                    isExhausted: (ui.Character is Soldier s) && s.AttackChances <= 0
                );
            }
        }
    }
}