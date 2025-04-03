// CharacterUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Assets.Scripts.Model;

public class CharacterUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Image characterImage;
    public Slider healthBar;
    public TMP_Text nameText;
    public TMP_Text attackChanceText;
    public GameObject exhaustedOverlay;

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

        UpdateVisuals(isAlly);
    }

    void UpdateVisuals(bool isAlly)
    {
        

        nameText.text = _character.Name;
        nameText.color = isAlly ? _allyTextColor : _enemyTextColor;
        characterImage.color = isAlly ? _allyImageColor : _enemyImageColor;
        UpdateState(false, false, isAlly, false);
    }

    public void UpdateState(bool isSelected, bool isExhausted, bool isAlly, bool isDead)
    {
        healthBar.value = (float)_character.Health / _character.MaxHealth;
        
        attackChanceText.text = $"{_character.AttackChances}/{_character.MaxAttacksPerTurn}";
        
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