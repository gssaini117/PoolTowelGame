using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] bool enableTestHotkeys = false;
    
    [Header("Audio Sources")]
    [SerializeField] AudioSource musicSource;

    [Header("Music List")]
    [SerializeField] AudioClip backgroundMusicClip;
    [SerializeField] AudioClip emergencyMusicClip;

    MusicMode currentMode = MusicMode.normal;
    bool changingMusicMode = false;

    public enum MusicMode
    {
        normal,
        emergency
    }

    private void Start()
    {
        musicSource.clip = backgroundMusicClip;
        musicSource.Play();
    }

    void Update()
    {
        if (changingMusicMode)
        {
            if (currentMode == MusicMode.normal)
            {
                musicSource.clip = backgroundMusicClip;
            }
            else
            {
                musicSource.clip = emergencyMusicClip;
            }
            musicSource.Play();
            changingMusicMode = false;
        }

        if (enableTestHotkeys)
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                SetMusicMode(MusicMode.emergency);
            }

            if (Input.GetKeyDown(KeyCode.V))
            {
                SetMusicMode(MusicMode.normal);
            }
        }
    }

    public void SetMusicMode(MusicMode mode)
    {
        if (currentMode != mode)
        {
            currentMode = mode;
            changingMusicMode = true;
        }
    }
}
