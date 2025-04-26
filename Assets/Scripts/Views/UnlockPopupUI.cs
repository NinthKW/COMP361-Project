using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Assets.Scripts.Model;
using Assets.Scripts.Controller;

public class UnlockPopupUI : MonoBehaviour
{
    [SerializeField] TMP_Text itemNameText;
    [SerializeField] TMP_Text costText;
    [SerializeField] Button   unlockButton;
    [SerializeField] Button   cancelButton;

    Weapon    currentW;
    Equipment currentE;

    void Awake() => gameObject.SetActive(false);

    public void Show(Weapon w)    { currentW = w; currentE = null;  Setup(w.name, w.resource_type, w.resource_amount); }
    public void Show(Equipment e) { currentE = e; currentW = null;  Setup(e.name, e.resource_type, e.resource_amount); }

    void Setup(string name,int resId,int resAmt)
    {
        itemNameText.text = $"Unlock {name}?";
        costText.text = $"{ResourceManager.Instance.GetResourceName(resId)} x{resAmt}";
        unlockButton.onClick.RemoveAllListeners();
        unlockButton.onClick.AddListener(TryUnlock);
        cancelButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener(()=>gameObject.SetActive(false));
        gameObject.SetActive(true);
    }

    void TryUnlock()
    {
        bool ok = currentW != null
            ? InventoryManager.Instance.TryUnlock(currentW)
            : InventoryManager.Instance.TryUnlock(currentE);

        if (ok) gameObject.SetActive(false);
        else    costText.text += "\n<color=red>Not enough!</color>";
    }
}
