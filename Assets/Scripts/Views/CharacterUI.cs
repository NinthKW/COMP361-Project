// CharacterUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Assets.Scripts.Model;

public class CharacterUI : MonoBehaviour
{
    /// <summary>
    /// Represents a character's UI in the combat screen.
    /// </summary>
    [Header("UI Elements")]
    public Image characterImage;
    public Slider healthBar;
    public TMP_Text nameText;
    public TMP_Text attackChanceText;
    public TMP_Text atkText;
    public TMP_Text defText;
    public GameObject exhaustedOverlay;
    public GameObject buffPanel;

    private Character _character;

    public Character Character => _character;

    private Color _allyTextColor;
    private Color _enemyTextColor;
    private Color _allyImageColor;
    private Color _enemyImageColor;

    public void Initialize(Character character, bool isAlly)
    {
        _character = character;
        ColorUtility.TryParseHtmlString("#A0B6FF", out this._allyTextColor);
        ColorUtility.TryParseHtmlString("#FFA0A0", out this._enemyTextColor);
        ColorUtility.TryParseHtmlString("#A0B6FF", out this._allyImageColor);
        ColorUtility.TryParseHtmlString("#FFA0A0", out this._enemyImageColor);
        this._allyTextColor.a = 1;
        this._enemyTextColor.a = 1;
        this._allyImageColor.a = 1;
        this._enemyImageColor.a = 1;

        // Set a role-specific sprite for soldiers
    if (_character is Soldier soldier)
    {
        string roleName = soldier.GetRoleName(); // e.g., "Tank", "Medic", etc.
        // Load sprite from Resources/SoldierImages folder.
        Sprite roleSprite = UnityEngine.Resources.Load<Sprite>(roleName);
        if (roleSprite != null)
        {
            characterImage.sprite = roleSprite;
        }
        else
        {
            Debug.LogWarning("No sprite found for role: " + roleName);
        }
    }
        else if (_character is Enemy enemy)
    {
        // Use the enemy's name to try and load a specific sprite.
        string enemyName = enemy.Name; // Ensure your Enemy class has a Name property.
        // Adjust the path if your enemy images are in a subfolder, e.g., "EnemyImages/"
        Sprite enemySprite = UnityEngine.Resources.Load<Sprite>(enemyName);
        
        if (enemySprite != null)
        {
            characterImage.sprite = enemySprite;
        }
        else
        {
            // Fallback: load a default enemy sprite.
            enemySprite = UnityEngine.Resources.Load<Sprite>("enemydefault");
            if (enemySprite != null)
            {
                characterImage.sprite = enemySprite;
            }
            else
            {
                Debug.LogWarning("No enemy sprite found for enemy: " + enemyName);
            }
        }
    }

UpdateVisuals(isAlly);


        UpdateVisuals(isAlly);
    }

    void UpdateVisuals(bool isAlly)
    {
        nameText.text = _character.Name;
        nameText.color = isAlly ? _allyTextColor : _enemyTextColor;
        characterImage.color = isAlly ? _allyImageColor : _enemyImageColor;
        atkText.text = (_character.Atk.ToString()); 
        defText.text =(_character.Def.ToString());

        UpdateState(false, false, isAlly, false);
    }

    public void UpdateState(bool isSelected, bool isExhausted, bool isAlly, bool isDead)
    {
        healthBar.value = (float)_character.Health / (_character.MaxHealth + _character.Shield);
        healthBar.maxValue = 1f;
        healthBar.minValue = 0f;
        healthBar.fillRect.GetComponent<Image>().color = isAlly ? _allyImageColor : _enemyImageColor;
        healthBar.fillRect.GetComponent<Image>().color = new Color(healthBar.fillRect.GetComponent<Image>().color.r, healthBar.fillRect.GetComponent<Image>().color.g, healthBar.fillRect.GetComponent<Image>().color.b, 0.5f);
        
        attackChanceText.text = $"{_character.AttackChances}/{_character.MaxAttacksPerTurn}";
        foreach (Transform child in buffPanel.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (var buffPair in _character.Buffs)
        {
            if (buffPair.Value.IsExpired()) continue;
            GameObject buffTextObj = new("BuffText");
            buffTextObj.transform.SetParent(buffPanel.transform);
            buffTextObj.transform.localScale = Vector3.one;
            TextMeshProUGUI buffText = buffTextObj.AddComponent<TextMeshProUGUI>();
            // Assuming each buff has Name and Duration properties
            buffText.text = $"{buffPair.Value.Name} ({buffPair.Value.Duration} rounds)";
            buffText.fontSize = 18;
            buffText.color = isAlly ? _allyTextColor : _enemyTextColor;
            buffText.font = UnityEngine.Resources.Load<TMP_FontAsset>("Assets/TextMesh Pro/Examples & Extras/Resources/Fonts & Materials/Electronic Highway Sign SDF.asset"); // Load your font here
        }
        
        exhaustedOverlay.SetActive(isExhausted || isDead);
        // Get the base color based on whether it's an ally
        Color baseColor = isAlly ? _allyImageColor : _enemyImageColor;

        if (isSelected) {
            // Make the color brighter
            baseColor = new Color(
                Mathf.Min(baseColor.r * 1.2f, 1f),
                Mathf.Min(baseColor.g * 1.2f, 1f),
                Mathf.Min(baseColor.b * 1.2f, 1f),
                baseColor.a
            );
        }

        characterImage.color = baseColor;
    }
}