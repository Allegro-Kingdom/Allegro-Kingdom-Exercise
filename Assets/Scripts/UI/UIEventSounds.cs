////////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2018 Audiokinetic Inc. / All Rights Reserved
//
////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEngine.EventSystems;

public class UIEventSounds : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    public AK.Wwise.Event OnPointerDownSound;
    public AK.Wwise.Event OnPointerUpSound;
    public AK.Wwise.Event OnPointerEnterSound;
    public AK.Wwise.Event OnPointerExitSound;

    public AudioSource pointerDownSound;
    public AudioSource pointerEnterSound;

    private void Start()
    {
        pointerDownSound = GameObject.Find("OpenSound").GetComponent<AudioSource>();
        pointerEnterSound = GameObject.Find("OverSound").GetComponent<AudioSource>();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        OnPointerDownSound.Post(gameObject);
        pointerDownSound.Play();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnPointerEnterSound.Post(gameObject);
        pointerEnterSound.Play();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnPointerExitSound.Post(gameObject);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnPointerUpSound.Post(gameObject);
    }
}
