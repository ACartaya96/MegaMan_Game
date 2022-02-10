using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // Audio players components
    public AudioSource EffectSource;
    public AudioSource MusicSource;

    public float LowPitchRange = .95f;
    public float HighPitchRange = 1.05f;

    public static SoundManager Instance = null;
  
    /*Script from this source
     * https//www.daggerhart.com/unity-audio-and-sound-manager-singleton-scrip/
     *
     * 
     */
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else if (Instance == this)
        {
            Destroy(gameObject);
        }

  
        DontDestroyOnLoad(gameObject);
    }

    public void Play(AudioClip clip)
    {
        EffectSource.clip = clip;
        EffectSource.Play();
    }

    public void PlayMusic(AudioClip clip)
    {
        MusicSource.clip = clip;
        MusicSource.Play();
    }

    public void Pause()
    {
        EffectSource.Pause();
    }

    public void PauseMusic()
    {
        MusicSource.Pause();
    }

    public void Stop()
    {
        EffectSource.Stop();
    }

    public void StopMusic()
    {
        MusicSource.Stop();
    }

    public void RandomSoundEffect(params AudioClip[] clips)
    {
        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(LowPitchRange, HighPitchRange);

        EffectSource.pitch = randomPitch;
        EffectSource.clip = clips[randomIndex];
        EffectSource.Play();
    }

}
