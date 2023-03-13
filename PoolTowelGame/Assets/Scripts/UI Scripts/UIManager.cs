using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Inspector Variables

    [SerializeField] bool testing;
    
    [Header("Canvas References")]
    [SerializeField] Image[] temperatureMeters;
    [SerializeField] Image[] faces;
    [SerializeField] Image[] umbrellaSlots;
    [SerializeField] Image[] patronLives;
    [SerializeField] Image[] towelIcons;
    [SerializeField] Image[] wetCovers;
    [SerializeField] Image endScreen;
    [SerializeField] Image warningScreen;
    [SerializeField] Image warningMeter;
    [SerializeField] Image towelMeter;
    [SerializeField] Text timerText;

    [Header("Sprite Swappables")]
    [SerializeField] Sprite UmbrellaSprite;
    [SerializeField] Sprite SunSprite;
    [SerializeField] Sprite TowelBoyWinsSprite;
    [SerializeField] Sprite PoolBoyWinsSprite;
    [SerializeField] Sprite[] HappySprites;
    [SerializeField] Sprite[] ColdSprites;
    [SerializeField] Sprite[] HotSprites;

    [Header("TestVariables")]
    float currTemp = 100f;

    // Patron Art Management Variables

    int[] patronFaceMarkers = { 0, 1, 2, 3 };

    public enum Emote {
        Hot,
        Happy,
        Cold,
        Gone,
    }

    private void Start()
    {
        endScreen.color = new Color(0, 0, 0, 0);
        SetWetCover(false, false);
        SetWarningIsOn(false);

        SetEmotion(0, Emote.Happy);
        SetEmotion(1, Emote.Happy);
        SetEmotion(2, Emote.Happy);
        SetEmotion(3, Emote.Happy);
    }

    // Update is called once per frame
    void Update()
    {
        if (testing)
        {
            if (Input.GetKey(KeyCode.G))
            {
                currTemp -= 15f * Time.deltaTime;
                SetTemperature(0, currTemp/100);
            }

            if (Input.GetKey(KeyCode.H))
            {
                currTemp += 25f * Time.deltaTime;
                SetTemperature(0, currTemp / 100);
            }

            if (Input.GetKeyDown(KeyCode.J))
            {
                SetEmotion(3, Emote.Hot);
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

            if (Input.GetKeyDown(KeyCode.R))
            {
                SetWetCover(true, false);
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                RandomizePatron(0);
                SetEmotion(0, Emote.Happy);
            }

            if (Input.GetKeyDown(KeyCode.Y))
            {
                RandomizePatron(3);
                SetEmotion(3, Emote.Happy);
            }
        }
    }

    public void SetTemperature(int patronNum, float value)
    {
        Image meterImage = temperatureMeters[patronNum];
        meterImage.fillAmount = value;
    }

    public void SetTowelMeter(float value)
    {
        towelMeter.fillAmount = value;
    }

    public void SetLivesRemaining(int livesRemaining)
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

    public void SetTowels(bool left, bool right)
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

    public void SetWetCover(bool left, bool right)
    {
        if (left)
        {
            wetCovers[0].color = new Color(1, 1, 1, 1);
        }
        else
        {
            wetCovers[0].color = new Color(0, 0, 0, 0);
        }

        if (right)
        {
            wetCovers[1].color = new Color(1, 1, 1, 1);
        }
        else
        {
            wetCovers[1].color = new Color(0, 0, 0, 0);
        }
    }

    public void SetWarningIsOn(bool isOn)
    {
        if (isOn)
        {
            warningScreen.color = new Color(1, 1, 1, 1);
            warningMeter.color = new Color(1, 1, 1, 1);
        }
        else
        {
            warningScreen.color = new Color(0, 0, 0, 0);
            warningMeter.color = new Color(0, 0, 0, 0);
        }
    }

    public void SetWarningMeter(float value)
    {
        warningMeter.fillAmount = value;
    }

    public void SetEmotion(int patronNum, Emote emote)
    {
        faces[patronNum].color = new Color(1, 1, 1, 1);

        if (emote == Emote.Hot)
        {
            faces[patronNum].sprite = HotSprites[patronFaceMarkers[patronNum]];
        }
        else if (emote == Emote.Cold)
        {
            faces[patronNum].sprite = ColdSprites[patronFaceMarkers[patronNum]];
        }
        else if (emote == Emote.Happy)
        {
            faces[patronNum].sprite = HappySprites[patronFaceMarkers[patronNum]];
        }
        else
        {
            faces[patronNum].color = new Color(1, 1, 1, 0);
        }
    }

    public void SetUmbrella(int patronNum, bool isCovered)
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

    public void SetCountdownTimerText(int time)
    {
        timerText.text = time.ToString();
    }

    public void DisplayEndScreen(bool towelBoyWon)
    {
        if (towelBoyWon)
        {
            endScreen.sprite = TowelBoyWinsSprite;
        } 
        else
        {
            endScreen.sprite = PoolBoyWinsSprite;
        }

        endScreen.color = new Color(1, 1, 1, 1);
    }

    public void RandomizePatron(int patronNum)
    {
        patronFaceMarkers[patronNum] = Random.Range(0, 3);
    }
}
