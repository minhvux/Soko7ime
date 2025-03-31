using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{   
    public bool isMoving = false;

    public float moveSpeed = 1f;
    public float bounceDistance = 0.5f;
    public float bounceSpeed = 1f;

    private Vector2 targetPosition;

    public void TryToMove(Vector2 moveDirection)
    {
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
        isMoving = true;

        while (Vector2.Distance(a:transform.position, b:target) > 0.01f)
        {
            transform.position = Vector2.MoveTowards(transform.position, target, moveSpeed *  Time.deltaTime);
            yield return null;
        }

        transform.position = target;
        isMoving = false;
        DataHub.Instance.AfterMovingUpdate();
    }

    private IEnumerator StaticBounce(Vector2 target)
    {
        isMoving = true;
        Vector2 currentPosition = transform.position;

        while (Vector2.Distance(a:transform.position, b:target) > bounceDistance)
        {    
            transform.position = Vector2.MoveTowards(transform.position, target, bounceSpeed *  Time.deltaTime);
            yield return null;
        }

        transform.position = currentPosition;
        isMoving = false;

    }

    private Collider2D IsBlocked(Vector2 position)
    {
        Collider2D hit = Physics2D.OverlapCircle(position, 0.1f, DataHub.Instance.wallLayer | DataHub.Instance.blockLayer);
        return hit;
    }
}
