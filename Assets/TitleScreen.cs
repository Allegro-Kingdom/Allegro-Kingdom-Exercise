using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreen : MonoBehaviour
{
    public AudioSource audio;
    // Start is called before the first frame update
    void Start()
    {
        audio.PlayDelayed(6.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
