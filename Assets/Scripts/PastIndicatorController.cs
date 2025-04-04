using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PastIndicatorController : MonoBehaviour
{
    public void Move(Vector2 moveDirection)
    {
        Vector2 targetPosition = (Vector2)transform.position + moveDirection;
        transform.position = targetPosition;
    }
    
}
