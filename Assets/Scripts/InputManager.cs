using UnityEngine;

public class InputManager : MonoBehaviour
{
    // Public references for the player and clone, assignable in the Inspector
    public PlayerController playerController;
    public PastIndicatorController pastIndicatorController;

    void Update()
    {
        if (playerController.isMoving) return;
        
        if (DataHub.Instance.isAlive == true){            
            if (Input.GetKeyDown(KeyCode.W))
            {
                playerController.TryToMove(Vector2.up);                      
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                playerController.TryToMove(Vector2.down);              
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                playerController.TryToMove(Vector2.left);   
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                playerController.TryToMove(Vector2.right);     
            }
            if (Input.GetKeyDown(KeyCode.Q) && DataHub.Instance.canRewind)
            {
                pastIndicatorController.Rewind();
            }
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            DataHub.Instance.RevertAllToPreviousPositions(); 
        }    
    }
}
