using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PastIndicatorController : MonoBehaviour
{
    public GameObject markerSprite;
    public GameObject playerSprite;

    public void DisablePlayerSprite(){
        playerSprite.SetActive(false);
    }

    public void EnablePlayerSprite(){
        playerSprite.SetActive(true);
    }
    
}
