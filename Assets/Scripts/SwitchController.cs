using UnityEngine;

public class SwitchController : MonoBehaviour
{
    public void Active()
    {   
        // Check for overlapping colliders on the "Player" or "Block" layers
        Physics2D.SyncTransforms();
        Collider2D collisionCollider = Physics2D.OverlapCircle(transform.position, 0.1f, LayerMask.GetMask("Player", "Block"));
        if (collisionCollider != null)
        {
            Debug.Log("Switch at " + transform.position + " activated by: " + collisionCollider.gameObject.name);
            // Additional logic for switch activation can be added here.
        }
    }
}
