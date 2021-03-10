using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterCameraSwitch : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private MeshRenderer water;

    public void OrthographicOn()
    {
        Debug.Log("ON ON ON");
        water.materials[0].SetInt("BOOLEAN_537AC313_ON", 1);
    }
    
    public void OrthographicOff()
    {
        water.materials[0].SetInt("BOOLEAN_537AC313_ON", 0);
    }
}
