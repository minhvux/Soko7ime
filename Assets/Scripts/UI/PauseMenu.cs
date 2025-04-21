using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject PauseMenuUI;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale == 1f)
                Pause();
            else
                Resume();
        }
    }

    public void Pause()
    {
        Time.timeScale = 0f; // Pause the game
        PauseMenuUI.SetActive(true); // Show the pause menu UI
    }
    public void Resume()
    {
        Time.timeScale = 1f; // Resume the game
        PauseMenuUI.SetActive(false); // Hide the pause menu UI
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }

    public void Menu()
    {
        // Get the button that was pressed
        SceneManager.LoadScene(0);
        Time.timeScale = 1f;
    }
}
