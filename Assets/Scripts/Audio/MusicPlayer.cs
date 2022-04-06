using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public static MusicPlayer instance;

    private AudioSource audioSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        audioSource = GetComponent<AudioSource>();
        //audioSource.volume = PlayerPrefs.GetFloat("music_configuration", 1);
    }

    public void PlaySong(AudioClip song)
    {
        if (song == null)
            return;
        StopAllCoroutines();
        audioSource.volume = PlayerPrefs.GetFloat("music_configuration", 1);
        audioSource.clip = song;
        audioSource.Play();
    }

    public void Play()
    {
        if (audioSource.clip == null)
            return;
        audioSource.Play();
    }

    public void PlayOneShotSong(AudioClip song)
    {
        StopAllCoroutines();
        audioSource.volume = PlayerPrefs.GetFloat("sound_configuration", 1);
        audioSource.PlayOneShot(song);
    }

    public void OnPitchEffect()
    {
        StartCoroutine(PitchFading());
    }

    public void OffPitchEffect()
    {
        audioSource.spatialBlend = 0;
        //StartCoroutine(PitchFadingOut());
    }

    IEnumerator PitchFading()
    {
        while (audioSource.spatialBlend < 0.75f)
        {
            audioSource.spatialBlend += Time.deltaTime / 2;
            yield return null;
        }
    }

    IEnumerator PitchFadingOut()
    {
        while (audioSource.spatialBlend > 0)
        {
            audioSource.spatialBlend -= Time.deltaTime / 2;
            yield return null;
        }
    }

    public void Stop()
    {
        audioSource.Stop();
    }

    public void FadeSong()
    {
        StartCoroutine(Fading());
    }

    IEnumerator Fading()
    {

        while (audioSource.volume > 0)
        {
            audioSource.volume -= Time.deltaTime / 2;
            yield return null;
        }
    }
}
