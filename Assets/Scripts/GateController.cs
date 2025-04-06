using System.Collections.Generic;
using UnityEngine;

public class GateController : MonoBehaviour
{
    // The gate object that blocks the level; when open, we disable it.
    public GameObject gate;
    // List of switches controlling this gate - assign them in the Inspector.
    public List<SwitchController> switches;

    void Start()
    {
        // Ensure the gate is active (closed) at the start.
        if (gate != null)
        {
            gate.SetActive(true);
        }
    }

    // This method is called by switches whenever their state updates.
    public void UpdateGateState()
    {
        bool allActivated = true;
        foreach (SwitchController sw in switches)
        {
            if (sw == null || !sw.isActivated)
            {
                allActivated = false;
                break;
            }
        }
        // Gate is open (disabled) only when all switches are activated; otherwise it is closed (active).
        if (gate != null)
        {
            gate.SetActive(!allActivated);
        }
    }
}
