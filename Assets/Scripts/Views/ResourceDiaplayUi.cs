using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Model;
using Assets.Scripts.Controller;
using TMPro;
using ModelResources = Assets.Scripts.Model.Resources;

public class ResourceDisplayUI : MonoBehaviour
{
    // The container where resource items will be displayed 
    public Transform resourcesContainer;

    // The prefab used to display each resource item 
    public GameObject resourceListItemPrefab;
    
    public TextMeshProUGUI resourceHeaderTextObject;

    public string resourcesHeaderText = "Resources";
    private float time = 0f;
    private float interval = 1.0f;

    void Start()
    {
        // Update the header if you have one placed already
        if (resourceHeaderTextObject != null)
        {
            resourceHeaderTextObject.text = resourcesHeaderText;
        }
        PopulateResources();
    }

    private void Update()
    {
        time += Time.deltaTime;
        if (time > interval)
        {
            time = 0f;
            PopulateResources();
        }
    }

    void PopulateResources()
    {
        // Clear any existing children from the resources container
        foreach (Transform child in resourcesContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Retrieve the resource data using your ResourceManager
        ModelResources resData = ResourceManager.Instance.GetResources();
        
        // Iterate through resource IDs 0 to 5 (for Food, Money, Iron, Wood, Titanium, Healing)
        for (int id = 0; id <= 5; id++)
        {
            string resName = resData.GetName(id);
            int resAmount = resData.GetAmount(id);

            // Instantiate a new resource list item under the resources container
            GameObject newItem = Instantiate(resourceListItemPrefab, resourcesContainer);

            
            TextMeshProUGUI tmp = newItem.GetComponent<TextMeshProUGUI>();
            if (tmp != null)
            {
                tmp.text = $"{resName}: {resAmount}";
            }
            else
            {
                Text txt = newItem.GetComponent<Text>();
                if (txt != null)
                {
                    txt.text = $"{resName}: {resAmount}";
                }
            }
        }
    }
}
