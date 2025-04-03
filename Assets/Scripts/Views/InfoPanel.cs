using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using Assets.Scripts.Model;

public class InfoPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI attackText;
    [SerializeField] private TextMeshProUGUI defenseText;
    [SerializeField] private TextMeshProUGUI roleText;

    public void UpdateInfo(Character character)
    {
        nameText.text = character.Name;
        levelText.text = $"Level: {character.Level}";
        healthText.text = $"HP: {character.Health}/{character.MaxHealth}";
        attackText.text = $"ATK: {character.Atk}";
        defenseText.text = $"DEF: {character.Def}";
        if (character is Soldier) roleText.text = $"Role: {((Soldier)character).GetRoleName()}";
        else roleText.text = "Role: Enemy";
    }    
}
