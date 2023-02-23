using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchTan : MonoBehaviour
{
    [SerializeField] Material paleMaterial;
    [SerializeField] Material tanMaterial;
    [SerializeField] Material burntMaterial;

    public void setPale()
    {
        GetComponent<Renderer>().material = paleMaterial;
    }

    public void setTan()
    {
        GetComponent<Renderer>().material = tanMaterial;
    }

    public void setBurnt()
    {
        GetComponent<Renderer>().material = burntMaterial;
    }
}
