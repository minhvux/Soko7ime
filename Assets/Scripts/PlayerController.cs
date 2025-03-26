using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 1f;
    private Vector2 moveDirection;

    public int rewindSteps = 7; // Number of steps to rewind
    private int rewindIndex = 0;
    public bool rewinded = false;
    private int rewindedIndex = 0;
    private int FutureTravelIndex = 0;

    public bool turnOffRewind = false;

    public GameObject indicatorPrefab; // Assign in the Inspector
    private List<Vector2> positionHistory = new List<Vector2>();
    private GameObject indicator;

    // LayerMasks for Block, Wall, and Goal layers
    public LayerMask blockLayer;
    public LayerMask wallLayer;
    public LayerMask goalLayer;

    // Manually assign your switch GameObjects in the Inspector.
    // Each switch should have a SwitchController component.
    public GameObject[] switches;

    // NEW: Ghost mode flag, ghostUsed flag, and Animator
    private bool ghostMode = false;
    public bool ghostUsed = false; // This ensures ghost mode can be used only once per level
    public Animator animator; // Assign your Animator in the Inspector

    void Start()
    {
        // Create an indicator at the start (but disable it initially)
        if (indicatorPrefab)
        {
            indicator = Instantiate(indicatorPrefab, transform.position, Quaternion.identity);
            indicator.SetActive(false);
        }
    }

    void Update()
    {   
        // Toggle ghost mode with R
        if (Input.GetKeyDown(KeyCode.R))
        {
            ToggleGhostMode();
        }
        
        // Handle player movement with WASD
        if (Input.GetKeyDown(KeyCode.W)) Move(Vector2.up);
        if (Input.GetKeyDown(KeyCode.S)) Move(Vector2.down);
        if (Input.GetKeyDown(KeyCode.A)) Move(Vector2.left);
        if (Input.GetKeyDown(KeyCode.D)) Move(Vector2.right);

        if(!ghostMode)
        {
            // Rewind functionality
            if (Input.GetKeyDown(KeyCode.Q)) // Press 'Q' to rewind
            {
                Rewind();
            }      

            // Revert functionality
            if (Input.GetKeyDown(KeyCode.E)) // Press 'E' to revert everything
            {   
                GameManager.Instance.RevertAllToPreviousPositions();
                GameManager.Instance.CheckLava();
                RevertRewind();
                checkSwitches();
            }
        }
    }

    void Move(Vector2 direction)
    {   
        if (GameManager.Instance.isDead) return;
        
        // In ghost mode, move without updating history or pushing blocks.
        if (ghostMode)
        {
            Vector2 targetPosition = (Vector2)transform.position + direction;
            // Ghost mode ignores collision checks (or you can check only for walls if desired)
            transform.position = targetPosition;
            return;
        }
        
        // Normal movement: check for block pushing, collisions, etc.
        Vector2 normalTargetPosition = (Vector2)transform.position + direction;
        Collider2D blockCollider = Physics2D.OverlapCircle(normalTargetPosition, 0.1f, blockLayer);
        if (blockCollider != null)
        {
            Vector2 blockTargetPosition = (Vector2)blockCollider.transform.position + direction;
            // Check if the destination for the block is free (no wall or block)
            if (!IsBlocked(blockTargetPosition))
            {
                blockCollider.transform.position = blockTargetPosition;
                transform.position = normalTargetPosition;
            }
        }
        else 
        {
            if (!IsBlocked(normalTargetPosition))
            {
                transform.position = normalTargetPosition;
            }
        }

        // Normal mode: update history, indicators, switches, etc.
        UpdatePositionHistory();
        checkSwitches();
        UpdateIndicator();
        GameManager.Instance.RevertUpdate();
        CheckWin();
        GameManager.Instance.CheckLava(); // Lava check after movement
    }

    // Existing collision check for normal mode (wall and block layers)
    bool IsBlocked(Vector2 position)
    {
        Collider2D hit = Physics2D.OverlapCircle(position, 0.1f, wallLayer | blockLayer);
        return hit != null;
    }

    private void CheckWin()
    {   
        Collider2D goalCollider = Physics2D.OverlapCircle(transform.position, 0.1f, goalLayer);
        if (goalCollider != null)
        {
            GameManager.Instance.LevelComplete();
        }
    }

    private void UpdatePositionHistory()
    {
        positionHistory.Add(transform.position);
        if (positionHistory.Count > rewindSteps)
        {
            rewindIndex += 1;
            if (rewinded)
            {
                rewindedIndex += 1;
            }
        }
    }

    // This function records the position just before the player exits ghost mode.
    private void UpdatePositionAfterFutureTravel()
    {
        // Add the current position to the history list.
        positionHistory.Add(transform.position);

        // Limit the history size to rewindSteps.
        rewindIndex += (rewindSteps - 1);
        if (rewinded)
        {
            rewindedIndex += (rewindSteps - 1);
        }
        FutureTravelIndex = rewindIndex;
    }

    private void UpdateIndicator()
    {
        if (positionHistory.Count >= rewindSteps && !rewinded)
        {
            if (indicator != null)
            {
                indicator.transform.position = positionHistory[rewindIndex];
                indicator.SetActive(true); // Show the indicator
            }
        }
        else if (indicator != null)
        {
            indicator.SetActive(false);
        }
    }

    private void Rewind()
    {
        if (positionHistory.Count >= rewindSteps && !rewinded)
        {
            transform.position = positionHistory[rewindIndex];
            rewinded = true;
            UpdatePositionHistory();
            UpdateIndicator();
            GameManager.Instance.RevertUpdate();
            CheckWin();
            GameManager.Instance.CheckLava();
        }
    }

    private void RevertRewind()
    {   
        if (ghostUsed)
        {
            if (rewindIndex == FutureTravelIndex)
            {   
                rewindIndex -= (rewindSteps - 2);
                ghostUsed = false;
                Debug.Log(FutureTravelIndex);
            }
        }
        if (!turnOffRewind)
        {
            positionHistory.RemoveAt(positionHistory.Count - 1);
            if (rewindIndex > 0)
            {   
                
                rewindIndex -= 1;
                if (rewindedIndex > 0)
                {
                    rewindedIndex -= 1;
                }
                if (rewindedIndex == 0)
                {
                    rewinded = false;
                }
                //Debug.Log(rewindedIndex);
                //Debug.Log(rewinded);
            }
            UpdateIndicator();
        }
    }

    private void checkSwitches()
    {
        if (switches != null)
        {
            foreach (GameObject sw in switches)
            {
                if (sw != null)
                {
                    SwitchController switchController = sw.GetComponent<SwitchController>();
                    if (switchController != null)
                    {
                        switchController.Active();
                    }
                }
            }
        }
    }

    // NEW: Toggle ghost mode and trigger animator state change.
    private void ToggleGhostMode()
    {
        // If we're not currently in ghost mode...
        if (!ghostMode)
        {
            // Check if ghost mode has already been used this level.
            if (ghostUsed)
            {
                Debug.Log("Ghost mode has already been used in this level.");
                return;
            }
            // Enter ghost mode.
            ghostMode = true;
            ghostUsed = true;
            if (animator != null)
            {
                animator.SetBool("IsGhost", ghostMode);
            }
        }
        else // Ghost mode is active, so toggle it off.
        {
            // Check if it's valid to exit ghost mode (e.g., ensure player isn't blocked)
            if (IsBlocked(transform.position))
            {
                // If player is in a blocked position, they cannot exit ghost mode.
                ghostMode = true;
                Debug.Log("Cannot turn off ghost mode here!");
                return;
            }
            ghostMode = false;
            if (animator != null)
            {
                animator.SetBool("IsGhost", ghostMode);
            }
            // Record the position before returning to normal mode.
            UpdatePositionAfterFutureTravel();
            UpdateIndicator();
            checkSwitches();
            GameManager.Instance.RevertUpdate();
            GameManager.Instance.CheckLava();
        }
    }
}
