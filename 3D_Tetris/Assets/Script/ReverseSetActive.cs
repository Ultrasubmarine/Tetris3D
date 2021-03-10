using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReverseSetActive : MonoBehaviour
{
    public void SetActive(bool value)
    {
        gameObject.SetActive(!value);
    }
    }