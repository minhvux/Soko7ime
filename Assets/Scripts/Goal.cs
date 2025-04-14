using UnityEngine;
using System.Collections;

public class Goal : MonoBehaviour
{   
    public float WinDelay = 1f;
    public GameObject WinParticlePrefab; 
    
    // Prefab for the win particle effect
    public void WinMove()
    {
        StartCoroutine(Win());
    }
    private IEnumerator Win()
    {
        DataHub.Instance.ReportMoveStarted();
        WinParticlePrefab.SetActive(true);
        yield return new WaitForSeconds(WinDelay);
        DataHub.Instance.ReportMoveComplete();
        GameManager.Instance.LevelComplete();
    }
}
