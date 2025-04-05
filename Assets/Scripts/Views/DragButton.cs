using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableBuilding : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // Building information fields.
    public int building_id;
    public string buildingName;
    public string description;
    public int level;
    public int cost;
    public int resource_amount;
    public int resource_type;
    public bool unlocked;

    private Transform originalParent;
    private Vector2 initialPosition; // Original anchored position.
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    // Stores the offset between the pointer and the object's pivot.
    private Vector2 pointerOffset;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        canvas = GetComponentInParent<Canvas>();
    }

    void Start()
    {
        initialPosition = rectTransform.anchoredPosition;
        originalParent = transform.parent;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Move the object to the canvas root to avoid clipping.
        transform.SetParent(canvas.transform);
        // Calculate the offset between the pointer and the object's pivot.
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform, eventData.position, eventData.pressEventCamera, out pointerOffset);
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform, eventData.position, eventData.pressEventCamera, out localPoint))
        {
            rectTransform.localPosition = localPoint - pointerOffset;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        LayoutElement layoutElement = GetComponent<LayoutElement>();
        layoutElement.ignoreLayout = true;

        // Reset anchors and pivot.
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);

        // Set both dimensions to 250.
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 250);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 250);
        
        // If not dropped into a valid target, reset position.
        if (transform.parent == canvas.transform)
        {
            layoutElement.ignoreLayout = false;
            ResetToInitialPosition();
        }
    }

    // Resets the building back to its original starting position and parent.
    public void ResetToInitialPosition()
    {
        transform.SetParent(originalParent);
        rectTransform.anchoredPosition = initialPosition;
    }
}
