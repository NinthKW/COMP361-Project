using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Assets.Scripts.Model;
using Assets.Scripts.Controller;
using ModelResources = Assets.Scripts.Model.Resources;

public class ResourceDisplayUI : MonoBehaviour
{
    // Global access for instant refresh 
    public static ResourceDisplayUI Instance;

    [Header("Layout")]
    public Transform resourcesContainer;          // parent for resource rows
    public GameObject resourceListItemPrefab;     // row prefab with TMP/Text

    [Header("Header")]
    public TextMeshProUGUI resourceHeaderTextObject;
    public string resourcesHeaderText = "Resources";

    // Refresh timer  
    private float timer;
    private const float REFRESH_SEC = 1f;

    void Awake() => Instance = this;

    void Start()
    {
        if (resourceHeaderTextObject)
            resourceHeaderTextObject.text = resourcesHeaderText;

        PopulateResources();                       // initial fill
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= REFRESH_SEC)
        {
            timer = 0f;
            PopulateResources();                   // periodic update
        }
    }

    public void PopulateResources()
    {
        foreach (Transform c in resourcesContainer) Destroy(c.gameObject);

        ModelResources data = ResourceManager.Instance.GetResources();

        for (int id = 0; id <= 5; id++)            // 0–5 : Food..Medicine
        {
            string name   = data.GetName(id);
            int    amount = data.GetAmount(id);

            GameObject row = Instantiate(resourceListItemPrefab, resourcesContainer);
            var tmp = row.GetComponent<TextMeshProUGUI>();
            if (tmp) tmp.text = $"{name}: {amount}";
            else
            {
                var txt = row.GetComponent<Text>();
                if (txt) txt.text = $"{name}: {amount}";
            }
        }
    }
}
