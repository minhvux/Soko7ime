using UnityEngine;
using System.Collections;

public class Goal : MonoBehaviour
{   
    public float WinDelay = 1f;
    public GameObject WinParticlePrefab; 

    // Flag to ensure win sound only plays once.
    private bool winSoundPlayed = false;

    // Duration for fading music volume down/up
    public float fadeDuration = 0.5f;
    // The factor to lower the music volume (e.g., 20% of default volume)
    public float lowerVolumeFactor = 0.2f;
    
    public void WinMove()
    {
        StartCoroutine(Win());
    }

    private IEnumerator Win()
    {
        DataHub.Instance.ReportMoveStarted();
        
        // Fade music volume down.
        yield return StartCoroutine(AudioManager.Instance.FadeMusicVolume(AudioManager.Instance.defaultMusicVolume * lowerVolumeFactor, fadeDuration));

        if (!winSoundPlayed)
        {
            winSoundPlayed = true;
            AudioManager.Instance.PlaySFX("Win", false);
        }
        
        WinParticlePrefab.SetActive(true);
        yield return new WaitForSeconds(WinDelay);
        
        // Fade music volume back up to default.
        yield return StartCoroutine(AudioManager.Instance.FadeMusicVolume(AudioManager.Instance.defaultMusicVolume, fadeDuration));
        DataHub.Instance.ReportMoveComplete();
        GameManager.Instance.LevelComplete();
    }
}
