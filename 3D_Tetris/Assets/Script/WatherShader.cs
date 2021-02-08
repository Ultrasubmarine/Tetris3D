using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WatherShader : MonoBehaviour
{
    [SerializeField] private MeshRenderer water;

    [SerializeField] private Slider Depth;
    [SerializeField] private Slider FoamAmount;
    [SerializeField] private Slider FoamCoff;

    [SerializeField] private Text _DepthText;
    [SerializeField] private Text _Depth2Text;
    [SerializeField] private Text _FoamCoffText;
    
    // Start is called before the first frame update
    void Start()
    {
        Depth.onValueChanged.AddListener(UpdateDepth);
        FoamAmount.onValueChanged.AddListener(UpdateFoamAmount);
        FoamCoff.onValueChanged.AddListener(UpdateFoamCoff);
        
    }

    // Update is called once per frame
    void UpdateDepth(float newValue)
    {
        water.materials[0].SetFloat("Depth", newValue);
        _DepthText.text = ("Depth:" + water.materials[0].GetFloat("Depth"));
    }
    
    void UpdateFoamAmount(float newValue)
    {
        water.materials[0].SetFloat("Foam_Amount", newValue);
        Debug.Log("Amount");
    }
    
    void UpdateFoamCoff(float newValue)
    {
        water.materials[0].SetFloat("Foam_Cutoff", newValue);
        Debug.Log("Coff");
    }
}
