using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Assets.Scripts.Model;
using Assets.Scripts.Controller;
using System.Collections.Generic;
using System.Linq;

public class CombatUI : MonoBehaviour, IPointerClickHandler
{
    // UI Elements
    [Header("UI Components")]
    [SerializeField] private Transform combatUnitContainer;
    [SerializeField] private GameObject characterUIPrefab;
    [SerializeField] private TextMeshProUGUI turnText;
    [SerializeField] private TextMeshProUGUI combatLog;
    [SerializeField] private Button attackButton;
    [SerializeField] private Button endTurnButton;
    [SerializeField] private GameObject selectionFramePrefab;
    [SerializeField] private TextMeshProUGUI unitName;
    [SerializeField] private TextMeshProUGUI unitRole;
    [SerializeField] private Button retreatButton;
    [SerializeField] private GameObject retreatConfirmationPrefab;
    [SerializeField] private GameObject formationSlotPrefab;


    [Header("Settings")]
    [SerializeField] private float attackDelay = 0.5f;
    [SerializeField] private Color exhaustedColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

    // Selection System
    private Character selectedAlly;
    private Character selectedEnemy;
    private GameObject selectionFrame;
    private bool isAttackExecuting;
    private List<GameObject> soldierCards = new List<GameObject>();
    private List<GameObject> enemyCards = new List<GameObject>();

    void Start()
    {
        if (CombatManager.Instance == null)
        {
            Debug.Log("Debugging Scene");
            CombatManager.Instance = new GameObject().AddComponent<CombatManager>();
            CombatManager.Instance.GetAvailableSoldiers().Add(new Soldier("Test Soldier", new Role("Tank"), 3, 2, 1, 1));
            CombatManager.Instance.GetAvailableEnemies().Add(new Enemy("Test Enemy", 1, 3, 2, 1));
        }
        Debug.Log("CombatUI Start");
        // CombatManager.Instance.StartCombat(CombatManager.Instance.GetAvailableSoldiers(), CombatManager.Instance.GetAvailableEnemies());
        CombatManager.Instance.OnCombatEnd += OnCombatEnd;
        InitializeUI();
    }

    void InitializeUI()
    {
        CreateCharacterDisplays();
        Update();

        attackButton.onClick.AddListener(OnAttackButton);
        endTurnButton.onClick.AddListener(OnEndTurnButton);
        retreatButton.onClick.AddListener(OnRetreatButton);

        combatLog.text = "Combat Ready! Select a soldier to begin.";
        turnText.text = "Player's Turn";
        turnText.color = Color.white;
        Update();
    }

    void CreateCharacterDisplays()
    {
        int midX = Screen.width / 2;
        int midY = Screen.height / 2;
        int midAllyX = midX - 250;
        int midEnemyX = midX + 200;
        List<Vector3> allyPositions = new()
        {
            new Vector3(midAllyX - 100, midY + 200, 0),
            new Vector3(midAllyX + 100, midY + 100, 0),
            new Vector3(midAllyX - 100, midY, 0),
            new Vector3(midAllyX + 100, midY - 100, 0),
            new Vector3(midAllyX - 100, midY - 200, 0)
        };

        List<Vector2> enemyPositions = new List<Vector2>
        {
            new(midEnemyX, midY - 100),
            new(midEnemyX, midY + 100),
            new(midEnemyX + 200, midY),
            new(midEnemyX + 200, midY + 200),
            new(midEnemyX + 200, midY - 200),
            new(midEnemyX + 400, midY),
        };
        foreach (var soldier in CombatManager.Instance.GetSelectedCharacters())
        {
            int index = CombatManager.Instance.GetSelectedCharacters().IndexOf(soldier);
            if (soldier != null && soldier is Soldier) 
            {
                soldierCards.Add(CreateCharacterCard(soldier, isAlly: true, allyPositions[index]));
                soldier.SetGameObject(soldierCards[index]);
            }
            else
            {
                soldierCards.Add(null);
            }
        }

        Debug.Log($"Enemy numbers is: {CombatManager.Instance.GetAvailableEnemies().Count}");
        foreach (var enemy in CombatManager.Instance.GetAvailableEnemies())
        {
            Debug.Log($"Enemy found: {enemy.Name}");
            int index = CombatManager.Instance.GetAvailableEnemies().IndexOf(enemy);
            enemyCards.Add(CreateCharacterCard(enemy, isAlly: false, enemyPositions[index]));
            enemy.SetGameObject(enemyCards[index]);
        }
    }

    GameObject CreateCharacterCard(Character character, bool isAlly, Vector2 position)
    {
        var card = Instantiate(characterUIPrefab, combatUnitContainer);
        float xPosition = isAlly ? 350 : 600;
        card.transform.position = (Vector3)position;
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
            UpdateSelectionVisual();
        }
    }

    void OnCharacterClicked(Character character)
    {
        if (isAttackExecuting) return;

        if (CombatManager.Instance.IsAlly(character))
        {
            HandleAllySelection(character as Soldier);
            unitRole.text = $"Soldier - {(character as Soldier).GetRoleName()} \n Level: {character.Level}";
        }
        else if (CombatManager.Instance.IsEnemy(character))
        {
            HandleEnemySelection(character);
        }
        unitName.text = character.Name;
        Update();
    }

    void HandleAllySelection(Soldier soldier)
    {
        if (soldier == null || soldier.IsDead() || soldier.AttackChances <= 0) return;

        selectedAlly = soldier;
        selectedEnemy = null;
        Update();
        combatLog.text = $"Selected {soldier.Name}, now select a target.";
    }

    void HandleEnemySelection(Character enemy)
    {
        if (selectedAlly == null || enemy.IsDead()) return;

        selectedEnemy = enemy;
        Update();
        combatLog.text = $"Targeting {enemy.Name}, ready to attack!";
    }

    void UpdateSelectionVisual()
    {
        Destroy(selectionFrame);

        var target = selectedEnemy ?? selectedAlly;
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

        StartCoroutine(ExecuteAttackRoutine(selectedAlly, selectedEnemy));

        Update();
    }

    IEnumerator ExecuteAttackRoutine(Character attacker, Character target)
    {
        isAttackExecuting = true;
        Update();

        attacker.AttackChances--;

        // TODO: Show attack animation
        yield return StartCoroutine(PlayAttackAnimation(attacker, target));
        combatLog.text = $"{attacker.Name} attacks {target.Name}!";
        yield return new WaitForSeconds(attackDelay);
        CombatManager.Instance.ProcessAttack(attacker, target);

        ClearSelection();
        isAttackExecuting = false;
        Update();
        CleanDeadUnits(); // Clean up dead units after attack
        CheckTurnEnd();
    }

    IEnumerator PlayAttackAnimation(Character attacker, Character target)
    {
        var originalPos = attacker.GameObject.transform.position; 
        var targetPos = target.GameObject.transform.position;

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

    void CleanDeadUnits()
    {
        // Remove dead units from the UI
        foreach (var card in soldierCards)
        {
            if (card == null) continue; // Skip if card is null
            if (card.GetComponent<CharacterUI>().Character.IsDead())
            {
                Destroy(card);
            }
        }
        List<GameObject> temp = new List<GameObject>(enemyCards);
        foreach (var card in temp)
        {
            if (card == null) continue; // Skip if card is null
            if (card.GetComponent<CharacterUI>().Character.IsDead())
            {
                Vector3 position = card.transform.position; // Save position before destroying
                Destroy(card);
                enemyCards.Remove(card);
                
                if (CombatManager.Instance.GetWaitingEnemies().Count > 0)
                {
                    // Get the waiting enemy BEFORE removing it
                    Enemy waitingEnemy = CombatManager.Instance.GetWaitingEnemies()[0];
                    
                    // Create new card and store the reference
                    GameObject newCard = CreateCharacterCard(waitingEnemy, isAlly: false, position);
                    enemyCards.Add(newCard);
                    Button newButton = newCard.GetComponent<Button>();
                    newButton.interactable = true;
                    
                    // Set the GameObject reference
                    waitingEnemy.SetGameObject(newCard);
                    
                    // Add it to available enemies and enemy characters
                    CombatManager.Instance.GetAvailableEnemies().Add(waitingEnemy);
                    CombatManager.Instance.GetEnemyCharacters().Add(waitingEnemy);
                    
                    // Remove from waiting list
                    CombatManager.Instance.GetWaitingEnemies().RemoveAt(0);
                }
            }
        }
    }

    void CheckTurnEnd()
    {
        Update();
        bool allExhausted = true;
        foreach (var soldier in CombatManager.Instance.GetAvailableSoldiers())
        {
            if (soldier == null) continue; // Skip if soldier is null
            if (soldier.AttackChances > 0 && !soldier.IsDead())
            {
                allExhausted = false;
                break;
            }
        }

        endTurnButton.image.color = allExhausted ? Color.red : Color.white;
        Update();
    }

    void OnRetreatButton()
    {
        DisableAll();

        // Add Canvas Group to block clicks
        GameObject confirmWindow = Instantiate(retreatConfirmationPrefab, transform);
        CanvasGroup group = confirmWindow.AddComponent<CanvasGroup>();
        group.blocksRaycasts = true;
        group.interactable = true;

        // Pause combat
        Time.timeScale = 0;

        confirmWindow.GetComponent<RetreatConfirmation>().Initialize(
            onConfirm: () => {
                Destroy(confirmWindow);
                Time.timeScale = 1;
                OnCombatEnd(false); // Trigger combat failure
            },
            onCancel: () => {
                Destroy(confirmWindow);
                Time.timeScale = 1;
                isAttackExecuting = false;
                EnableAll();
                Update();
            }
        );
        confirmWindow.GetComponent<RetreatConfirmation>().SetMessage("Are you sure you want to retreat?");
    }


    public void OnEndTurnButton()
    {
        endTurnButton.image.color = endTurnButton.image.color == Color.red ? Color.white : endTurnButton.image.color;
        StartCoroutine(EndTurnRoutine());
        Update();

        CombatManager.Instance.CheckAndReplaceDeadEnemies();
    }

    IEnumerator EndTurnRoutine()
    {
        isAttackExecuting = true;
        Update();

        yield return new WaitForSeconds(attackDelay);

        CombatManager.Instance.EndCurrentTurn();
        ResetAttackChances();
        ClearSelection();

        isAttackExecuting = false;
        if (CombatManager.Instance.IsPlayerTurn)
        {
            Debug.Log("Player's Turn Finished.");
            yield break;
        } else {
            Debug.Log("Enemy's Turn Started.");
            OnEnemyTurn();
        }
        Update();
    }

    private void OnEnemyTurn()
    {
        Debug.Log("Enemy's Turn Started.");
        Debug.Log($"length of GetAvailableEnemies {CombatManager.Instance.GetAvailableEnemies().Count}");

        Queue<Enemy> enemyQueue = new Queue<Enemy>();

        foreach (var enemy in CombatManager.Instance.GetAvailableEnemies())
        {
            if (!enemy.IsDead())
            {
                enemyQueue.Enqueue(enemy);
            }
        }

        StartCoroutine(ExecuteEnemyTurn(enemyQueue));
    }

    private IEnumerator ExecuteEnemyTurn(Queue<Enemy> enemyQueue)
    {
        isAttackExecuting = true;
        Update();
        while (enemyQueue.Count > 0)
        {
            var attacker = enemyQueue.Dequeue();

            if (attacker.IsDead()) continue;

            var target = CombatManager.Instance.GetRandomSoldier();
            if (target != null)
            {
                Debug.Log($"{attacker.Name} is attacking {target.Name}");
                yield return StartCoroutine(ExecuteAttackRoutine(attacker, target)); // �ȴ���ǰ���˹������
            }
        }

        ResetAttackChances();
        Update();

        CombatManager.Instance.CheckAndReplaceDeadEnemies();
        isAttackExecuting = false;
        yield return null;
    }


    void ResetAttackChances()
    {
        foreach (var soldier in CombatManager.Instance.GetAvailableSoldiers())
        {
            soldier.AttackChances = soldier.MaxAttacksPerTurn;
        }

        foreach (var enemy in CombatManager.Instance.GetAvailableEnemies())
        {
            enemy.AttackChances = enemy.MaxAttacksPerTurn;
        }
    }

    void UpdateButtonStates()
    {
        attackButton.interactable = CanAttack() && !isAttackExecuting;
        endTurnButton.interactable = !isAttackExecuting;

        bool isPlayerTurn = CombatManager.Instance.IsPlayerTurn;
        turnText.text = isPlayerTurn ? "Player's Turn" : "Enemy's Turn";
        turnText.color = isPlayerTurn ? Color.white : Color.red;
        attackButton.gameObject.SetActive(isPlayerTurn);
        endTurnButton.gameObject.SetActive(isPlayerTurn);

        retreatButton.interactable = !isAttackExecuting && isPlayerTurn;
    }

    void ClearSelection()
    {
        selectedAlly = null;
        selectedEnemy = null;
        Destroy(selectionFrame);
    }

    void OnCombatEnd(bool victory)
    {
        DisableAll();
        string resultMessage = victory ? "Victory!" : "Defeat!";
        combatLog.fontSize = 36; // Make text larger
        combatLog.color = victory ? Color.white : Color.red; // Change color based on result
        combatLog.text = resultMessage;
        StartCoroutine(ReturnToBaseAfterDelay(5f)); // Increased delay to see the message
    }

    IEnumerator ReturnToBaseAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        // Load other scene or reset UI
        Debug.Log("Returning to base...");
        GameManager.Instance.LoadGameState(GameState.MissionPage);
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
                    isExhausted: (ui.Character is Soldier s) && s.AttackChances <= 0,
                    isAlly: CombatManager.Instance.IsAlly(ui.Character),
                    isDead: ui.Character.IsDead()
                );
            }
        }
        UpdateButtonStates();
        UpdateSelectionVisual();
    }

    void EnableAll()
    {
        isAttackExecuting = false; // Reset combat state

        // Enable all character buttons
        foreach (Transform child in combatUnitContainer)
        {
            var button = child.GetComponent<Button>();
            if (button != null)
            {
                button.interactable = true;
            }
        }

        UpdateButtonStates(); // Force refresh button states
    }

    void DisableAll()
    {
        attackButton.interactable = false;
        endTurnButton.interactable = false;
        retreatButton.interactable = false; // This line can be kept

        isAttackExecuting = true;

        foreach (Transform child in combatUnitContainer)
        {
            var button = child.GetComponent<Button>();
            if (button != null)
            {
                button.interactable = false;
            }
        }
    }
}
