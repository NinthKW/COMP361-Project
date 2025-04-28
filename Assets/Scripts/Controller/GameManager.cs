using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Assets.Scripts.Model;
using System;
using Assets.Scripts.Utils;
using System.IO;

namespace Assets.Scripts.Controller 
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        public GameState currentState;
        
        [NonSerialized]
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

            if (!File.Exists(DBUtils.PersistentDBPath))
            {
                File.Copy(DBUtils.TemplateDBPath, DBUtils.PersistentDBPath);
            }

            // if (AudioManager.Instance == null)
            // {
            //     AudioManager.Instance = gameObject.AddComponent<AudioManager>();
            // }
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
            if (newState != GameState.WelcomePage) AudioManager.Instance.PlaySound("Select");
            AudioManager.Instance.StopAdditiveMusic();

            switch (newState)
            {
                case GameState.WelcomePage:
                    AudioManager.Instance.PlayMusic("Menu");
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
                    AudioManager.Instance.PlayMusic("Battle2");
                    SceneManager.LoadScene("CombatPage");
                    break;
                case GameState.MissionPreparationPage:
                    AudioManager.Instance.PlayMusic("Battle1");
                    SceneManager.LoadScene("MissionPrepPageUI");
                    break;
                case GameState.HospitalPage:
                    AudioManager.Instance.PlayAdditiveMusic("Inside Buildings");
                    SceneManager.LoadScene("HospitalPage");
                    break;
                case GameState.CombatResultPage:
                    SceneManager.LoadScene("CombatResultPage");
                    break;
                case GameState.TrainingPage:
                    AudioManager.Instance.PlayAdditiveMusic("Inside Buildings");
                    SceneManager.LoadScene("TrainingPage");
                    break;
                case GameState.LoadoutPage:
                    AudioManager.Instance.PlayAdditiveMusic("Inside Buildings");
                    SceneManager.LoadScene("LoadoutPage");
                    break;
            }

            ChangeState(newState);
        }

        public void NewGame()
        {
            if (File.Exists(DBUtils.PersistentDBPath))
            {
                File.Delete(DBUtils.PersistentDBPath);
            }
            
            File.Copy(DBUtils.TemplateDBPath, DBUtils.PersistentDBPath);

            currentGame = new Game("URI=file:" + DBUtils.PersistentDBPath);
            Debug.Log("New game started with default settings.");
        }

        public void LoadGame()
        {
            if (!File.Exists(DBUtils.PersistentDBPath))
            {
                File.Copy(DBUtils.TemplateDBPath, DBUtils.PersistentDBPath);
            }

            currentGame = new Game("URI=file:" + DBUtils.PersistentDBPath);
            Debug.Log("Game loaded from database.");
        }

        public void SaveGame()
        {
            if (currentGame != null)
            {
                currentGame.SaveGameData();
                Debug.Log("Game saved to database.");
            }
            else
            {
                Debug.LogWarning("No game instance available to save.");
            }   
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