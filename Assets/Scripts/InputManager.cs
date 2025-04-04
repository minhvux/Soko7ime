using UnityEngine;

public class InputManager : MonoBehaviour
{
    // Public references for the player and clone, assignable in the Inspector
    public PlayerController playerController;
    public PastIndicatorController pastIndicatorController;
    public FuturePastPlayerController futurePastPlayerController;

    void Update()
    {
        if (DataHub.Instance.isMoving) return;
        
        if (DataHub.Instance.isAlive && !GameManager.Instance.paradox){            
            if (Input.GetKeyDown(KeyCode.W))
            {
                DataHub.Instance.Move(Vector2.up);                   
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                DataHub.Instance.Move(Vector2.down);        
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                DataHub.Instance.Move(Vector2.left);
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                DataHub.Instance.Move(Vector2.right);
            }
            if (Input.GetKeyDown(KeyCode.Q) && DataHub.Instance.canRewind)
            {
                DataHub.Instance.DataHubRewind();
            }
            if (Input.GetKeyDown(KeyCode.E) && DataHub.Instance.canFuture)
            {
                playerController.ToggleFutureMode();
            }
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            DataHub.Instance.RevertAllToPreviousPositions(); 
        }    
    }
}
