using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuturePastPlayerController : MonoBehaviour
{
    

    public float moveSpeed = 6f;
    public float bounceDistance = 0.9f;
    public float bounceSpeed = 1.5f;

    private Vector2 targetPosition;
    
    
    // Start is called before the first frame update
    public void TryToMove(Vector2 moveDirection)
    {   
        if (DataHub.Instance.futureMode) return;
        targetPosition = (Vector2)transform.position + moveDirection;
        Collider2D hit = IsBlocked(targetPosition);
        if (hit == null)
        {
            StartCoroutine(MoveToPosition(targetPosition));
        }
        else if (hit.gameObject.CompareTag("Box"))
        {
            var box = hit.GetComponent<BoxController>();
            if (box != null && box.TryToPushBox(moveDirection))
            {
                StartCoroutine(MoveToPosition(targetPosition));
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

    private IEnumerator MoveToPosition(Vector2 target)
    {
        

        while (Vector2.Distance(a:transform.position, b:target) > 0.01f)
        {
            transform.position = Vector2.MoveTowards(transform.position, target, moveSpeed *  Time.deltaTime);
            yield return null;
        }
      
        transform.position = target;
        

        
    }

    private IEnumerator StaticBounce(Vector2 target)
    {
        
        Vector2 currentPosition = transform.position;

        while (Vector2.Distance(a:transform.position, b:target) > bounceDistance)
        {    
            transform.position = Vector2.MoveTowards(transform.position, target, bounceSpeed *  Time.deltaTime);
            yield return null;
        }

        transform.position = currentPosition;
        

    }

    private Collider2D IsBlocked(Vector2 position)
    {
        Collider2D hit = Physics2D.OverlapCircle(position, 0.1f, DataHub.Instance.wallLayer | DataHub.Instance.blockLayer);
        return hit;
    }
}
