using UnityEngine;

public class SwitchController : MonoBehaviour
{
    public GateController gateController;  // Reference to the GateController script

    public void CheckActive()
    {   
        Physics2D.SyncTransforms();
        // Check for overlapping colliders on the "Player" or "Block" layers
        //Debug.Log("Checking for collisions at " + transform.position);
        Collider2D collisionCollider = Physics2D.OverlapCircle(transform.position, 0.1f, LayerMask.GetMask("Player", "Box"));
        
        if (collisionCollider != null)
        {
            Debug.Log("Switch at " + transform.position + " activated by: " + collisionCollider.gameObject.name);

            // If the switch is activated, toggle the gate state by calling the ToggleGateState method
            if (gateController != null)
            {
                // Toggle the gate state based on the switch being activated
                gateController.ToggleGateState(true); // Passing 'true' indicates the switch is active
            }
        }
        else
        {
            // If no collision is detected, make sure the gate is activated again
            if (gateController != null)
            {
                gateController.ToggleGateState(false); // Passing 'false' indicates the switch is not active
            }
        }
    }
}
