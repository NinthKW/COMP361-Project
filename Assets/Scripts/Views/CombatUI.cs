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
    [SerializeField] private TextMeshProUGUI missionName;
    [SerializeField] private Button attackButton;
    [SerializeField] private Button endTurnButton;
    [SerializeField] private Button retreatButton;
    [SerializeField] private GameObject retreatConfirmationPrefab;
    [SerializeField] private TextMeshProUGUI enemyCountText;

    [SerializeField] private TextMeshProUGUI WeatherText;

    [SerializeField] private TextMeshProUGUI TerrainText;

    [Header("Ability Panel Settings")]
    [SerializeField] private GameObject abilityPanel; // Ability panel (pre-attached in scene)
    [SerializeField] private GameObject abilityButtonPrefab; // Ability button prefab
    [SerializeField] private GameObject abilityInfoPanel; // Ability info panel (pre-attached in scene)
    [SerializeField] private TextMeshProUGUI abilityNameText; // Text for ability name
    [SerializeField] private TextMeshProUGUI abilityDescriptionText; // Text for ability description
    [SerializeField] private TextMeshProUGUI abilityStatText; // Text for ability stats

    [Header("Position Settings")]
    [SerializeField] private List<Vector3> allyPositions = new();
    [SerializeField] private List<Vector3> enemyPositions = new();

    [Header("Combat Settings")]
    [SerializeField] private float attackDelay = 0.5f;
    #endregion

    #region Combat State
    private Character selectedAlly;
    private Character selectedTarget;
    private GameObject selectionFrame;
    private Ability selectedAbility = null;  // Currently selected ability
    private Character abilityTarget = null;    // Ability target (if target selection needed)
    private bool isAttackExecuting;
    private bool castable = false; // Flag to indicate if the ability is castable
    private readonly List<GameObject> soldierCards = new();
    private readonly List<GameObject> enemyCards = new();
    private readonly List<GameObject> waitingEnemyCards = new();
    #endregion

    #region Colors
    private Color healColor;
    private Color controlColor;
    private Color buffColor;
    private Color enemyColor;
    private Color allyColor;
    private Color transparentColor;
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
        CheckTurnEnd();
        UpdateButtonStates();
        UpdateSelectionVisual();
        UpdateEnemyCountDisplay();
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
        DisplayTerrainAndWeatherInfo();
        SetupButtonListeners();
        InitializeCombatLog();
        SetupColors();
    }

    void SetupColors()
    {
        ColorUtility.TryParseHtmlString("#A0B6FF", out allyColor);
        ColorUtility.TryParseHtmlString("#A0FFB6", out healColor);
        ColorUtility.TryParseHtmlString("#FFA500", out controlColor);
        ColorUtility.TryParseHtmlString("#A0B6FF", out buffColor);
        ColorUtility.TryParseHtmlString("#FFA0A0", out enemyColor);
        ColorUtility.TryParseHtmlString("#000000", out transparentColor);
        transparentColor.a = 0.01f;
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
        int midX = (Screen.width / 2) - 160;
        int midY = Screen.height / 2;
        
        // Adjust these values until the spacing feels right.
        float horizontalOffset = 50f;   // Increase this if soldiers are too close horizontally.
        float verticalSpacing = 50f;    // Increase this if soldiers are too close vertically.
        
        allyPositions = new List<Vector3>
        {
            new Vector3(midX - horizontalOffset, midY + verticalSpacing * 2, 0),
            new Vector3(midX + horizontalOffset, midY + verticalSpacing, 0),
            new Vector3(midX - horizontalOffset, midY, 0),
            new Vector3(midX + horizontalOffset, midY - verticalSpacing, 0),
            new Vector3(midX - horizontalOffset, midY - verticalSpacing * 2, 0)
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
        int midX = (Screen.width / 2) + 120;
        int midY = Screen.height / 2;
        
        // Adjust these values until the spacing feels right.
        float horizontalOffset = 50f;   // Increase if enemies are too close horizontally.
        float verticalSpacing = 50f;    // Increase if enemies are too close vertically.
        
        enemyPositions = new List<Vector3>
        {
            new Vector3(midX - horizontalOffset, midY + verticalSpacing * 2, 0),
            new Vector3(midX + horizontalOffset, midY + verticalSpacing, 0),
            new Vector3(midX - horizontalOffset, midY, 0),
            new Vector3(midX + horizontalOffset, midY - verticalSpacing, 0),
            new Vector3(midX - horizontalOffset, midY - verticalSpacing * 2, 0),
            new Vector3(midX + horizontalOffset * 2, midY, 0)
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

        // Update enemy count display after positioning cards.
        UpdateEnemyCountDisplay();
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

    void DisplayTerrainAndWeatherInfo()
    {
        if (CombatManager.Instance.currentMission == null)
        {
            TerrainText.text = "Terrain: Unknown";
            WeatherText.text = "Weather: Unknown";
            return;
        }

        var mission = CombatManager.Instance.currentMission;

        // 更新 Terrain 信息
        TerrainText.text = $"Terrain: {mission.terrain}\n" +
                        $"ATK Effect: {mission.terrainAtkEffect}\n" +
                        $"DEF Effect: {mission.terrainDefEffect}\n" +
                        $"HP Effect: {mission.terrainHpEffect}";

        // 更新 Weather 信息
        WeatherText.text = $"Weather: {mission.weather}\n" +
                        $"ATK Effect: {mission.weatherAtkEffect}\n" +
                        $"DEF Effect: {mission.weatherDefEffect}\n" +
                        $"HP Effect: {mission.weatherHpEffect}";
    }

    #endregion

    #region Combat Logic
    public void OnAttackButton()
    {
        if (selectedAbility != null)
        {
            // In skill mode: Check if enough action points (using AttackChances)
            if (selectedAlly == null)
            {
                UpdateCombatLog("No casting unit selected.");
                return;
            }
            if (selectedAbility.Cost > selectedAlly.AttackChances)
            {
                UpdateCombatLog("Not enough action points to cast this ability.");
                return;
            }
            // If the ability requires a target (Heal and Buff) then a target must be selected
            if ((CompareAbility(selectedAbility, "Heal") || 
                CompareAbility(selectedAbility, "Buff") || 
                CompareAbility(selectedAbility, "HealBuff")) && 
                abilityTarget == null)
            {
                UpdateCombatLog("Please select a valid target first.");
                return;
            }
                        
            List<Character> targets = new List<Character>();
            if (selectedAbility is TauntAbility)
            {
                // Taunt ability defaults to targeting the caster itself
                targets.Add(selectedAlly);
            }
            else
            {
                targets.Add(abilityTarget);
            }
            
            // Execute ability logic
            AudioManager.Instance.PlaySound(selectedAbility.Name);
            if (!selectedAbility.Activate(targets)) Debug.LogError("Ability activation failed.");
            else 
            {
                selectedAlly.AttackChances -= selectedAbility.Cost;
                UpdateCombatLog($"{selectedAlly.Name} cast {selectedAbility.Name}!");
            }
            // Clear skill mode state and restore the Attack button display
            selectedAbility = null;
            abilityTarget = null;
            castable = false; // Reset castable state after attack
            attackButton.GetComponentInChildren<TextMeshProUGUI>().text = "Attack";
            endTurnButton.GetComponentInChildren<TextMeshProUGUI>().text = "End Turn";
            attackButton.image.color = Color.gray;
            HideAbilityPanel();
            abilityInfoPanel.SetActive(false);
            PostAttackCleanup();
            return;
        }
        else if (CanAttack()) 
        {
            StartCoroutine(ExecuteAttackRoutine(selectedAlly, selectedTarget));
            HideAbilityPanel();
            abilityInfoPanel.SetActive(false);
        }
    }

    private void OnAbilityButtonClicked(Ability ability)
    {
        selectedAbility = ability;
        // Update Attack button style and prompt based on ability type
        if (CompareAbility(ability, "Heal"))
        {
            attackButton.GetComponentInChildren<TextMeshProUGUI>().text = "Heal";
            endTurnButton.GetComponentInChildren<TextMeshProUGUI>().text = "Cancel";
            attackButton.image.color = Color.green;
            UpdateCombatLog("Please select an injured ally for healing.");
            castable = false;
        }
        else if (CompareAbility(ability, "HealBuff"))
        {
            attackButton.GetComponentInChildren<TextMeshProUGUI>().text = "HealBuff";
            endTurnButton.GetComponentInChildren<TextMeshProUGUI>().text = "Cancel";
            attackButton.image.color = Color.green;
            UpdateCombatLog("Please select any ally for applying permanent heal buff.");
            castable = false;
        }
        else if (CompareAbility(ability, "TauntAll")) // Taunt
        {
            attackButton.GetComponentInChildren<TextMeshProUGUI>().text = "Taunt";
            selectedTarget = null; // Clear target selection
            attackButton.image.color = new Color(1f, 0.5f, 0f); // Orange
            UpdateCombatLog("Please click to confirm casting taunt ability.");
            castable = true;
        }
        else if (CompareAbility(ability, "Buff") || CompareAbility(ability, "Shield"))
        {
            attackButton.GetComponentInChildren<TextMeshProUGUI>().text = "Buff";
            attackButton.image.color = Color.cyan;
            UpdateCombatLog("Please select an ally for buffing.");
            castable = false;
        }
        else if (CompareAbility(ability, "Damage") || CompareAbility(ability, "Lifesteal"))
        {
            attackButton.GetComponentInChildren<TextMeshProUGUI>().text = "Attack!";
            attackButton.image.color = Color.red;
            UpdateCombatLog("Please select an enemy for attacking.");
            castable = false;
        }
        else
        {
            attackButton.GetComponentInChildren<TextMeshProUGUI>().text = "Attack";
            attackButton.image.color = Color.gray;
            castable = false; // Reset castable state if not a valid ability
        }
        
        // Enter skill casting mode; waiting for target selection (if necessary) or direct confirmation
        UpdateAbilityInfo(ability);
    }

    IEnumerator ExecuteAttackRoutine(Character attacker, Character target)
    {
        isAttackExecuting = true;
        UpdateCombatLog($"{attacker.Name} attacks {target.Name}!");
        
        attacker.AttackChances--;
        AudioManager.Instance.PlaySound("Attack");

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

        // Charge animation
        yield return MoveToPosition(attacker.GameObject.transform, 
            targetPos - new Vector3(1, 0, 0), 0.2f);

        ShowDamageText(target, damage);
        CombatManager.Instance.ProcessAttack(attacker, target);

        // Return animation
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

        // Calculate effective damage considering defense
        int reducedDamage = Mathf.Max(0, damage - target.Def);

        if (reducedDamage <= 0) reducedDamage = 1; // Ensure minimum damage of 1

        Debug.Log($"Damage: {damage}, Defense: {target.Def}, Reduced Damage: {reducedDamage}");

        // Create Damage Text
        GameObject damageTextObj = new GameObject("DamageText");
        damageTextObj.transform.SetParent(canvas.transform, false);

        TextMeshProUGUI textMesh = damageTextObj.AddComponent<TextMeshProUGUI>();
        textMesh.text = $"-{reducedDamage} HP";
        textMesh.fontSize = 36;
        textMesh.color = Color.red;
        textMesh.alignment = TextAlignmentOptions.Center;
        textMesh.raycastTarget = false;

        // Set position to above the target (converted from world to screen coordinates)
        Vector3 screenPosition = (target.GameObject.transform.position + new Vector3(0, 2f, 0));
        damageTextObj.transform.position = screenPosition;

        // Start fade-out coroutine
        StartCoroutine(FadeAndDestroyText(textMesh));
    }

    private IEnumerator FadeAndDestroyText(TextMeshProUGUI textMesh)
    {
        float duration = 2f;
        Color originalColor = textMesh.color;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            // Gradually move the text upward
            textMesh.transform.Translate(Vector3.up * Time.deltaTime * 20f);

            // Adjust transparency
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
        // 更新敌人数量显示
        UpdateEnemyCountDisplay();
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
        bool anyEnemyRemoved = false; // 用于检查是否有敌人被移除

        foreach (var card in enemyCards.ToArray())
        {
            if (card == null) continue;
            
            var enemy = card.GetComponent<CharacterUI>().Character;
            if (!enemy.IsDead()) continue;

            var position = card.transform.position;
            enemyCards.Remove(card);
            Destroy(card);
            anyEnemyRemoved = true; // 标记有敌人被移除
            
            // TODO: replace dead units in new logics
            if (waitingEnemyCards.Count > 0)
            {
                StartCoroutine(MoveToPosition(waitingEnemyCards[0].transform, position, 0.2f));
                enemyCards.Add(waitingEnemyCards[0]);
                waitingEnemyCards.RemoveAt(0);
            }
        }

        if (anyEnemyRemoved)
        {
            UpdateEnemyCountDisplay(); // 在清除敌人后更新显示
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
        if (character.IsDead()) return;

        AudioManager.Instance.PlaySound("Select");
        // If in skill casting mode
        if (selectedAbility != null)
        {
            // For Heal and Buff abilities, require selecting an ally target
            if ((CompareAbility(selectedAbility, "Heal") || 
                CompareAbility(selectedAbility, "Buff") || 
                CompareAbility(selectedAbility, "HealBuff") ||
                CompareAbility(selectedAbility, "Shield")) &&
                CombatManager.Instance.IsAlly(character) && !character.IsDead())
            {
                // For Heal ability, only allow selection if the target is injured
                if (CompareAbility(selectedAbility, "Heal"))
                {
                    Soldier soldierTarget = character as Soldier;
                    if (soldierTarget.Health < soldierTarget.MaxHealth)
                    {
                        abilityTarget = character;
                        UpdateCombatLog($"Selected {character.Name} as healing target.");
                    }
                    else
                    {
                        UpdateCombatLog($"{character.Name} is not injured.");
                    }
                }
                else // Buff ability
                {
                    abilityTarget = character;
                    UpdateCombatLog($"Selected {character.Name} as {selectedAbility.Type} target.");
                }
                castable = true; // Set castable state to true
                UpdateCharacterUIStates();
            } else if ((CompareAbility(selectedAbility, "Damage") || 
                CompareAbility(selectedAbility, "Lifesteal")) && 
                CombatManager.Instance.IsEnemy(character) && !character.IsDead())
            {
                abilityTarget = character;
                UpdateCombatLog($"Selected {character.Name} as attack target.");
                castable = true; // Set castable state to true
                UpdateCharacterUIStates();
            }
            else
            {
                selectedAbility = null;
                abilityTarget = null;
                castable = false; // Reset castable state after target selection
                endTurnButton.GetComponentInChildren<TextMeshProUGUI>().text = "End Turn";
                attackButton.GetComponentInChildren<TextMeshProUGUI>().text = "Attack";
                attackButton.image.color = Color.gray;
                HideAbilityPanel();
            }
            // For Taunt ability, no target selection is needed; just wait for confirmation
            return;
        }

        // Normal selection mode
        switch (character)
        {
            case Soldier soldier when CombatManager.Instance.IsAlly(soldier):
                HandleAllySelection(soldier);
                break;
            case Enemy enemy when CombatManager.Instance.IsEnemy(enemy):
                HandleTargetSelection(enemy);
                break;
        }
        
        UpdateUnitInfoDisplay(character);
    }

    void HandleAllySelection(Soldier soldier)
    {
        if (soldier.IsDead() || soldier.AttackChances <= 0) return;
        
        selectedAlly = soldier;
        selectedTarget = null;
        UpdateCombatLog($"Chosen {soldier.Name}");
        
        // Show the soldier's ability buttons
        ShowAbilityPanel(soldier);
    }

    void HandleTargetSelection(Character target)
    {
        if (selectedAlly == null || target.IsDead()) return;
        
        selectedTarget = target;
        UpdateCombatLog($"Targeting {target.Name}");
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
        
        var target = selectedTarget ?? selectedAlly;
        if (target?.GameObject == null) return;

        selectionFrame = Instantiate(selectionFramePrefab, 
            target.GameObject.transform, 
            false);
    }
    #endregion

    #region Turn Management
    public void OnEndTurnButton() 
    {
        if (castable) 
        {
            selectedAbility = null;
            abilityTarget = null;
            castable = false; // Reset castable state after target selection
            attackButton.GetComponentInChildren<TextMeshProUGUI>().text = "Attack";
            endTurnButton.GetComponentInChildren<TextMeshProUGUI>().text = "End Turn";
            attackButton.image.color = Color.gray;
            HideAbilityPanel();
            abilityInfoPanel.SetActive(false);
            return;
        }
        else {
            StartCoroutine(EndTurnRoutine());
        }
    }

    IEnumerator EndTurnRoutine()
    {
        isAttackExecuting = true;
        UpdateCombatLog("Ending turn...");
        
        CombatManager.Instance.EndCurrentTurn();
        ClearSelection();
        
        if (!CombatManager.Instance.IsPlayerTurn)
            yield return StartCoroutine(ExecuteEnemyTurn());
        
        isAttackExecuting = false;
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
    private void ShowAbilityPanel(Soldier soldier)
    {
        // Clear existing buttons
        foreach (Transform child in abilityPanel.transform)
        {
            Destroy(child.gameObject);
        }
        
        // Create a button for each ability of the soldier
        foreach (var ability in soldier.Abilities)
        {
            var abilityButton = Instantiate(abilityButtonPrefab, abilityPanel.transform);
            var btnText = abilityButton.GetComponentInChildren<TextMeshProUGUI>();
            btnText.text = $"{ability.Name}";
            ColorUtility.TryParseHtmlString("#A0B6FF", out var color);
            color.a = 1f;
            btnText.color = color;
            Button btn = abilityButton.GetComponent<Button>();
            btn.interactable = true;
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => OnAbilityButtonClicked(ability));
        }
        
        Debug.Log($"Showing ability panel for {soldier.Name}");
        abilityPanel.SetActive(true);
    }

    private void UpdateAbilityInfo(Ability ability)
    {
        if (ability == null)
        {
            abilityInfoPanel.SetActive(false);
            return;
        }
        abilityInfoPanel.SetActive(true);
        abilityNameText.text = ability.Name;
        abilityDescriptionText.text = ability.Description;
        abilityStatText.text = $"Cost: {ability.Cost} \n Cooldown: {ability.Cooldown} \n Duration: {ability.Duration} \n Type: {ability.Type}";

        // Append additional numeric stats if they exist on the ability
        var type = ability.GetType();
        var statProperties = new string[] { "Damage", "HealAmount", "Range", "Multiplier" };
        foreach (var stat in statProperties)
        {
            var prop = type.GetProperty(stat);
            if (prop != null)
            {
                var value = prop.GetValue(ability);
                abilityStatText.text += $"\n{stat}: {value}";
            }
        }
        var color = Color.white;
        if (CompareAbility(ability, "Heal") || CompareAbility(ability, "HealBuff")) color = healColor;
        else if (CompareAbility(ability, "TauntAll")) color = controlColor;
        else if (CompareAbility(ability, "Buff") || CompareAbility(ability, "Shield")) color = buffColor;
        else if (CompareAbility(ability, "Enemy")) color = enemyColor;
        abilityNameText.color = color;
        abilityDescriptionText.color = color;
        abilityStatText.color = color;
    }

    private void HideAbilityPanel()
    {
        foreach (Transform child in abilityPanel.transform)
        {
            Destroy(child.gameObject);
        }
        abilityPanel.SetActive(false);
    }

    bool IsSelected(Character character) => 
        character == selectedAlly || character == selectedTarget || character == abilityTarget;

    bool IsExhausted(Character character) =>
        character is Soldier soldier && soldier.AttackChances <= 0;

    void UpdateButtonStates()
    {
        attackButton.interactable = (CanAttack() && !isAttackExecuting) || castable;
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
        selectedTarget != null && 
        selectedAlly is Soldier { AttackChances: > 0 };

    private void UpdateEnemyCountDisplay()
    {
        if (enemyCountText == null) return;

        int activeEnemies = CombatManager.Instance.CountAliveEnemies();
        int waitingEnemies = CombatManager.Instance.GetWaitingEnemies().Count;

        enemyCountText.text = $"Enemies Remaining: {activeEnemies + waitingEnemies}";
    }
    
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
        missionName.text = CombatManager.Instance.currentMission.name;
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
        endTurnButton.image.color = allExhausted ? Color.green : transparentColor;
    }

    void ClearSelection()
    {
        selectedAlly = null;
        selectedTarget = null;
        Destroy(selectionFrame);
    }

    void DisableAllControls()
    {
        isAttackExecuting = true;
        SetControlsInteractable(false);
        turnText.enabled = false;
        combatLog.enabled = false;
        unitName.enabled = false;
        unitRole.enabled = false;
        missionName.enabled = false;
        abilityNameText.enabled = false;
        abilityDescriptionText.enabled = false;
        abilityStatText.enabled = false;
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

    

    bool CompareAbility(Ability ability, string type)
    {
        return ability.Type.Equals(type, System.StringComparison.OrdinalIgnoreCase);
    }
    #endregion

    #region Combat Events
    void OnCombatEnd(bool victory)
    {
        DisableAllControls();
        ShowEndMessage(victory);
        AudioManager.Instance.PlayMusic("Menu");
        if (victory == false){
            CombatManager.Instance.SaveCombatResults(victory, "");
        }
        StartCoroutine(ReturnToBaseAfterDelay(1.0f));
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
        GameManager.Instance.LoadGameState(GameState.CombatResultPage);
    }
    #endregion
}
