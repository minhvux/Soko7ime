using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private Dictionary<GameObject, Stack<Vector2>> eventPositionHistory = new Dictionary<GameObject, Stack<Vector2>>();

    public List<GameObject> eventObjects;
    public GameObject dieObject; // Reference to the "Die" object in Canvas
    public GameObject player; // Reference to the player GameObject
    public bool isDead = false; // Public bool to check if the player is dead

    // LayerMask for Lava Layer
    public LayerMask lavaLayer;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // Initialize the event positions and their stacks
        foreach (var obj in eventObjects)
        {
            eventPositionHistory[obj] = new Stack<Vector2>();
            // Push the initial position to the stack
            eventPositionHistory[obj].Push(new Vector2(obj.transform.position.x, obj.transform.position.y));
        }

        // Ensure the "Die" object is hidden at the start
        if (dieObject != null)
        {
            dieObject.SetActive(false);
        }
    }

    public void Test ()
    {
        Debug.Log("Test");
    }
    public void CheckLava()
    {
        if (player != null)
        {
            // Use LayerMask to check if the player is standing on lava
            Collider2D lavaCollider = Physics2D.OverlapCircle(player.transform.position, 0.1f, lavaLayer);
            
            if (lavaCollider != null)
            {
                // Player is on lava, trigger death if not already dead
                if (!isDead) 
                {
                    PlayerDie();
                }
            }
            else
            {
                // Player is not on lava, trigger alive state if already dead
                if (isDead)
                {
                    PlayerAlive();
                }
            }
        }
    }

    private void PlayerDie()
    {
        if (!isDead) 
        {
            isDead = true;
            if (dieObject != null)
            {
                dieObject.SetActive(true);
            }
            Debug.Log("Player is dead!");
        }
    }

    private void PlayerAlive()
    {
        if (isDead) 
        {
            isDead = false;
            if (dieObject != null)
            {
                dieObject.SetActive(false);
            }
            Debug.Log("Player is alive!");
        }
    }

    public void RevertUpdate()
    {
        foreach (var obj in eventObjects)
        {
            Vector2 currentPosition = new Vector2(obj.transform.position.x, obj.transform.position.y);
            eventPositionHistory[obj].Push(currentPosition); // Store the new position in history
        }
    }

    public void RevertAllToPreviousPositions()
    {
        foreach (var obj in eventObjects)
        {
            RevertToPreviousPosition(obj);
        }
    }

    public void RevertToPreviousPosition(GameObject obj)
    {
        if (eventPositionHistory.ContainsKey(obj) && eventPositionHistory[obj].Count > 1)
        {
            eventPositionHistory[obj].Pop();
            Vector2 previousPosition = eventPositionHistory[obj].Peek();
            obj.transform.position = new Vector3(previousPosition.x, previousPosition.y, obj.transform.position.z);
        }
    }

    public void LevelComplete()
    {
        Debug.Log("Level Completed!");
        Invoke(nameof(LoadNextLevel), 2f); // Delay for effect
    }

    private void LoadNextLevel()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(nextSceneIndex);
        else
            Debug.Log("All levels completed! Show end game screen.");
    }
}
