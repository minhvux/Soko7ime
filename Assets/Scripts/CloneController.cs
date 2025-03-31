using UnityEngine;

public class CloneController : MonoBehaviour
{
    public LayerMask blockLayer;
    public LayerMask wallLayer;

    // Handle clone movement in the same way as the player.
    public void Move(Vector2 direction)
    {
        Vector2 targetPosition = (Vector2)transform.position + direction;
        Collider2D blockCollider = Physics2D.OverlapCircle(targetPosition, 0.1f, blockLayer);
        if (blockCollider != null)
        {
            Vector2 blockTargetPosition = (Vector2)blockCollider.transform.position + direction;
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
    }

    // Check if the clone's movement will result in a block or wall collision
    private bool IsBlocked(Vector2 position)
    {
        Collider2D hit = Physics2D.OverlapCircle(position, 0.1f, wallLayer | blockLayer);
        return hit != null;
    }
}
