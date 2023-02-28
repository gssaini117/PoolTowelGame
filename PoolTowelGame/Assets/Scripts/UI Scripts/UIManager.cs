using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Canvas References")]
    [SerializeField] Image[] temperatureMeters;
    [SerializeField] Image[] faces;
    [SerializeField] Image[] umbrellaSlots;
    [SerializeField] Image[] patronLives;
    [SerializeField] Image[] towelIcons;
    [SerializeField] Image towelMeter;

    [Header("Sprite Swappables")]
    [SerializeField] Sprite SmileyFaceSprite;
    [SerializeField] Sprite AngryFaceSprite;
    [SerializeField] Sprite UmbrellaSprite;
    [SerializeField] Sprite SunSprite;

    public enum Face {
        Happy,
        Angry
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            SetTemperature(0, .8f);
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            SetTemperature(3, .2f);
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            SetAngry(3, true);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            SetUmbrella(2, true);
            SetUmbrella(1, true);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            SetTowelMeter(.2f);
            SetLivesRemaining(2);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            SetLivesRemaining(1);
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            SetTowels(false, false);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            SetTowels(false, true);
        }
    }

    void SetTemperature(int patronNum, float value)
    {
        Image meterImage = temperatureMeters[patronNum];
        meterImage.fillAmount = value;
    }

    void SetTowelMeter(float value)
    {
        towelMeter.fillAmount = value;
    }

    void SetLivesRemaining(int livesRemaining)
    {
        if (livesRemaining == 3)
        {
            patronLives[2].color = new Color(1, 1, 1, 1);
            patronLives[1].color = new Color(1, 1, 1, 1);
            patronLives[0].color = new Color(1, 1, 1, 1);
        }
        else if (livesRemaining == 2)
        {
            patronLives[2].color = new Color(0, 0, 0, .15f);
            patronLives[1].color = new Color(1, 1, 1, 1);
            patronLives[0].color = new Color(1, 1, 1, 1);
        }
        else if (livesRemaining == 1)
        {
            patronLives[2].color = new Color(0, 0, 0, .15f);
            patronLives[1].color = new Color(0, 0, 0, .15f);
            patronLives[0].color = new Color(1, 1, 1, 1);
        }
        else
        {
            patronLives[2].color = new Color(0, 0, 0, .15f);
            patronLives[1].color = new Color(0, 0, 0, .15f);
            patronLives[0].color = new Color(0, 0, 0, .15f);
        }
    }

    void SetTowels(bool left, bool right)
    {
        if (left)
        {
            towelIcons[0].color = new Color(1, 1, 1, 1);
        }
        else
        {
            towelIcons[0].color = new Color(0, 0, 0, .15f);
        }

        if (right)
        {
            towelIcons[1].color = new Color(1, 1, 1, 1);
        }
        else
        {
            towelIcons[1].color = new Color(0, 0, 0, .15f);
        }
    }

    void SetAngry(int patronNum, bool isAngry)
    {
        if (isAngry)
        {
            faces[patronNum].sprite = AngryFaceSprite;
        } 
        else
        {
            faces[patronNum].sprite = SmileyFaceSprite;
        }
    }

    void SetUmbrella(int patronNum, bool isCovered)
    {
        if (isCovered)
        {
            umbrellaSlots[patronNum].sprite = UmbrellaSprite;
        }
        else
        {
            umbrellaSlots[patronNum].sprite = SunSprite;
        }
    }
}
