using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsScene : MonoBehaviour
{
    public AudioSource audio1;
    public AudioSource audio2;
    // Start is called before the first frame update
    void Start()
    {
        audio1.PlayDelayed(2);
        StartCoroutine(StartFade(audio1, 4.0f, 1.0f, 2.0f));
        Invoke("Play2", 17.8f);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            if(audio1.isPlaying)
                StartCoroutine(StartFade(audio1, 2.0f, 0.0f, 0.0f));
            if (audio2.isPlaying)
                StartCoroutine(StartFade(audio2, 2.0f, 0.0f, 0.0f));
        }
    }

    void Play1()
    {
        audio1.Play();
        Invoke("Play2", 15.8f);
    }

    void Play2()
    {
        audio2.Play();
        Invoke("Play1", 15.8f);
    }

    public static IEnumerator StartFade(AudioSource audioSource, float duration, float targetVolume, float delay)
    {
        yield return new WaitForSeconds(delay);
        float currentTime = 0;
        float start = audioSource.volume;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }
        yield break;
    }
}


