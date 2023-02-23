using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStates : MonoBehaviour
{
    // Game Objects
    public GameObject TowelSensor1;
    private bool towel1;
    public GameObject TowelSensor2;
    private bool towel2;
    public GameObject WaterSensor1;
    private bool water1;
    public GameObject WaterSensor2;
    private bool water2;
    public GameObject UmbrellaSensor1;
    private bool umbrella1;
    public GameObject UmbrellaSensor2;
    private bool umbrella2;
    public GameObject UmbrellaSensor3;
    private bool umbrella3;
    public GameObject UmbrellaSensor4;
    private bool umbrella4;
    public GameObject Person1;
    private float personStatus1;
    public GameObject Person2;
    private float personStatus2;
    public GameObject Person3;
    private float personStatus3;
    public GameObject Person4;
    private float personStatus4;

    // Constants
    public float gameLength;
    private float gameTimer = 0f;
    public int poolBoyChances;
    private int currentPoolBoyChances = 0;
    public float tanningRate;
    public float tanningMin;
    public float tanningMax;
    public float towelTime;
    private float currentTowelTime = 0f;
    public float towelDecayRate;
    public float towelGracePeriod;
    private float towelGracePeriodStart = 0f;
    public float emergencyModePeriod;
    private float emergencyModePeriodStart = 0f;

    // Start is called before the first frame update
    void Start()
    {
        personStatus1 = 0f;
        personStatus2 = 0f;
        personStatus3 = 0f;
        personStatus4 = 0f;

        gameLength = 180f;
        poolBoyChances = 3;
        tanningRate = 0.2f;
        tanningMin = -10f;
        tanningMax = 10f;
        towelTime = 30f;
        towelDecayRate = 1f;
        towelGracePeriod = 5f;
        emergencyModePeriod = 10f;
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
        // program logic using these numbers
    }

    // Manages game input
    // TODO: PROCESS ARDUINO INPUTS
    void HandleInput()
    {
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
            water1 = !water1;
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            water2 = !water2;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            umbrella1 = !umbrella1;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            umbrella2 = !umbrella2;
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            umbrella3 = !umbrella3;
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            umbrella4 = !umbrella4;
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
        if (water1) WaterSensor1.GetComponent<SwitchMaterial>().setActiveMaterial();
        else WaterSensor1.GetComponent<SwitchMaterial>().setInactiveMaterial();
        if (water2) WaterSensor2.GetComponent<SwitchMaterial>().setActiveMaterial();
        else WaterSensor2.GetComponent<SwitchMaterial>().setInactiveMaterial();
        if (umbrella1) UmbrellaSensor1.GetComponent<SwitchMaterial>().setActiveMaterial();
        else UmbrellaSensor1.GetComponent<SwitchMaterial>().setInactiveMaterial();
        if (umbrella2) UmbrellaSensor2.GetComponent<SwitchMaterial>().setActiveMaterial();
        else UmbrellaSensor2.GetComponent<SwitchMaterial>().setInactiveMaterial();
        if (umbrella3) UmbrellaSensor3.GetComponent<SwitchMaterial>().setActiveMaterial();
        else UmbrellaSensor3.GetComponent<SwitchMaterial>().setInactiveMaterial();
        if (umbrella4) UmbrellaSensor4.GetComponent<SwitchMaterial>().setActiveMaterial();
        else UmbrellaSensor4.GetComponent<SwitchMaterial>().setInactiveMaterial();
    }
}
