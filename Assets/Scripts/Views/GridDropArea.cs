using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class GridDropArea : MonoBehaviour, IDropHandler
{
    public float occupancyThreshold = 0.1f;
    public float edgeMargin = 100f;

    // Reference to a TextMeshProUGUI component that displays drop messages.
    // If not assigned, one will be created automatically.
    public TextMeshProUGUI buildingInfoDisplay;

    void Awake()
    {
        // If no display is assigned, create one programmatically.
        if (buildingInfoDisplay == null)
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("No Canvas found in the scene to attach the buildingInfoDisplay!");
                return;
            }

            GameObject textObj = new GameObject("BuildingInfoDisplay", typeof(TextMeshProUGUI));
            textObj.transform.SetParent(canvas.transform, false);
            buildingInfoDisplay = textObj.GetComponent<TextMeshProUGUI>();

            // Set up the text properties.
            buildingInfoDisplay.fontSize = 25; // Big text
            buildingInfoDisplay.alignment = TextAlignmentOptions.Center;
            buildingInfoDisplay.text = ""; // Start with empty text
            buildingInfoDisplay.color = Color.white;

            // Configure the RectTransform to anchor at the bottom left.
            RectTransform rect = buildingInfoDisplay.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(0, 0);
            rect.pivot = new Vector2(0, 0);
            // Position it 50 pixels from the left and 50 pixels from the bottom.
            rect.anchoredPosition = new Vector2(190, -40);
            rect.sizeDelta = new Vector2(800, 150);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        DraggableBuilding draggable = eventData.pointerDrag.GetComponent<DraggableBuilding>();
        if (draggable == null)
            return;

        RectTransform gridRect = GetComponent<RectTransform>();
        RectTransform droppedRect = draggable.GetComponent<RectTransform>();

        Vector2 localPos;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                gridRect,
                eventData.position,
                eventData.pressEventCamera,
                out localPos))
        {
            draggable.ResetToInitialPosition();
            return;
        }

        float halfGridWidth = gridRect.rect.width * 0.5f;
        float halfGridHeight = gridRect.rect.height * 0.5f;
        float halfObjWidth = droppedRect.rect.width * 0.5f;
        float halfObjHeight = droppedRect.rect.height * 0.5f;

        float clampedX = Mathf.Clamp(localPos.x, -halfGridWidth + halfObjWidth - edgeMargin, halfGridWidth - halfObjWidth + edgeMargin);
        float clampedY = Mathf.Clamp(localPos.y, -halfGridHeight + halfObjHeight - edgeMargin, halfGridHeight - halfObjHeight + edgeMargin);
        Vector2 finalPos = new Vector2(clampedX, clampedY);

        bool overlap = false;
        foreach (Transform child in transform)
        {
            if (child == draggable.transform)
                continue;

            RectTransform childRect = child.GetComponent<RectTransform>();
            float buttonWidth = droppedRect.rect.width;
            float buttonHeight = droppedRect.rect.height;
            float thresholdWidth = occupancyThreshold * buttonWidth;
            float thresholdHeight = occupancyThreshold * buttonHeight;
            float xDiff = Mathf.Abs(childRect.anchoredPosition.x - finalPos.x);
            float yDiff = Mathf.Abs(childRect.anchoredPosition.y - finalPos.y);

            if (xDiff < thresholdWidth && yDiff < thresholdHeight)
            {
                overlap = true;
                break;
            }
        }

        if (overlap)
        {
            draggable.ResetToInitialPosition();
        }
        else
        {
            draggable.transform.SetParent(transform);
            droppedRect.anchoredPosition = finalPos;
            if (buildingInfoDisplay != null)
            {
                buildingInfoDisplay.text = "Building placed: " + draggable.buildingName;
            }
            else
            {
                Debug.LogError("buildingInfoDisplay is not assigned in the Inspector!");
            }
        }
    }
}
