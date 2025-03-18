using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Assets.Scripts.Model;

namespace Assets.Scripts.Controller 
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        public GameState currentState;
        public Game currentGame;
        private Dictionary<int, int> tempResourceAmounts = new Dictionary<int, int>();

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);  
            }
            else
            {
                Destroy(gameObject);
            }
            //// Initialize combat manager to sleep state when game starts
            //if (CombatManager.Instance.gameObject.activeSelf)
            //{
            //    CombatManager.Instance.gameObject.SetActive(false);
            //}

            //// Initialize mission manager to sleep state when game starts
            //if (MissionManager.Instance.gameObject.activeSelf)
            //{
            //    MissionManager.Instance.gameObject.SetActive(false);
            //}
        }

        void Start()
        {
            if (SceneManager.GetActiveScene().name != "WelcomePage")
            {
                LoadGameState(GameState.WelcomePage);
            }
        }

        public void ChangeState(GameState newState)
        {
            Debug.Log("Game State changed to: " + newState);
        }

        public void LoadGameState(GameState newState)
        {
            currentState = newState;

            switch (newState)
            {
                case GameState.WelcomePage:
                    SceneManager.LoadScene("WelcomePage");
                    break;
                case GameState.MainMenuPage:
                    SceneManager.LoadScene("MainMenuPage");
                    break;
                case GameState.TechPage:
                    SceneManager.LoadScene("TechPage");
                    break;
                case GameState.MissionPage:
                    SceneManager.LoadScene("MissionPage");
                    break;
                case GameState.BasePage:
                    SceneManager.LoadScene("BasePage");
                    break;
                case GameState.StaffPage:
                    SceneManager.LoadScene("StaffPage");
                    break;
                case GameState.InventoryPage:
                    SceneManager.LoadScene("InventoryPage");
                    break;
                case GameState.CombatPage:
                    SceneManager.LoadScene("CombatPage");
                    break;
            }

            ChangeState(newState);
        }

        public void NewGame()
        {

        }
        public void LoadGame()
        {

        }

        public void SaveGame()
        {
            
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        public void ChangeTempResource(int resourceId, int changeAmount)
        {
            tempResourceAmounts[resourceId] = currentGame.resourcesData.GetAmount(resourceId);
            tempResourceAmounts[resourceId] += changeAmount;
            Debug.Log("Temporary value for resource " + resourceId + " is now " + tempResourceAmounts[resourceId]);
        }

        public void CancelResourceChanges()
        {
            tempResourceAmounts.Clear();
        }
        public void ConfirmResourceChanges()
        {
            currentGame.resourcesData.UpdateAllResources(tempResourceAmounts);
            tempResourceAmounts.Clear();
            Debug.Log("Resource changes confirmed");
        }
        
        public int GetResourceDisplayValue(int resourceId)
        {
            if (tempResourceAmounts.ContainsKey(resourceId))
            {
                return tempResourceAmounts[resourceId];
            }
            else
            {
                return currentGame.resourcesData.GetAmount(resourceId);
            }
        }
    }
}