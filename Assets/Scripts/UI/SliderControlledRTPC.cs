////////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2018 Audiokinetic Inc. / All Rights Reserved
//
////////////////////////////////////////////////////////////////////////

﻿using UnityEngine;
using UnityEngine.Audio;

public class SliderControlledRTPC : MonoBehaviour
{
    public AK.Wwise.RTPC RTPC;
    public AudioMixer audioMixer;

    public void SetRTPC(float value)
    {
        if (Menu.isOpen)
        {
            RTPC.SetGlobalValue(value);
        }
    }

    public void SetMasterVolume(float value)
    {
        audioMixer.SetFloat("masterVolume", value);
    }

    public void SetMusicVolume(float value)
    {
        audioMixer.SetFloat("musicVolume", value);
    }

}
