using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Tools
{
    public static IEnumerator SetActiveAfterWait(GameObject obj, float time, bool setActive)
    {
        yield return new WaitForSeconds(time);
        obj.SetActive(setActive);
    }
}