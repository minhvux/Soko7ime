using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxController : MonoBehaviour
{   

    public float moveSpeed = 6f;

    public bool TryToPushBox(Vector2 moveDirection)
    {
        Vector2 targetPosition = (Vector2)transform.position + moveDirection;
        if (!IsBlocked(targetPosition))
        {
            StartCoroutine(MoveToPosition(targetPosition));
            return true;
        }
        
        return false;
       
        
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

    private bool IsBlocked(Vector2 position)
    {
        Collider2D hit = Physics2D.OverlapCircle(position, 0.1f, DataHub.Instance.wallLayer | DataHub.Instance.blockLayer);
        return hit != null;
    }

}
