using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScene : MonoBehaviour
{
    public enum Type
    {
        None = -1,
        Training,
        Village,
        RoadWoodlands,
        Woodlands,
        Cave,
        Desert,
        MAX
    }

    float[] duration = { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };

    public AudioClip[] clips;
    public AudioSource audio1;
    public AudioSource audio2;

    public GameObject sun;

    Type type1 = Type.None;
    Type type2 = Type.None;

    IEnumerator co1 = null;
    IEnumerator co2 = null;

    int night = 0;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if ((sun.GetComponent<DayNightCycle>().timeOfDay < 6 || sun.GetComponent<DayNightCycle>().timeOfDay >= 18) && night == 0)
        {
            night = (int)Type.MAX;
            if (type1 != Type.None)
            {
                audio2.time = audio1.time;
                Insert(type1);
            }
            else if (type2 != Type.None)
            {
                audio1.time = audio2.time;
                Insert(type2);
            }
        }
        else if (sun.GetComponent<DayNightCycle>().timeOfDay >= 6 && sun.GetComponent<DayNightCycle>().timeOfDay < 18 && night != 0)
        {
            night = 0;
            if (type2 != Type.None)
            {
                audio1.time = audio2.time;
                Insert(type2);
            }else if (type1 != Type.None)
            {
                audio2.time = audio1.time;
                Insert(type1);
            }
        }
    }

    public void Insert(Type type)
    {
        if(type1 == Type.None)
        {
            type1 = type;
            type2 = Type.None;
            audio1.clip = clips[(int)type + night];
            if(co1 != null)
                StopCoroutine(co1);
            if (co2 != null)
                StopCoroutine(co2);
            co1 = StartFade(audio1, 2.0f, 1.0f, 0.0f);
            co2 = StartFade(audio2, 2.0f, 0.0f, 0.0f);
            StartCoroutine(co1);
            StartCoroutine(co2);
            audio1.Play();
        }
        else if (type2 == Type.None)
        {
            type1 = Type.None;
            type2 = type;
            audio2.clip = clips[(int)type + night];
            if (co1 != null)
                StopCoroutine(co1);
            if (co2 != null)
                StopCoroutine(co2);
            co1 = StartFade(audio1, 2.0f, 0.0f, 0.0f);
            co2 = StartFade(audio2, 2.0f, 1.0f, 0.0f);
            StartCoroutine(co1);
            StartCoroutine(co2);
            audio2.Play();
        }
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
