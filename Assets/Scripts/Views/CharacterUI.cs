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

    void Start()
    {

    }
    public void Initialize(Character character, bool isAlly)
    {
        _character = character;
        UpdateVisuals(isAlly);
    }

    void UpdateVisuals(bool isAlly)
    {
        nameText.text = _character.Name;
        characterImage.color = isAlly ? Color.blue : Color.red;
        UpdateState(false, false, isAlly, false);
    }

    public void UpdateState(bool isSelected, bool isExhausted, bool isAlly, bool isDead)
    {
        healthBar.value = (float)_character.Health / _character.MaxHealth;
        
        attackChanceText.text = $"{_character.AttackChances}/{_character.MaxAttacksPerTurn}";
        
        exhaustedOverlay.SetActive(isExhausted);
        characterImage.color = isDead? Color.black : (isSelected ? Color.white : (isAlly ? Color.blue : Color.red));
    }
}