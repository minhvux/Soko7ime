using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public bool paradox = false; 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            
        }
        else
        {
            Destroy(gameObject);
        }
    }

   

    // Call this function when the game is lost.
    public void Paradox()
    {
        Debug.Log("Paradox!");
        paradox = true;
        // Optionally add effects or delays here.
    }

    public void NoParadox()
    {
        Debug.Log("No Paradox!");
        paradox = false;
        // Optionally add effects or delays here.
    }

    // Call this function when the level is completed.
    public void LevelComplete()
    {
        Debug.Log("Level Complete!");
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            LoadLevel(nextSceneIndex);
        }
        else
        {
            LoadLevel(0);
            
        }
    }

    // Restart the current level.
    public void RestartLevel()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    // Load a specified level by scene index.
    public void LoadLevel(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
}
