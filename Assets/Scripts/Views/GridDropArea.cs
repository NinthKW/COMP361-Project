// Assets/Scripts/GridDropArea.cs
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

namespace Assets.Scripts
{
    public class GridDropArea : MonoBehaviour, IDropHandler
    {
        [Header("Bounds Settings")]
        public float padding = 10f;       // space from the very edge
        public bool freeDrop = false;     // if true, skips clamping

        public TextMeshProUGUI buildingInfoDisplay;

        private Canvas parentCanvas;
        private Camera canvasCamera;
        public Transform grid;

        void Awake()
        {
            parentCanvas = GetComponentInParent<Canvas>();
            if (parentCanvas == null)
            {
                Debug.LogError("No Canvas found in parent hierarchy!");
                return;
            }
            canvasCamera = parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay
                ? null
                : parentCanvas.worldCamera;

        }

        public void OnDrop(PointerEventData eventData)
        {
            var draggable = eventData.pointerDrag?.GetComponent<DraggableBuilding>();
            if (draggable == null) return;

            var gridRect = GetComponent<RectTransform>();
            var droppedRect = draggable.GetComponent<RectTransform>();

            // 1) get drop point in grid‑local coords
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    gridRect,
                    eventData.position,
                    canvasCamera,
                    out Vector2 localPos))
            {
                draggable.ResetToInitialPosition();
                return;
            } else
            {
                draggable.transform.SetParent(grid, false);
                return;
            }
        }
    }
}
