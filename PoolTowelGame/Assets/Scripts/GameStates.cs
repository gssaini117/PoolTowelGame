using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStates : MonoBehaviour
{
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

    // Towel Values pt.2 
    public float towelTime;
    private float currentTowelTime = 0f; // actual value
    public float towelDecayRate;
    public float towelGracePeriodDuration;
    private float towelGracePeriodTimer; // actual value

    // Emergency Values
    public float emergencyModeDuration;
    private float emergencyModeTimer; // actual value

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
        UpdateMaterials();
    }

    // Fixed Update is called once per frame at 60 fps
    void FixedUpdate()
    {
        if (!gameOver)
        {

            // pool boy chances logic
            if (currentPoolBoyChances > 3)
            {
                gameOver = true;
                towelBoyWins = true;
            }

            // emergency mode logic
            if (water[0] && water[1])
            {
                if (emergencyModeTimer < emergencyModeDuration)
                {
                    emergencyModeTimer += Time.deltaTime;
                }
                else
                {
                    gameOver = true;
                    towelBoyWins = true;
                }
            }
            else
            {
                emergencyModeTimer = 0f;
            }

            // towel time logic
            //Debug.Log(currentTowelTime);
            TowelBar.transform.localScale = new Vector3(TowelBar.transform.localScale.x, currentTowelTime / 10, TowelBar.transform.localScale.z);
            // ^^ towel bar implementation
            if (currentTowelTime < 0)
            {
                gameOver = true;
                poolBoyWins = true;
            }
            if (!towel1 && !towel2)
            {
                currentTowelTime -= towelDecayRate * 2 * Time.deltaTime;
            }
            else if (towel1 ^ towel2)
            {
                if (towelGracePeriodTimer < towelGracePeriodDuration)
                {
                    towelGracePeriodTimer += Time.deltaTime;
                }
                else
                {
                    currentTowelTime -= towelDecayRate * Time.deltaTime;
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
                    patronStatus[i] += tanningRate * umbrellaActive[i] * wetnessFactor[i] * Time.deltaTime;
                    if (patronStatus[i] > tanningMax)
                    {
                        Patrons[i].GetComponent<SwitchTan>().setBurnt();
                        StartCoroutine(resetPatron(i));
                    }
                    else if (patronStatus[i] < tanningMin)
                    {
                        Patrons[i].GetComponent<SwitchTan>().setPale();
                        StartCoroutine(resetPatron(i));
                    }
                    else
                    {
                        Patrons[i].GetComponent<SwitchTan>().setTan();
                    }
                }
            }

            // game timer
            gameTimer += Time.deltaTime;
            if (gameTimer > gameLength)
            {
                gameOver = true;
                poolBoyWins = true;
            }
        }
        else
        {
            gameOverTimer += Time.deltaTime;
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

    // Switches Material based on status
    // TODO: CHANGE FROM JUST SWITCHING MATERIAL TO REFLECTING VALUES ON THE GAME UI
    void UpdateMaterials()
    {
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
        yield return new WaitForSeconds(2f);
        Patrons[i].GetComponent<SwitchTan>().setAngry();
        yield return new WaitForSeconds(3f);
        patronStatus[i] = 0;
        patronReset[i] = false;
    }
}
