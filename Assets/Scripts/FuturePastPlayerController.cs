using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuturePastPlayerController : MonoBehaviour
{
    

    public float moveSpeed = 6f;
    public float bounceDistance = 0.9f;
    public float bounceSpeed = 1.5f;

    private Vector2 targetPosition;
    public Animator animator;
    public GameObject runTrailParticlePrefab;
    public GameObject StartFutureParticlePrefab;

    public float SettleFutureDelay = 0.75f;
    
    
    
    
    // Start is called before the first frame update
    public void TryToMove(Vector2 moveDirection)
    {   
        runTrailParticlePrefab.SetActive(true);
        
        AlignPlayerDirection(moveDirection);

        if (DataHub.Instance.futureMode) return;
        
        targetPosition = (Vector2)transform.position + moveDirection;
        Collider2D hit = IsBlocked(targetPosition);
        if (hit == null)
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

        if (moveDirection.x != 0)
        {
            // Moving left or right, trigger horizontal movement animation
            animator.SetBool("FMovingRL", true);
            animator.SetBool("FMovingUD", false); // Stop vertical animation
        }
        else if (moveDirection.y != 0)
        {
            // Moving up or down, trigger vertical movement animation
            animator.SetBool("FMovingUD", true);
            animator.SetBool("FMovingRL", false); // Stop horizontal animation
        }
        
        while (Vector2.Distance(a:transform.position, b:target) > 0.01f)
        {
            transform.position = Vector2.MoveTowards(transform.position, target, moveSpeed *  Time.deltaTime);
            yield return null;
        }
      
        transform.position = target;
        
        animator.SetBool("FMovingRL", false);
        animator.SetBool("FMovingUD", false);
        
        
        DataHub.Instance.ReportMoveComplete();
        
    }

    private IEnumerator StaticBounce(Vector2 target)
    {
        DataHub.Instance.ReportMoveStarted(); 
        animator.SetBool("FBounce", true);
        
        Vector2 currentPosition = transform.position;

        while (Vector2.Distance(a:transform.position, b:target) > bounceDistance)
        {    
            transform.position = Vector2.MoveTowards(transform.position, target, bounceSpeed *  Time.deltaTime);
            yield return null;
        }

        transform.position = currentPosition;

        animator.SetBool("FBounce", false);
        
        DataHub.Instance.ReportMoveComplete();
    }

    private Collider2D IsBlocked(Vector2 position)
    {
        Collider2D hit = Physics2D.OverlapCircle(position, 0.1f, DataHub.Instance.wallLayer | DataHub.Instance.blockLayer);
        return hit;
    }

    private void AlignPlayerDirection(Vector2 moveDirection)
    {   
        if (DataHub.Instance.futureMode) 
        {
            Vector2 distance = (Vector2)transform.position - (Vector2)DataHub.Instance.player.transform.position; 
            if (distance.x < 0)
            {
                // Moving left, flip the character horizontally
                transform.localScale = new Vector3(1, 1, 1);
            }
            else if (distance.x > 0)
            {
                // Moving right, reset to the original scale
                transform.localScale = new Vector3(-1, 1, 1);
            }
            return;
        }
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
        
        
    }

    public void FuturePastPlayerRevert()
    {
        runTrailParticlePrefab.SetActive(false);
    }

    public void FuturePastPlayerToFuture()
    {
       
        runTrailParticlePrefab.SetActive(false);
        animator.SetBool("Future", true);
        Debug.Log("dcm????");
        StartFutureParticlePrefab.SetActive(true);
        
        
        
        
    }

   

    public void FuturePastPlayerSettleFuture()
    {
        StartCoroutine(SettleFuture());
    }

    private IEnumerator SettleFuture()
    {
        
        if (StartFutureParticlePrefab != null)
        {
            ParticleSystem ps = StartFutureParticlePrefab.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Stop();
            }
        }
        animator.SetBool("Future", false);
        animator.SetBool("FSettleFuture", true);
        Debug.Log("dcm????");

        
        yield return new WaitForSeconds(SettleFutureDelay);

        animator.SetBool("FSettleFuture", false);

        StartFutureParticlePrefab.SetActive(false);
        
        
        
    }
}
