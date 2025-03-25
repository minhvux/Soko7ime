using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 1f;
    private Vector2 moveDirection;

    public int rewindSteps = 5; // Number of steps to rewind
    private int rewindIndex = 0;
    public bool rewinded = false;
    private int rewindedIndex = 0;

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
        // Handle player movement
        if (Input.GetKeyDown(KeyCode.W)) Move(Vector2.up);
        if (Input.GetKeyDown(KeyCode.S)) Move(Vector2.down);
        if (Input.GetKeyDown(KeyCode.A)) Move(Vector2.left);
        if (Input.GetKeyDown(KeyCode.D)) Move(Vector2.right);

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

    void Move(Vector2 direction)
    {   
        if (GameManager.Instance.isDead) return;

        Vector2 targetPosition = (Vector2)transform.position + direction;
        Collider2D blockCollider = Physics2D.OverlapCircle(targetPosition, 0.1f, blockLayer);
        if (blockCollider != null)
        {
            Vector2 blockTargetPosition = (Vector2)blockCollider.transform.position + direction;
            // Check if the destination for the block is free (no wall or block)
            if (!IsBlocked(blockTargetPosition))
            {
                blockCollider.transform.position = blockTargetPosition;
                transform.position = targetPosition;
            }
        }
        else 
        {
            if (!IsBlocked(targetPosition))
            {
                transform.position = targetPosition;
            }
        }

        // Update position history after movement
        UpdatePositionHistory();

        // Instead of checking switches here, let each SwitchController handle its own activation.
        checkSwitches();

        // Continue with other updates
        UpdateIndicator();
        GameManager.Instance.RevertUpdate();
        CheckWin();
        GameManager.Instance.CheckLava(); // Lava check after movement
    }

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
                Debug.Log(rewindedIndex);
                Debug.Log(rewinded);
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
}
