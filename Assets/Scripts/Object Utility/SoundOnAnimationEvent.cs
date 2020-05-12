////////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2018 Audiokinetic Inc. / All Rights Reserved
//
////////////////////////////////////////////////////////////////////////

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundOnAnimationEvent : MonoBehaviour {

    public List<AK.Wwise.Event> Sounds = new List<AK.Wwise.Event>();
    public AudioSource openRoll;
    public AudioSource closeRoll;

    public void PlaySoundWithIdx(int idx){
        Sounds[idx].Post(gameObject);

        if (idx == 0)
            openRoll.Play();

        else if (idx == 2)
            closeRoll.Play();
    }
}
