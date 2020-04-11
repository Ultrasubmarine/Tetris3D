using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using Script.Controller;

public class TutorialForUser : Listener<move>
{
    private static float time = 0.2f;

    [Header("TICK")] [SerializeField]
    private List<GameObject> CheckMotion = new List<GameObject>(Enum.GetValues(typeof(move)).Length);

    [SerializeField] private UnityEvent FinishLearn;

    private void Awake()
    {
        CallDelegate = ListenEvent;
    }

    public void ListenEvent(move param)
    {
        CheckMotion[(int) param].SetActive(true);
        if (CheckMotion.Where(s => s.active).ToArray().Length == CheckMotion.Count) FinishLearn.Invoke();
    }

    private void OnEnable()
    {
        GameController.MoveTutorial = true;
    }

    private void OnDisable()
    {
        GameController.MoveTutorial = false;
    }

    public void HideAfterWait(GameObject obj)
    {
        StartCoroutine(Tools.SetActiveAfterWait(obj, time + 0.2f, false));
    }

    public void OpenAfterWait(GameObject obj)
    {
        StartCoroutine(Tools.SetActiveAfterWait(obj, time, true));
    }
}

// определенные функции c особыми действиями определяются в статик классе 
internal static class SpecificTutorial
{
    public static void SetTick(this TutorialForUser temp, move t)
    {
    }
}