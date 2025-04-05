using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Assets.Scripts.Model;
using Assets.Scripts.Controller;
using System.Collections.Generic;

public class CombatUI : MonoBehaviour, IPointerClickHandler
{
    #region UI Components
    [Header("Unit Container Settings")]
    [SerializeField] private Transform combatUnitContainer;
    [SerializeField] private GameObject characterUIPrefab;
    [SerializeField] private GameObject selectionFramePrefab;

    [Header("Combat UI Settings")]
    [SerializeField] private TextMeshProUGUI turnText;
    [SerializeField] private TextMeshProUGUI combatLog;
    [SerializeField] private TextMeshProUGUI unitName;
    [SerializeField] private TextMeshProUGUI unitRole;
    [SerializeField] private Button attackButton;
    [SerializeField] private Button endTurnButton;
    [SerializeField] private Button retreatButton;
    [SerializeField] private GameObject retreatConfirmationPrefab;

    [Header("Ability Panel Settings")]
    [SerializeField] private GameObject abilityPanel; // 能力面板（在场景中提前挂好）
    [SerializeField] private GameObject abilityButtonPrefab; // 能力按钮预制件

    [Header("Position Settings")]
    [SerializeField] private List<Vector3> allyPositions = new();
    [SerializeField] private List<Vector3> enemyPositions = new();

    [Header("Combat Settings")]
    [SerializeField] private float attackDelay = 0.5f;
    [SerializeField] private Color exhaustedColor = new(0.5f, 0.5f, 0.5f, 0.5f);
    #endregion

    #region Combat State
    private Character selectedAlly;
    private Character selectedEnemy;
    private GameObject selectionFrame;
    private Ability selectedAbility = null;  // 当前选中的技能
    private Character abilityTarget = null;    // 技能目标（如果需要选择目标）
    private bool isAttackExecuting;
    private readonly List<GameObject> soldierCards = new();
    private readonly List<GameObject> enemyCards = new();
    private readonly List<GameObject> waitingEnemyCards = new();
    #endregion

    #region Lifecycle Methods
    void Start()
    {
        InitializeCombatManager();
        InitializeUIComponents();
        SubscribeToEvents();
    }

    void Update()
    {
        UpdateCharacterUIStates();
        UpdateButtonStates();
        UpdateSelectionVisual();
    }

    void OnDestroy()
    {
        if (CombatManager.Instance != null)
            CombatManager.Instance.OnCombatEnd -= OnCombatEnd;
    }
    #endregion

    #region Initialization
    void InitializeCombatManager()
    {
        if (CombatManager.Instance != null) return;

        Debug.Log("Initializing Test Combat Manager");
        var managerObj = new GameObject("CombatManager");
        CombatManager.Instance = managerObj.AddComponent<CombatManager>();
    }

    void InitializeUIComponents()
    {
        CreateCharacterDisplays();
        SetupButtonListeners();
        InitializeCombatLog();
    }

    void SubscribeToEvents() => 
        CombatManager.Instance.OnCombatEnd += OnCombatEnd;
    #endregion

    #region UI Creation
    void CreateCharacterDisplays()
    {
        CreateSoldierCards();
        CreateEnemyCards();
    }

    void CreateSoldierCards()
    {
        int midX = (Screen.width / 2) - 250;
        int midY = Screen.height / 2;
        allyPositions = new List<Vector3>
        {
            new (x: midX-100, midY+200, 0),
            new (midX+100, midY+100, 0),
            new (midX-100, midY, 0),
            new (midX+100, midY-100, 0),
            new (midX-100, midY-200, 0)
        };
        foreach (var soldier in CombatManager.Instance.GetInBattleSoldiers())
        {
            if (soldier is not Soldier validSoldier) continue;
            
            var index = CombatManager.Instance.GetInBattleSoldiers().IndexOf(soldier);
            var card = CreateCharacterCard(validSoldier, true, allyPositions[index]);
            soldierCards.Add(card);
            validSoldier.SetGameObject(card);
        }
    }

    void CreateEnemyCards()
    {
        int midX = (Screen.width / 2) + 200;
        int midY = Screen.height / 2;
        enemyPositions = new List<Vector3>
        {
            new (x: midX-100, midY+200, 0),
            new (midX+100, midY+100, 0),
            new (midX-100, midY, 0),
            new (midX+100, midY-100, 0),
            new (midX-100, midY-200, z: 0),
            new (midX+300, midY, 0)
        };
        foreach (var enemy in CombatManager.Instance.GetAvailableEnemies())
        {
            var index = CombatManager.Instance.GetAvailableEnemies().IndexOf(enemy);
            Debug.Log($"Enemy Index: {index}");
            var card = CreateCharacterCard(enemy, false, enemyPositions[index]);
            enemyCards.Add(card);
            enemy.SetGameObject(card);
        }
        foreach (var enemy in CombatManager.Instance.GetWaitingEnemies())
        {
            var card = CreateCharacterCard(enemy, false, new Vector3(Screen.width + 200, -200, 0));
            waitingEnemyCards.Add(card);
            enemy.SetGameObject(card);
        }
    }

    GameObject CreateCharacterCard(Character character, bool isAlly, Vector2 position)
    {
        var card = Instantiate(characterUIPrefab, combatUnitContainer);
        card.transform.position = position;
        
        var ui = card.GetComponent<CharacterUI>();
        ui.Initialize(character, isAlly);
        
        SetupCardInteraction(card, character);
        return card;
    }

    void SetupCardInteraction(GameObject card, Character character)
    {
        if (!card.TryGetComponent<Button>(out var button))
            button = card.AddComponent<Button>();
        
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => OnCharacterClicked(character));
    }
    #endregion

    #region Combat Logic
    public void OnAttackButton()
    {
        if (!CanAttack()) return;
        StartCoroutine(ExecuteAttackRoutine(selectedAlly, selectedEnemy));
    }

    IEnumerator ExecuteAttackRoutine(Character attacker, Character target)
    {
        isAttackExecuting = true;
        UpdateCombatLog($"{attacker.Name} attacks {target.Name}!");
        
        attacker.AttackChances--;
        
        yield return StartCoroutine(PlayAttackAnimation(attacker, target, attacker.GetAttackAmount(target)));
        PostAttackCleanup();
        
        yield return new WaitForSeconds(attackDelay);
        isAttackExecuting = false;
    }

    IEnumerator PlayAttackAnimation(Character attacker, Character target, int damage)
    {
        if (attacker.GameObject == null || target.GameObject == null) yield break;
        var originalPos = attacker.GameObject.transform.position;
        var targetPos = target.GameObject.transform.position;

        // 冲锋动画
        yield return MoveToPosition(attacker.GameObject.transform, 
            targetPos - new Vector3(1, 0, 0), 0.2f);

        ShowDamageText(target, damage);
        CombatManager.Instance.ProcessAttack(attacker, target);

        // 返回动画
        yield return MoveToPosition(attacker.GameObject.transform, 
            originalPos, 0.2f);
    }

    IEnumerator MoveToPosition(Transform objTransform, Vector3 targetPos, float duration)
    {
        float elapsed = 0f;
        Vector3 startPos = objTransform.position;

        while (elapsed < duration)
        {
            objTransform.position = Vector3.Lerp(
                startPos, 
                targetPos, 
                elapsed / duration
            );
            elapsed += Time.deltaTime;
            yield return null;
        }
        objTransform.position = targetPos;
    }

    private void ShowDamageText(Character target, int damage)
    {
        Canvas canvas = FindObjectOfType<Canvas>();

        if (canvas == null)
        {
            Debug.LogError("No Canvas found in the scene!");
            return;
        }

        // 创建一个新的 TextMeshProUGUI 对象
        GameObject damageTextObj = new GameObject("DamageText");
        damageTextObj.transform.SetParent(canvas.transform, false);

        TextMeshProUGUI textMesh = damageTextObj.AddComponent<TextMeshProUGUI>();
        textMesh.text = $"-{damage}";
        textMesh.fontSize = 36;
        textMesh.color = Color.red;
        textMesh.alignment = TextAlignmentOptions.Center;
        textMesh.raycastTarget = false;

        // 设置位置到目标上方（从世界坐标转化为屏幕坐标）
        Vector3 screenPosition = (target.GameObject.transform.position + new Vector3(0, 2f, 0));
        damageTextObj.transform.position = screenPosition;

        // 开始淡出协程
        StartCoroutine(FadeAndDestroyText(textMesh));
    }

    private IEnumerator FadeAndDestroyText(TextMeshProUGUI textMesh)
    {
        float duration = 2f;
        //float fadeSpeed = 0.5f;

        Color originalColor = textMesh.color;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            // 让文字慢慢上升
            textMesh.transform.Translate(Vector3.up * Time.deltaTime * 20f);

            // 控制透明度
            float alpha = Mathf.Lerp(1f, 0f, t / duration);
            textMesh.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            yield return null;
        }

        Destroy(textMesh.gameObject);
    }

    void PostAttackCleanup()
    {
        ClearSelection();
        CheckTurnEnd();
        UpdateCombatState();
    }

    void CleanDeadUnits()
    {
        CleanSoldierUnits();
        CleanEnemyUnits();
    }

    void CleanSoldierUnits()
    {
        foreach (var card in soldierCards.ToArray())
        {
            if (card == null) continue;
            
            var character = card.GetComponent<CharacterUI>().Character;
            if (!character.IsDead()) continue;
            
            soldierCards.Remove(card);
            Destroy(card);
        }
    }

    void CleanEnemyUnits()
    {
        foreach (var card in enemyCards.ToArray())
        {
            if (card == null) continue;
            
            var enemy = card.GetComponent<CharacterUI>().Character;
            if (!enemy.IsDead()) continue;

            var position = card.transform.position;
            enemyCards.Remove(card);
            Destroy(card);
            
            if (waitingEnemyCards.Count == 0) return;
            StartCoroutine(MoveToPosition(waitingEnemyCards[0].transform, position, 0.2f));
            enemyCards.Add(waitingEnemyCards[0]);
            waitingEnemyCards.RemoveAt(0);
        }
    }
    #endregion

    #region Selection System
    public void OnPointerClick(PointerEventData eventData)
    {
        if (IsClickingEmptyArea(eventData))
            ClearSelection();
    }

    void OnCharacterClicked(Character character)
    {
        if (isAttackExecuting) return;

        switch (character)
        {
            case Soldier soldier when CombatManager.Instance.IsAlly(soldier):
                HandleAllySelection(soldier);
                break;
            case Enemy enemy when CombatManager.Instance.IsEnemy(enemy):
                HandleEnemySelection(enemy);
                break;
        }
        
        UpdateUnitInfoDisplay(character);
    }

    void HandleAllySelection(Soldier soldier)
    {
        if (soldier.IsDead() || soldier.AttackChances <= 0) return;
        
        selectedAlly = soldier;
        selectedEnemy = null;
        UpdateCombatLog($"Chosen {soldier.Name}");
    }

    void HandleEnemySelection(Character enemy)
    {
        if (selectedAlly == null || enemy.IsDead()) return;
        
        selectedEnemy = enemy;
        UpdateCombatLog($"Targeting {enemy.Name}");
    }

    void UpdateUnitInfoDisplay(Character character)
    {
        unitName.text = character.Name;
        if (character is Soldier soldier)
            unitRole.text = $"{soldier.GetRoleName()} Lv.{soldier.Level}";
    }

    void UpdateSelectionVisual()
    {
        Destroy(selectionFrame);
        
        var target = selectedEnemy ?? selectedAlly;
        if (target?.GameObject == null) return;

        selectionFrame = Instantiate(selectionFramePrefab, 
            target.GameObject.transform, 
            false);
    }
    #endregion

    #region Turn Management
    public void OnEndTurnButton() => StartCoroutine(EndTurnRoutine());

    IEnumerator EndTurnRoutine()
    {
        isAttackExecuting = true;
        UpdateCombatLog("Ending turn...");
        
        // yield return new WaitForSeconds(attackDelay);
        
        CombatManager.Instance.EndCurrentTurn();
        ResetAttackChances();
        ClearSelection();
        
        if (!CombatManager.Instance.IsPlayerTurn)
            yield return StartCoroutine(ExecuteEnemyTurn());
        
        isAttackExecuting = false;
        UpdateCombatState();
    }

    IEnumerator ExecuteEnemyTurn()
    {
        UpdateCombatLog("Enemy Turn Starts!");
        var enemies = GetActiveEnemies();

        foreach (var enemy in enemies)
        {
            if (enemy.IsDead()) continue;
            
            var target = CombatManager.Instance.GetRandomSoldier();
            if (target == null) continue;

            yield return StartCoroutine(ExecuteAttackRoutine(enemy, target));
        }
        UpdateCombatLog("Enemy Turn Ends!");
        ResetAttackChances();
        ClearSelection();
        OnEndTurnButton();
        CleanDeadUnits();
        UpdateCombatLog("Player's Turn Starts!");
    }

    List<Enemy> GetActiveEnemies()
    {
        var activeEnemies = new List<Enemy>();
        foreach (var enemy in CombatManager.Instance.GetAvailableEnemies())
        {
            if (!enemy.IsDead())
                activeEnemies.Add(enemy);
        }
        return activeEnemies;
    }

    void ResetAttackChances()
    {
        foreach (var soldier in CombatManager.Instance.GetAvailableSoldiers())
            soldier.ResetAttackChances();
        
        foreach (var enemy in CombatManager.Instance.GetAvailableEnemies())
            enemy.ResetAttackChances();
    }
    #endregion

    #region UI Update
    void UpdateCharacterUIStates()
    {
        foreach (Transform child in combatUnitContainer)
        {
            if (!child.TryGetComponent<CharacterUI>(out var ui)) continue;

            var character = ui.Character;
            ui.UpdateState(
                isSelected: IsSelected(character),
                isExhausted: IsExhausted(character),
                isAlly: CombatManager.Instance.IsAlly(character),
                isDead: character.IsDead()
            );
        }
    }

    bool IsSelected(Character character) => 
        character == selectedAlly || character == selectedEnemy;

    bool IsExhausted(Character character) =>
        character is Soldier soldier && soldier.AttackChances <= 0;

    void UpdateButtonStates()
    {
        attackButton.interactable = CanAttack() && !isAttackExecuting;
        endTurnButton.interactable = !isAttackExecuting;
        retreatButton.interactable = !isAttackExecuting && CombatManager.Instance.IsPlayerTurn;
        
        UpdateTurnDisplay();
    }

    void UpdateTurnDisplay()
    {
        turnText.text = CombatManager.Instance.IsPlayerTurn ? 
            "Player's Turn" : "Enemy's Turn";
        turnText.color = CombatManager.Instance.IsPlayerTurn ? 
            Color.white : Color.red;
    }

    bool CanAttack() => 
        selectedAlly != null && 
        selectedEnemy != null && 
        selectedAlly is Soldier { AttackChances: > 0 };
    #endregion

    #region Retreat System
    void OnRetreatButton()
    {
        DisableAllControls();
        ShowRetreatConfirmation();
    }

    void ShowRetreatConfirmation()
    {
        var confirmWindow = Instantiate(retreatConfirmationPrefab, transform);
        var canvasGroup = confirmWindow.AddComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = true;
        
        var retreatConfirm = confirmWindow.GetComponent<RetreatConfirmation>();
        retreatConfirm.Initialize(
            onConfirm: () => HandleRetreatConfirmed(retreatConfirm),
            onCancel: () => HandleRetreatCanceled(retreatConfirm)
        );
    }

    void HandleRetreatConfirmed(RetreatConfirmation window)
    {
        Destroy(window.gameObject);
        OnCombatEnd(false);
    }

    void HandleRetreatCanceled(RetreatConfirmation window)
    {
        Destroy(window.gameObject);
        EnableAllControls();
    }
    #endregion

    #region Helper Methods
    void InitializeCombatLog()
    {
        combatLog.text = "Combat Begins!";
        turnText.text = "Player's Turn";
        turnText.color = Color.white;
    }

    void SetupButtonListeners()
    {
        attackButton.onClick.AddListener(OnAttackButton);
        endTurnButton.onClick.AddListener(OnEndTurnButton);
        retreatButton.onClick.AddListener(OnRetreatButton);
    }

    bool IsClickingEmptyArea(PointerEventData eventData) => 
        eventData.pointerCurrentRaycast.gameObject == null;

    void UpdateCombatLog(string message) => 
        combatLog.text = message;

    void UpdateCombatState()
    {
        CheckTurnEnd();
        Update();
    }

    void CheckTurnEnd()
    {
        var allExhausted = true;
        foreach (var soldier in CombatManager.Instance.GetInBattleSoldiers())
        {
            if (soldier == null || soldier.IsDead()) continue;
            if (soldier.AttackChances > 0) allExhausted = false;
        }
        endTurnButton.image.color = allExhausted ? Color.red : Color.white;
    }

    void ClearSelection()
    {
        selectedAlly = null;
        selectedEnemy = null;
        Destroy(selectionFrame);
    }

    void DisableAllControls()
    {
        isAttackExecuting = true;
        SetControlsInteractable(false);
    }

    void EnableAllControls()
    {
        isAttackExecuting = false;
        SetControlsInteractable(true);
    }

    void SetControlsInteractable(bool state)
    {
        attackButton.interactable = state;
        endTurnButton.interactable = state;
        retreatButton.interactable = state;

        foreach (Transform child in combatUnitContainer)
        {
            if (child.TryGetComponent<Button>(out var button))
                button.interactable = state;
        }
    }
    #endregion

    #region Combat Events
    void OnCombatEnd(bool victory)
    {
        DisableAllControls();
        ShowEndMessage(victory);
        StartCoroutine(ReturnToBaseAfterDelay(3f));
    }

    void ShowEndMessage(bool victory)
    {
        combatLog.fontSize = 36;
        combatLog.color = victory ? Color.green : Color.red;
        combatLog.text = victory ? "Victory!" : "Defeated!";
    }

    IEnumerator ReturnToBaseAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        GameManager.Instance.LoadGameState(GameState.MissionPage);
    }
    #endregion
}