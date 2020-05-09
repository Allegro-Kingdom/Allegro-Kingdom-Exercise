////////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2018 Audiokinetic Inc. / All Rights Reserved
//
////////////////////////////////////////////////////////////////////////

﻿using UnityEngine;
using System.Collections;

public class ArcadeMachine : MonoBehaviour, IInteractable
{
    public bool isPlayingMusic = false;
    public ParticleSystem NoteParticles;

    public AK.Wwise.Event ArcadeMusicStart;
    public AK.Wwise.Event ArcadeMusicStop;

    public void OnInteract()
    {
        if (!isPlayingMusic)
        {
			isPlayingMusic = true;
            NoteParticles.Play();
            ArcadeMusicStart.Post(gameObject);
        }
        else
        {
			isPlayingMusic = false;
            NoteParticles.Stop();
            ArcadeMusicStop.Post(gameObject);
        }

    }

    public void OnDestroy()
    {
        ArcadeMusicStop.Post(gameObject);
    }

}
