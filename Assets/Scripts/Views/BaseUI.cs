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
        private RectTransform tableRect;
        public GameObject grid2d;

        public GameObject scroll;
        public Button backButton;
        public Button modeButton;
        public int mode = 0; //0 = use, 1 = edit

        public List<GameObject> buttonList = new List<GameObject>();

        void Start()
        {
            PopulateBuildingList();
            backButton.onClick.AddListener(OnBackButtonClicked);
            modeButton.onClick.AddListener(OnModeButtonClicked);

            //Make sure that it is in use mode
            mode = 0;
            foreach (GameObject button in buttonList)
            {
                button.GetComponent<DraggableBuilding>().enabled = false;

                if (button.GetComponent<DraggableBuilding>().building.placed == true)
                {
                    button.GetComponent<Button>().enabled = true;
                } else
                {
                    button.GetComponent <Button>().enabled = false;
                }
            }

            modeButton.GetComponentInChildren<TextMeshProUGUI>().text = "Enter Edit Mode";
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

                //Link each building with its functionality
                if (building.name.ToLower() == "hospital")
                {
                    btn.onClick.AddListener(BuildingFunctionality.hospitalFunctionality);
                }
                else if (building.name.ToLower() == "hq")
                {
                    btn.onClick.AddListener(BuildingFunctionality.hqFunctionality);
                }
                else if (building.name.ToLower() == "training room")
                {
                    btn.onClick.AddListener(BuildingFunctionality.trainingFunctionality);
                }
                else if (building.name.ToLower() == "loadout room")
                {
                    btn.onClick.AddListener(BuildingFunctionality.loadoutFunctionality);
                }


                    //Add to buttonsList
                    buttonList.Add(buttonObj);
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

        void OnModeButtonClicked()
        {
            Debug.Log("Base mode changed");

            ScrollRect scrollRect = scroll.GetComponent<ScrollRect>();
            if (scrollRect == null) {
                Debug.LogError("scroll rect not found");
            }


            if (mode == 1) { //change from edit to use
                mode = 0;
                foreach(GameObject button in buttonList)
                {
                    button.GetComponent<DraggableBuilding>().enabled = false;
                    
                    if (button.GetComponent<DraggableBuilding>().building.placed == true)
                    {
                        button.GetComponent<Button>().enabled = true;
                    }
                }

                modeButton.GetComponentInChildren<TextMeshProUGUI>().text = "Enter Edit Mode";
            }
            else if (mode == 0) { //from use to edit
                mode = 1;
                foreach (GameObject button in buttonList)
                {
                    button.GetComponent<DraggableBuilding>().enabled = true;
                    button.GetComponent<Button>().enabled = false;
                }


                modeButton.GetComponentInChildren<TextMeshProUGUI>().text = "Enter Use Mode";
            }
        }
    }
}