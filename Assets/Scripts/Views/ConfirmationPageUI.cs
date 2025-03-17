using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Assets.Scripts.Model;
using Assets.Scripts.Controller;
using System;

public class RetreatConfirmation : MonoBehaviour 
{
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private TMP_Text messageText;

    public void Initialize(Action onConfirm, Action onCancel)
    {
        confirmButton.onClick.AddListener(() => onConfirm());
        cancelButton.onClick.AddListener(() => onCancel());
    }

    public void SetMessage(string message)
    {
        messageText.text = message;
    }
}