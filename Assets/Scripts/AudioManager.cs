using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public AudioSource musicSource;
    public AudioSource sfxSource;

    // Option to randomize SFX pitch/volume
    public bool randomizeSfx = true;
    public float pitchVariance = 0.1f;
    public float volumeVariance = 0.1f;    
    
    // Arrays of audio clips
    public AudioClip[] musicSounds;
    public AudioClip[] sfxSounds;

    // Store the default music volume for later restore.
    [HideInInspector]
    public float defaultMusicVolume;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            if(musicSource != null)
                defaultMusicVolume = musicSource.volume;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Start()
    {
        PlayMusic("MainTheme");
    }
    
    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (musicSource == null)
        {
            Debug.LogError("Music AudioSource is not assigned!");
            return;
        }
        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
    }
    
    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }
    
    public void PlaySFX(AudioClip clip, bool randomize = true)
    {
        if (sfxSource == null)
        {
            Debug.LogError("SFX AudioSource is not assigned!");
            return;
        }
        
        if (randomizeSfx && randomize)
        {
            float originalPitch = sfxSource.pitch;
            float originalVolume = sfxSource.volume;
            
            sfxSource.pitch = Random.Range(1f - pitchVariance, 1f + pitchVariance);
            float randomVolume = Random.Range(originalVolume - volumeVariance, originalVolume + volumeVariance);
            randomVolume = Mathf.Clamp(randomVolume, 0f, 1f);
            
            sfxSource.PlayOneShot(clip, randomVolume);
            sfxSource.pitch = originalPitch;
        }
        else
        {
            sfxSource.PlayOneShot(clip);
        }
    }
    
    public void PlayMusic(string soundName, bool loop = true)
    {
        AudioClip clip = null;
        foreach (AudioClip item in musicSounds)
        {
            if (item.name == soundName)
            {
                clip = item;
                break;
            }
        }
        
        if (clip != null)
        {
            PlayMusic(clip, loop);
        }
        else
        {
            Debug.LogWarning("Music sound not found: " + soundName);
        }
    }
    
    public void PlaySFX(string soundName, bool randomize = true)
    {
        AudioClip clip = null;
        foreach (AudioClip item in sfxSounds)
        {
            if (item.name == soundName)
            {
                clip = item;
                break;
            }
        }
        
        if (clip != null)
        {
            PlaySFX(clip, randomize);
        }
        else
        {
            Debug.LogWarning("SFX sound not found: " + soundName);
        }
    }
    
    // Fades the music volume to targetVolume over duration seconds.
    public IEnumerator FadeMusicVolume(float targetVolume, float duration)
    {
        if(musicSource == null)
            yield break;
            
        float startVolume = musicSource.volume;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            musicSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        musicSource.volume = targetVolume;
    }
}