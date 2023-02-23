using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchMaterial : MonoBehaviour
{
    [SerializeField] Material inactiveMaterial;
    [SerializeField] Material activeMaterial;

    public void setInactiveMaterial()
    {
        GetComponent<Renderer>().material = inactiveMaterial;
    }

    public void setActiveMaterial()
    {
        GetComponent<Renderer>().material = activeMaterial;
    }
}
