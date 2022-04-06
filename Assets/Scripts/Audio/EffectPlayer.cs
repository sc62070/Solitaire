using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectPlayer : MonoBehaviour
{
    public static EffectPlayer instance;
    private AudioSource[] audioSources;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

        audioSources = transform.GetComponentsInChildren<AudioSource>();

        for (int i = 0; i < audioSources.Length; i++)
            audioSources[i].volume = PlayerPrefs.GetFloat("sound_configuration", 1);
    }

    public void PlayOneShotSong(AudioClip song)
    {
        if (song == null)       
            return;

        for (int i = 0; i < audioSources.Length; i++)
        {
            if (!audioSources[i].isPlaying)
            {
                audioSources[i].pitch = 1;
                audioSources[i].volume = PlayerPrefs.GetFloat("sound_configuration", 1);
                audioSources[i].PlayOneShot(song);
                return;
            }
        }
        audioSources[0].PlayOneShot(song);
    }

    public void PlayOneShotRandomPitchSong(AudioClip song)
    {
        if (song == null)
            return;

        for (int i = 0; i < audioSources.Length; i++)
        {
            if (!audioSources[i].isPlaying)
            {
                audioSources[i].volume = PlayerPrefs.GetFloat("sound_configuration", 1);
                audioSources[i].pitch = Random.Range(0.75f, 1.25f);
                audioSources[i].PlayOneShot(song);
                return;
            }
        }
        audioSources[0].PlayOneShot(song);
    }

    public void Stop()
    {
        for (int i = 0; i < audioSources.Length; i++)
        {
            audioSources[i].Stop();
        }
    }

    public void Mute(bool state)
    {
        for (int i = 0; i < audioSources.Length; i++)
        {
            audioSources[i].mute = state;
        }
    }

    public bool InPlay()
    {
        return audioSources[0].isPlaying;
    }
}
