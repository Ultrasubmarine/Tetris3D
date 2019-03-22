using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class TutorialForUser: Listener<move>
{

    static private float time = 0.2f;
    [Header("TICK")]
    [SerializeField] List<GameObject> CheckMotion = new List<GameObject>( Enum.GetValues(typeof(move)).Length);

    [SerializeField] UnityEvent FinishLearn;

    private void Awake()
    {
        CallDelegate = ListenEvent;
    }
    public void ListenEvent(move param)
    {
        CheckMotion[(int)param].SetActive(true);
        if (CheckMotion.Where(s => s.active).ToArray().Length == CheckMotion.Count)
        {
            FinishLearn.Invoke();
        }

    }

    private void OnEnable()
    {
        ControllerScript.MoveTutorial = true;
    }

    private void OnDisable()
    {
        ControllerScript.MoveTutorial = false;
    }
    public void HideAfterWait( GameObject obj)
    {
        StartCoroutine(Tools.SetActiveAfterWait(obj, time + 0.2f, false ));
    }

    public void OpenAfterWait(GameObject obj)
    {
        StartCoroutine(Tools.SetActiveAfterWait(obj, time, true));
    }
}

// определенные функции c особыми действиями определяются в статик классе 
static class SpecificTutorial
{
    public static void SetTick(this TutorialForUser temp, move t)
    {

    }

}


