using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    [SerializeField] bool enableTestHotkeys = false;
    
    [Header("Audio Sources")]
    [SerializeField] AudioSource patronSource;
    [SerializeField] AudioSource splashSource;

    [Header("SFX List")]
    [SerializeField] AudioClip patronHotClip;
    [SerializeField] AudioClip patronColdClip;
    [SerializeField] AudioClip patronLeavingClip;
    [SerializeField] AudioClip splashClip;

    private void Update()
    {
        if (enableTestHotkeys)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                PlaySplash();
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                PlayPatronLeaving();
            }
        }
    }

    public void PlayPatronHot()
    {
        patronSource.clip = patronHotClip;
        patronSource.Play();
    }
    public void PlayPatronCold()
    {
        patronSource.clip = patronColdClip;
        patronSource.Play();
    }

    public void PlayPatronLeaving()
    {
        patronSource.clip = patronLeavingClip;
        patronSource.Play();
    }

    public void PlaySplash()
    {
        splashSource.clip = splashClip;
        splashSource.Play();
    }
}
