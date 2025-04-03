using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableBuilding : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
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

        // Ensure there's a CanvasGroup attached.
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // Get the parent Canvas for proper scaling.
        canvas = GetComponentInParent<Canvas>();
    }

    void Start()
    {
        // Save the initial position and parent at the start.
        initialPosition = rectTransform.anchoredPosition;
        originalParent = transform.parent;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //Change anchor point
        //LayoutElement layoutElement = GetComponent<LayoutElement>();
        //layoutElement.ignoreLayout = true;

        //rectTransform.anchorMax = new Vector2(1, 1);
        //rectTransform.anchorMin = new Vector2(0, 0);
        //rectTransform.pivot = new Vector2(0.5f, 0.5f);

        //rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 250);
        //rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 250);

        // Calculate the offset between where the pointer is and the object's pivot.
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform, eventData.position, eventData.pressEventCamera, out pointerOffset);
        
        // Move the object to the canvas root to avoid clipping.
        transform.SetParent(canvas.transform);
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPoint;
        // Convert the screen point of the cursor to a local point in the canvas's coordinate space.
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform, eventData.position, eventData.pressEventCamera, out localPoint))
        {
            // Set the new position adjusted by the initial pointer offset.
            rectTransform.localPosition = localPoint - pointerOffset;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        LayoutElement layoutElement = GetComponent<LayoutElement>();
        layoutElement.ignoreLayout = true;

        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);

        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 250);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 250);
        // If the object hasn't been reparented to a valid drop target, return it to its starting position.
        if (transform.parent == canvas.transform)
        {
            layoutElement.ignoreLayout = false;
            ResetToInitialPosition();
        }
    }

    // Resets the building back to the original starting position and parent.
    public void ResetToInitialPosition()
    {
        transform.SetParent(originalParent);
        rectTransform.anchoredPosition = initialPosition;
    }
}