using UnityEngine;

public class InputManager : MonoBehaviour
{
    // Public references for the player and clone, assignable in the Inspector
    public PlayerController playerController;
    public PastIndicatorController pastIndicatorController;
    public FuturePastPlayerController futurePastPlayerController;

    // Time interval between actions (e.g., 0.2 seconds)
    public float actionDelay = 0.2f;
    private float nextActionTime = 0f;

    

    void Update()
    {   
        if (Input.GetKeyDown(KeyCode.R))
        {
            GameManager.Instance.RestartLevel();
        }




        if (DataHub.Instance.isMoving) return;
        
        if (DataHub.Instance.isAlive && !GameManager.Instance.paradox)
        {
            // Only process the input if enough time has passed
            if (Time.time >= nextActionTime)
            {
                // Handle movement keys (W, A, S, D)
                if (Input.GetKey(KeyCode.W))
                {
                    DataHub.Instance.Move(Vector2.up);
                    nextActionTime = Time.time + actionDelay; // Set the time for the next allowed action
                } else
                if (Input.GetKey(KeyCode.S))
                {
                    DataHub.Instance.Move(Vector2.down);
                    nextActionTime = Time.time + actionDelay;
                } else
                if (Input.GetKey(KeyCode.A))
                {
                    DataHub.Instance.Move(Vector2.left);
                    nextActionTime = Time.time + actionDelay;
                } else
                if (Input.GetKey(KeyCode.D))
                {
                    DataHub.Instance.Move(Vector2.right);
                    nextActionTime = Time.time + actionDelay;
                } else

                // Handle Rewind (Q)
                if (Input.GetKeyDown(KeyCode.Q) && DataHub.Instance.canRewind && !DataHub.Instance.futureMode)
                {
                    DataHub.Instance.DataHubRewind();
                    nextActionTime = Time.time + actionDelay;
                } else

                // Handle Future Mode (E)
                if (Input.GetKeyDown(KeyCode.E) && DataHub.Instance.canFuture)
                {
                    DataHub.Instance.DataHubToggleFuture();
                    nextActionTime = Time.time + actionDelay;
                }
            }
        }

        // Handle reverting all positions (Z)
        if (Input.GetKey(KeyCode.Z))
        {   
            if (DataHub.Instance.futureMode) DataHub.Instance.SettleFuture();
            if (Time.time >= nextActionTime)
            {
                DataHub.Instance.RevertAllToPreviousPositions();
                nextActionTime = Time.time + actionDelay;
            }
        }
    }
}
