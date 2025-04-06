using Assets.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Assets.Scripts.Controller;
using UnityEngine.UI;
using Codice.CM.Common.Checkin.Partial.ConflictCheckers;

namespace Assets.Scripts
{
    public class BaseUI : MonoBehaviour
    {
        public Transform missionButtonContainer;
        public GameObject missionButtonPrefab;
        private Base selectedBuilding;
        public Button backButton;
        private RectTransform tableRect;
        public GameObject grid2d;

        void Start()
        {
            PopulateBuildingList();
            backButton.onClick.AddListener(OnBackButtonClicked);
        }

        void Update()
        {
        }

        void PopulateBuildingList()
        {
            Debug.Log("PopulateBuildingList called");
            if (BaseManager.Instance == null)
            {
                Debug.LogError("BaseManager.Instance is NULL");
                return;
            }
            if (BaseManager.Instance.buildingList == null)
            {
                Debug.LogError("building list is NULL");
                return;
            }

            Debug.Log("Building count: " + BaseManager.Instance.buildingList.Count);
            foreach (var building in BaseManager.Instance.buildingList)
            {
                Debug.Log("Adding building: " + building.name);
                GameObject buttonObj = Instantiate(missionButtonPrefab, missionButtonContainer);

                // Increase the button's height (doubling it in this example)
                RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
                buttonRect.sizeDelta = new Vector2(buttonRect.sizeDelta.x, buttonRect.sizeDelta.y * 2);

                LayoutElement layoutElement = buttonObj.GetComponent<LayoutElement>();
                if (layoutElement != null)
                {
                    layoutElement.preferredHeight = buttonRect.sizeDelta.y;
                }

                Sprite buttonTexture = UnityEngine.Resources.Load<Sprite>("base_" + building.name.ToLower());
                buttonObj.GetComponent<Image>().sprite = buttonTexture;
                if (buttonTexture == null)
                {
                    Debug.LogError("Sprite not found: " + "base_" + building.name.ToLower());
                }

                TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
                buttonText.text = building.name;

                // Assign the building info to the DraggableBuilding component.
                DraggableBuilding draggable = buttonObj.GetComponent<DraggableBuilding>();
                if (draggable != null)
                {
                    draggable.building = building;
                }

                Button btn = buttonObj.GetComponent<Button>();
                btn.onClick.AddListener(() => OnSelectedBuilding(building));
                buttonObj.SetActive(true);

                //If building should be placed, add it to grid2d
                if (building.placed)
                {
                    buttonObj.transform.parent = grid2d.transform;
                    RectTransform rectTransform = buttonObj.transform.GetComponent<RectTransform>();

                    rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                    rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                    rectTransform.pivot = new Vector2(0.5f, 0.5f);

                    rectTransform.anchoredPosition = new Vector2((float) building.x, (float) building.y);
                }
            }

            GridLayoutGroup grid = missionButtonContainer.GetComponent<GridLayoutGroup>();
            if (grid != null)
            {
                grid.cellSize = new Vector2(grid.cellSize.x, grid.cellSize.y * 2);
            }
        }

        void OnSelectedBuilding(Base building)
        {
            selectedBuilding = building;
        }

        void OnBackButtonClicked()
        {
            GameManager.Instance.ChangeState(GameState.MainMenuPage);
            GameManager.Instance.LoadGameState(GameState.MainMenuPage);
        }
    }
}