using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStates : MonoBehaviour
{
    // Test Variables
    [SerializeField] bool enableTestHotkeys = false;
    
    // UI Manager
    [SerializeField] UIManager uiManager;
    [SerializeField] SFXManager sfxManager;
    [SerializeField] MusicManager musicManager;

    // Arduino Values
    public SerialController serialController;
    private string[] arduinoValues;
    private bool[] arduinoConvertedValues;

    // Towel Values
    public GameObject TowelBar;
    public GameObject TowelSensor1;
    public GameObject TowelSensor2;
    private bool towel1; // value of towel sensor 1
    private bool towel2; // value of towel sensor 2

    // Water Values
    [SerializeField] public GameObject[] WaterSensors;
    private bool[] water = {false, false}; // values of water sensors
    private int[] wetnessFactor = { 1, 1, 1, 1 }; // rate of water

    // Umbrella Values
    [SerializeField] public GameObject[] UmbrellaSensors;
    private bool[] umbrella = {false, false, false, false}; // values of umbrella sensors
    private int[] umbrellaActive = { 1, 1, 1, 1 }; // value for rate of tanning

    // Patron Values
    [SerializeField] public GameObject[] Patrons;
    private float[] patronStatus = {0f, 0f, 0f, 0f}; // values for patron's tans
    private bool[] patronReset = {false, false, false, false}; // if patron's are currently resetting (takes 5 seconds)

    // Game Constants
    public float gameLength;
    private float gameTimer = 0f;
    private bool gameOver = false;
    public float gameOverLength;
    private float gameOverTimer = 0f;

    // Win Conditions
    private bool poolBoyWins = false;
    private bool towelBoyWins = false;

    // Pool Boy Lives
    public int poolBoyChances;
    private int currentPoolBoyChances = 0; // actual value

    // Tanning Values
    public float tanningRate;
    public float tanningMin;
    public float tanningMax;
    public float tanningWarningBuffer;

    // Towel Values pt.2 
    public float towelTime;
    private float currentTowelTime = 0f; // actual value
    public float towelDecayRate;
    public float towelGracePeriodDuration;
    private float towelGracePeriodTimer; // actual value

    // Emergency Values
    public float emergencyModeDuration;
    private float emergencyModeTimer; // actual value
    private bool pausingForEmergency = false;

    // Start is called before the first frame update
    void Start()
    {
        serialController = GameObject.Find("SerialController").GetComponent<SerialController>();
        arduinoValues = new string[8];
        arduinoConvertedValues = new bool[8];
        gameLength = 180f;
        gameOverLength = 5f;
        poolBoyChances = 3;
        tanningRate = 1f;
        tanningMin = -10f;
        tanningMax = 10f;
        tanningWarningBuffer = 5.3f;
        towelTime = 30f;
        currentTowelTime = towelTime;
        towelDecayRate = 1f;
        towelGracePeriodDuration = 5f;
        emergencyModeDuration = 10f;
    }

    // Update is called once per frame as fast as the computer can go
    void Update()
    {
        HandleInput();
        UpdateUI();
    }

    // Fixed Update is called once per frame at 60 fps
    void FixedUpdate()
    {
        int truncatedCurrentTowelTime = (int)currentTowelTime;
        string serialMessage = truncatedCurrentTowelTime.ToString();
        serialController.SendSerialMessage(serialMessage);

        if (!gameOver)
        {

            // pool boy chances logic
            uiManager.SetLivesRemaining(poolBoyChances - currentPoolBoyChances);
            if (currentPoolBoyChances > 3)
            {
                gameOver = true;
                towelBoyWins = true;
                uiManager.DisplayEndScreen(true);
            }

            // emergency mode logic
            if (water[0] && water[1])
            {
                uiManager.SetWarningIsOn(true);
                pausingForEmergency = true;
                musicManager.SetMusicMode(MusicManager.MusicMode.emergency);
                if (emergencyModeTimer < emergencyModeDuration)
                {
                    emergencyModeTimer += Time.deltaTime;
                    uiManager.SetWarningMeter((emergencyModeDuration - emergencyModeTimer)/emergencyModeDuration);
                }
                else
                {
                    musicManager.SetMusicMode(MusicManager.MusicMode.normal);
                    uiManager.SetWarningIsOn(false);
                    uiManager.DisplayEndScreen(true);
                    gameOver = true;
                    towelBoyWins = true;
                    pausingForEmergency = false;
                }
            }
            else
            {
                pausingForEmergency = false;
                musicManager.SetMusicMode(MusicManager.MusicMode.normal);
                uiManager.SetWarningIsOn(false);
                emergencyModeTimer = 0f;
            }

            // towel time logic
            uiManager.SetTowelMeter(currentTowelTime / towelTime);
            //TowelBar.transform.localScale = new Vector3(TowelBar.transform.localScale.x, currentTowelTime / 10, TowelBar.transform.localScale.z);
            if (currentTowelTime < 0)
            {
                gameOver = true;
                poolBoyWins = true;
                uiManager.DisplayEndScreen(false);
            }
            if (!towel1 && !towel2)
            {
                if (!pausingForEmergency) currentTowelTime -= towelDecayRate * Time.deltaTime;
            }
            else if (towel1 ^ towel2)
            {
                if (towelGracePeriodTimer < towelGracePeriodDuration)
                {
                    if (!pausingForEmergency) towelGracePeriodTimer += Time.deltaTime;
                }
                else
                {
                    if (!pausingForEmergency) currentTowelTime -= towelDecayRate * Time.deltaTime;
                }
            }
            else
            {
                towelGracePeriodTimer = 0f;
            }

            // wetness logic
            for (int i = 0; i < 4; i++)
            {
                if (water[i / 2])
                {
                    wetnessFactor[i] = 2;
                }
                else
                {
                    wetnessFactor[i] = 1;
                }
            }

            // patron tanning logic
            for (int i = 0; i < 4; i++)
            {
                if (!patronReset[i])
                {
                    if (!pausingForEmergency) patronStatus[i] += tanningRate * umbrellaActive[i] * wetnessFactor[i] * Time.deltaTime * 0.3f;
                    uiManager.SetTemperature(i, patronStatus[i] / 20f + 0.5f);
                    if (patronStatus[i] > tanningMax)
                    {
                        // --- Reached upper limit, now Reset Patron
                        StartCoroutine(resetPatron(i));
                        sfxManager.SetPatronLeavingSFX(i, true);
                    }
                    else if (patronStatus[i] > tanningMax - tanningWarningBuffer) {
                        // --- Within the upper buffer, warn the player with hot emotion
                        uiManager.SetEmotion(i, UIManager.Emote.Hot);
                        sfxManager.SetPatronHotSFX(i, true);
                    }
                    else if (patronStatus[i] > tanningMin + tanningWarningBuffer)
                    {
                        // --- Within the happy zone, set patron emotion to happy
                        uiManager.SetEmotion(i, UIManager.Emote.Happy);
                        sfxManager.SetPatronHotSFX(i, false);
                        sfxManager.SetPatronColdSFX(i, false);
                        sfxManager.SetPatronLeavingSFX(i, false);
                    }
                    else if (patronStatus[i] > tanningMin)
                    {
                        // Within the lower buffer, warn the player with cold emotion
                        uiManager.SetEmotion(i, UIManager.Emote.Cold);
                        sfxManager.SetPatronColdSFX(i, true);
                    }
                    else
                    {
                        // --- Reached the lower limit, now Reset Patron
                        StartCoroutine(resetPatron(i));
                        sfxManager.SetPatronLeavingSFX(i, true);
                        //Patrons[i].GetComponent<SwitchTan>().setTan();
                    }
                }
            }

            // game timer
            if (!pausingForEmergency) gameTimer += Time.deltaTime;
            uiManager.SetCountdownTimerText( (int)gameLength - (int)gameTimer);
            if (gameTimer > gameLength)
            {
                gameOver = true;
                poolBoyWins = true;
                uiManager.DisplayEndScreen(false);
            }
        }
        else
        {
            if (!pausingForEmergency) gameOverTimer += Time.deltaTime;
            if (gameOverTimer > gameOverLength)
            {
                SceneManager.LoadScene(0);
            }

            if (poolBoyWins)
            {
                Debug.Log("Pool Boy Wins");
            }
            if (towelBoyWins)
            {
                Debug.Log("Towel Boy Wins");
            }
        }

    }

    // Manages game input
    // TODO: PROCESS ARDUINO INPUTS
    void HandleInput()
    {
        if (!enableTestHotkeys)
        {
            // MAIN FORM OF INPUT

            string message = serialController.ReadSerialMessage();

            if (message == null)
                return;

            // Check if the message is plain data or a connect/disconnect event.
            if (ReferenceEquals(message, SerialController.SERIAL_DEVICE_CONNECTED))
                Debug.Log("Connection established");
            else if (ReferenceEquals(message, SerialController.SERIAL_DEVICE_DISCONNECTED))
                Debug.Log("Connection attempt failed or disconnection detected");
            else
            {
                Debug.Log(message);
                arduinoValues = message.Split(',');
                for (int i = 0; i < 8; i++)
                {
                    if (arduinoValues[i] == "1")
                        arduinoConvertedValues[i] = true;
                    if (arduinoValues[i] == "0")
                        arduinoConvertedValues[i] = false;
                }
            }

            umbrella[0] = arduinoConvertedValues[0];
            umbrella[1] = arduinoConvertedValues[1];
            umbrella[2] = arduinoConvertedValues[2];
            umbrella[3] = arduinoConvertedValues[3];
            towel1 = arduinoConvertedValues[4];
            towel2 = arduinoConvertedValues[5];
            water[0] = arduinoConvertedValues[6];
            water[1] = arduinoConvertedValues[7];
            for (int i = 0; i < 4; i++)
            {
                if (umbrella[i]) umbrellaActive[i] = -1;
                else umbrellaActive[i] = 1;
            }
        }
        else
        {
            // INPUT FOR TESTING PURPOSES

            if (Input.GetKeyDown(KeyCode.Q))
            {
                towel1 = !towel1;
            }
            if (Input.GetKeyDown(KeyCode.P))
            {
                towel2 = !towel2;
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                water[0] = !water[0];
            }
            if (Input.GetKeyDown(KeyCode.O))
            {
                water[1] = !water[1];
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                umbrella[0] = !umbrella[0];
                umbrellaActive[0] *= -1;
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                umbrella[1] = !umbrella[1];
                umbrellaActive[1] *= -1;
            }
            if (Input.GetKeyDown(KeyCode.I))
            {
                umbrella[2] = !umbrella[2];
                umbrellaActive[2] *= -1;
            }
            if (Input.GetKeyDown(KeyCode.U))
            {
                umbrella[3] = !umbrella[3];
                umbrellaActive[3] *= -1;
            }
        }
    }

    // Switches Material based on status
    // TODO: CHANGE FROM JUST SWITCHING MATERIAL TO REFLECTING VALUES ON THE GAME UI
    void UpdateUI()
    {
        uiManager.SetTowels(towel1, towel2);
        uiManager.SetWetCover(water[0], water[1]);
        sfxManager.SetSplashSFX(0, water[0]);
        sfxManager.SetSplashSFX(1, water[1]);
        for (int i = 0; i < 4; i++) { uiManager.SetUmbrella(i, umbrella[i]); }


        if (towel1) TowelSensor1.GetComponent<SwitchMaterial>().setActiveMaterial();
        else TowelSensor1.GetComponent<SwitchMaterial>().setInactiveMaterial();
        if (towel2) TowelSensor2.GetComponent<SwitchMaterial>().setActiveMaterial();
        else TowelSensor2.GetComponent<SwitchMaterial>().setInactiveMaterial();
        if (water[0]) WaterSensors[0].GetComponent<SwitchMaterial>().setActiveMaterial();
        else WaterSensors[0].GetComponent<SwitchMaterial>().setInactiveMaterial();
        if (water[1]) WaterSensors[1].GetComponent<SwitchMaterial>().setActiveMaterial();
        else WaterSensors[1].GetComponent<SwitchMaterial>().setInactiveMaterial();
        if (umbrella[0]) UmbrellaSensors[0].GetComponent<SwitchMaterial>().setActiveMaterial();
        else UmbrellaSensors[0].GetComponent<SwitchMaterial>().setInactiveMaterial();
        if (umbrella[1]) UmbrellaSensors[1].GetComponent<SwitchMaterial>().setActiveMaterial();
        else UmbrellaSensors[1].GetComponent<SwitchMaterial>().setInactiveMaterial();
        if (umbrella[2]) UmbrellaSensors[2].GetComponent<SwitchMaterial>().setActiveMaterial();
        else UmbrellaSensors[2].GetComponent<SwitchMaterial>().setInactiveMaterial();
        if (umbrella[3]) UmbrellaSensors[3].GetComponent<SwitchMaterial>().setActiveMaterial();
        else UmbrellaSensors[3].GetComponent<SwitchMaterial>().setInactiveMaterial();
    }

    // resets the patron when they become unhappy
    public IEnumerator resetPatron(int i)
    {
        patronReset[i] = true;
        currentPoolBoyChances++;
        uiManager.SetEmotion(i, UIManager.Emote.Gone);
        yield return new WaitForSeconds(5f);
        patronStatus[i] = 0;
        uiManager.RandomizePatron(i);
        uiManager.SetEmotion(i, UIManager.Emote.Happy);
        patronReset[i] = false;
    }
}
