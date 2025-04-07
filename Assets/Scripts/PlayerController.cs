using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{   

    public float moveSpeed = 1f;
    public float bounceDistance = 0.5f;
    public float bounceSpeed = 1f;
    
    private Vector2 targetPosition;
    private Animator animator;
    public GameObject runTrailParticlePrefab; 
    public GameObject RewindBurstParticlePrefab; 

    public float rewindDelayBefore = 1f; // Delay before changing the position
    public float rewindDelayAfter = 1f;
    

    void Start()
    {
        // Get the Animator component from the player
        animator = GetComponent<Animator>();
    }

    public void TryToMove(Vector2 moveDirection)
    {   
        runTrailParticlePrefab.SetActive(true);
        if (moveDirection.x < 0)
        {
            // Moving left, flip the character horizontally
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (moveDirection.x > 0)
        {
            // Moving right, reset to the original scale
            transform.localScale = new Vector3(1, 1, 1);
        }


        targetPosition = (Vector2)transform.position + moveDirection;
        Collider2D hit = IsBlocked(targetPosition);
        if (hit == null || DataHub.Instance.futureMode)
        {
            StartCoroutine(MoveToPosition(targetPosition, moveDirection));
        }
        else if (hit.gameObject.CompareTag("Box"))
        {
            var box = hit.GetComponent<BoxController>();
            if (box != null && box.TryToPushBox(moveDirection))
            {
                StartCoroutine(MoveToPosition(targetPosition, moveDirection));
            }
            else
            {
                StartCoroutine(StaticBounce(targetPosition));        
            } 
        } else 
        {
            StartCoroutine(StaticBounce(targetPosition));        
        }      
    }

    private IEnumerator MoveToPosition(Vector2 target, Vector2 moveDirection)
    {   
        DataHub.Instance.ReportMoveStarted();

        // Set the animation parameters before starting the movement
        if (moveDirection.x != 0)
        {
            // Moving left or right, trigger horizontal movement animation
            animator.SetBool("MovingRL", true);
            animator.SetBool("MovingUD", false); // Stop vertical animation
        }
        else if (moveDirection.y != 0)
        {
            // Moving up or down, trigger vertical movement animation
            animator.SetBool("MovingUD", true);
            animator.SetBool("MovingRL", false); // Stop horizontal animation
        }

        // Move towards the target position
        while (Vector2.Distance(transform.position, target) > 0.01f)
        {
            transform.position = Vector2.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
            yield return null;
        }
        
        // Set the final position
        transform.position = target;       

        // Stop the animation flags once the movement is complete
        animator.SetBool("MovingRL", false);
        animator.SetBool("MovingUD", false);
        
        DataHub.Instance.ReportMoveComplete();
    }

    private IEnumerator StaticBounce(Vector2 target)
    {
        DataHub.Instance.ReportMoveStarted();
        animator.SetBool("Bounce", true);
        Vector2 currentPosition = transform.position;

        while (Vector2.Distance(a:transform.position, b:target) > bounceDistance)
        {    
            transform.position = Vector2.MoveTowards(transform.position, target, bounceSpeed *  Time.deltaTime);
            yield return null;
        }

        transform.position = currentPosition;

        animator.SetBool("Bounce", false);
        DataHub.Instance.ReportMoveComplete();
       

    }

    private Collider2D IsBlocked(Vector2 position)
    {
        Collider2D hit = Physics2D.OverlapCircle(position, 0.1f, DataHub.Instance.wallLayer | DataHub.Instance.blockLayer);
        return hit;
    }

    public void ToggleFutureMode()
    {
        // If not in future mode, enable it.
        if (!DataHub.Instance.futureMode)
        {
            DataHub.Instance.DataHubToFuture();
            
        }
        else
        {
            if (IsBlocked(transform.position) != null)
            {
                Debug.Log("Cannot settle here – the location overlaps with an obstacle.");
            }           
            
            else
            {
                DataHub.Instance.SettleFuture();
            }
        }
    }

    public void PlayerRewind()
    {
        StartCoroutine(Rewind());
        
    }

    private IEnumerator Rewind()
    {
        DataHub.Instance.ReportMoveStarted();
        runTrailParticlePrefab.SetActive(false);
        animator.SetBool("Rewind", true);
        Debug.Log("Starting rewind...");
        yield return new WaitForSeconds(rewindDelayBefore);
        RewindBurstParticlePrefab.SetActive(true);
        transform.position = DataHub.Instance.pastIndicator.transform.position;
        yield return new WaitForSeconds(rewindDelayAfter);
        RewindBurstParticlePrefab.SetActive(false);
        Debug.Log("Finished rewind");

        

        animator.SetBool("Rewind", false);
        
        DataHub.Instance.ReportMoveComplete();
    }

    
    public void PlayerRevert()
    {
        
        runTrailParticlePrefab.SetActive(false);
       
        
    
       
    }
}
