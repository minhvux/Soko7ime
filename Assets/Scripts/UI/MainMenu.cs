using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;


public class MainMenu : MonoBehaviour
{
   public void QuitGame()
   {
       Debug.Log("Quitting game...");
       Application.Quit();
   }

   public void SelectLevel()
    {
        // Get the button that was pressed
        GameObject selectedButton = EventSystem.current.currentSelectedGameObject;
        if (selectedButton != null)
        {
            // Use the button's name as the scene name
            string levelName = selectedButton.name;
            Debug.Log("Loading level: " + levelName);
            SceneManager.LoadScene(levelName);
        }
        else
        {
            Debug.LogWarning("No button selected for level loading.");
        }
    }
}
