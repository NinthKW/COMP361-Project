using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class DraggableBuilding : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public int building_id;
        public string buildingName;
        public string description;
        public int level;
        public int cost;
        public int resource_amount;
        public int resource_type;
        public bool unlocked;

        public Model.Base building;
        public GameObject grid2d;

        private Transform originalParent;
        private Vector2 initialPosition;
        private Canvas canvas;
        private RectTransform rectTransform;
        private CanvasGroup canvasGroup;

        [HideInInspector] public Vector2 pointerOffset;

        void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
            canvas = GetComponentInParent<Canvas>();

            grid2d = GameObject.Find("2dGrid");
            if (grid2d == null)
            {
                Debug.LogError("2d grid not attached to button");
            }
        }

        void Start()
        {
            initialPosition = rectTransform.anchoredPosition;
            originalParent = transform.parent;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            transform.SetParent(canvas.transform);
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
            if (layoutElement != null)
            {
                layoutElement.ignoreLayout = true;
            }

            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);

            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 150);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 150);

            if (transform.parent == canvas.transform)
            {
                building.placed = false;
                building.x = 0;
                building.y = 0;
                ResetToInitialPosition();
            } else if (transform.parent == grid2d.transform) {
                building.placed = true;
                building.x = (int) System.Math.Round(GetComponent<RectTransform>().anchoredPosition.x);
                building.y = (int) System.Math.Round(GetComponent<RectTransform>().anchoredPosition.y);

                // --- Option 2: Get Anchored Position (matches Inspector Pos X/Y) ---
                Debug.Log("Anchored X pos (Inspector Pos X): " + GetComponent<RectTransform>().anchoredPosition.x);
                Debug.Log("Anchored Y pos (Inspector Pos Y): " + GetComponent<RectTransform>().anchoredPosition.y);

            }
        }

        public void ResetToInitialPosition()
        {
            transform.SetParent(originalParent);
            rectTransform.anchoredPosition = initialPosition;
        }
    }
}