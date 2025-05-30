using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{   

    public float moveSpeed = 1f;
    public float bounceDistance = 0.5f;
    public float bounceSpeed = 1f;
    
    private Vector2 targetPosition;
    public Animator animator;
    public GameObject runTrailParticlePrefab; 
    public GameObject RewindBurstParticlePrefab; 
    public GameObject SettleFutureParticlePrefab; 

    public float rewindDelayBefore = 1f; // Delay before changing the position
    public float rewindDelayAfter = 1f;

    
    public float SettleFutureDelay = 0.5f;
    

    

    public void TryToMove(Vector2 moveDirection)
    {   
        if (!DataHub.Instance.futureMode) runTrailParticlePrefab.SetActive(true);
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
        AudioManager.Instance.PlaySFX("Move", randomize: true);
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
        AudioManager.Instance.PlaySFX("Bounce", randomize: true);
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
        AudioManager.Instance.PlaySFX("Pop", randomize: false);
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

    public void PlayerToFuture()
    {
       
        runTrailParticlePrefab.SetActive(false);
        animator.SetBool("ToFuture", true);
        
        
        
        
        
    }

   

    public void PlayerSettleFuture()
    {
        StartCoroutine(SettleFuture());
        
    }

    private IEnumerator SettleFuture()
    {
        DataHub.Instance.ReportMoveStarted();

        animator.SetBool("ToFuture", false);
        animator.SetBool("SettleFuture", true);

        Debug.Log("Settle future called");

        SettleFutureParticlePrefab.SetActive(true);
        
        yield return new WaitForSeconds(SettleFutureDelay);

        animator.SetBool("SettleFuture", false);
        SettleFutureParticlePrefab.SetActive(false);
        
        DataHub.Instance.ReportMoveComplete();
    }

    public void Win() {
        
        animator.SetBool("Win", true);
        
    }
    
}
