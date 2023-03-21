using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    [SerializeField] bool enableTestHotkeys = false;
    
    [Header("Audio Sources")]
    [SerializeField] AudioSource[] patronSources;
    [SerializeField] AudioSource[] splashSources;

    [Header("SFX List")]
    [SerializeField] AudioClip[] patronHotClips;
    [SerializeField] AudioClip[] patronColdClips;
    [SerializeField] AudioClip[] patronLeavingClips;
    [SerializeField] AudioClip splashClip;

    // Boolean Checks
    bool[] patronHotBools       = { false, false, false, false };
    bool[] patronColdBools      = { false, false, false, false };
    bool[] patronLeavingBools   = { false, false, false, false };
    bool[] splashBools = {false,false};

    private void Update()
    {
        if (enableTestHotkeys)
        {
            if (Input.GetKey(KeyCode.F))
            {
                SetSplashSFX(0, true);
            }
            else
            {
                SetSplashSFX(0, false);
            }

            if (Input.GetKey(KeyCode.G))
            {
                SetSplashSFX(1, true);
            }
            else
            {
                SetSplashSFX(1, false);
            }
        }
    }

    public void SetPatronHotSFX(int patronNum, bool condition)
    {
        bool lastCondition = patronHotBools[patronNum];
        if (lastCondition != condition)
        {
            if (condition == true)
            {
                // play sfx
                patronSources[patronNum].clip = patronHotClips[Random.Range(0, 4)];
                patronSources[patronNum].Play();

                // change condition so next 'true' call won't fire sfx
                patronHotBools[patronNum] = true;
            }
            else
            {
                // Prep splash (enables sound play on the next 'true')
                patronHotBools[patronNum] = false;
            }
        }
    }

    public void SetPatronColdSFX(int patronNum, bool condition)
    {
        bool lastCondition = patronColdBools[patronNum];
        if (lastCondition != condition)
        {
            if (condition == true)
            {
                // play sfx
                patronSources[patronNum].clip = patronColdClips[Random.Range(0, 3)]; // ONLY RANDOM BETWEEN 0 AND 3
                patronSources[patronNum].Play();

                // change condition so next 'true' call won't fire sfx
                patronColdBools[patronNum] = true;
            }
            else
            {
                // Prep splash (enables sound play on the next 'true')
                patronColdBools[patronNum] = false;
            }
        }
    }

    public void SetPatronLeavingSFX(int patronNum, bool condition)
    {
        bool lastCondition = patronLeavingBools[patronNum];
        if (lastCondition != condition)
        {
            if (condition == true)
            {
                // play sfx
                patronSources[patronNum].clip = patronLeavingClips[Random.Range(0, 5)]; // IS NOT RANDOM YET
                patronSources[patronNum].Play();

                // change condition so next 'true' call won't fire sfx
                patronLeavingBools[patronNum] = true;
            }
            else
            {
                // Prep splash (enables sound play on the next 'true')
                patronLeavingBools[patronNum] = false;
            }
        }
    }

    public void SetSplashSFX(int side, bool condition)
    {
        bool lastCondition = splashBools[side];
        if (lastCondition != condition)
        {
            if (condition == true)
            {
                // play sfx
                splashSources[side].clip = splashClip;
                splashSources[side].Play();

                // change condition so next 'true' call won't fire sfx
                splashBools[side] = true;
            }
            else
            {
                // Prep splash (enables sound play on the next 'true')
                splashBools[side] = false;
            }
        }
    }
}
