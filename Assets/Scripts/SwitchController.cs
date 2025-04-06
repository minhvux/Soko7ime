using UnityEngine;

public class SwitchController : MonoBehaviour
{
    // Reference to the GateController that manages the gate.
    public GateController gateController;
    // This flag tracks whether this switch is currently activated.
    [HideInInspector]
    public bool isActivated = false;

    void Start()
    {
        CheckActive();
    }

    public void CheckActive()
    {   
        Physics2D.SyncTransforms();
        // Check for overlapping colliders on the "Player" or "Box" layers.
        Collider2D collisionCollider = Physics2D.OverlapCircle(transform.position, 0.1f, LayerMask.GetMask("Player", "Box"));

        if (collisionCollider != null)
        {
            Debug.Log("Switch at " + transform.position + " activated by: " + collisionCollider.gameObject.name);
            isActivated = true;
        }
        else
        {
            isActivated = false;
        }

        // Notify the gate controller to update gate state.
        if (gateController != null)
        {
            gateController.UpdateGateState();
        }
    }
}
