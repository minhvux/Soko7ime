using UnityEngine;

public class GateController : MonoBehaviour
{
    public GameObject gate;  // The gate object to be activated/deactivated
    private bool isGateActive = true;  // Gate is initially active

    void Start()
    {
        // Ensure the gate is active at the start
        if (gate != null)
        {
            gate.SetActive(isGateActive);
        }
    }

    // Method to activate or deactivate the gate based on the switch status
    public void ToggleGateState(bool isSwitchActive)
    {
        if (gate != null)
        {
            // If the switch is active, deactivate the gate, else activate it
            gate.SetActive(!isSwitchActive);
        }
    }

    // This method will be called to check for collisions with the gate
    private void OnTriggerStay2D(Collider2D other)
    {
        // Check if the object colliding with the gate is the switch
        if (other.CompareTag("Switch"))
        {
            // Call the ToggleGateState method to update the gate's state based on the switch's state
            SwitchController switchController = other.GetComponent<SwitchController>();
            if (switchController != null)
            {
                // You can pass the switch's active state to the gate here
                bool isSwitchActive = (switchController != null);
                ToggleGateState(isSwitchActive);
            }
        }
    }
}
