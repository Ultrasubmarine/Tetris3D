using System;
using Script.Controller;
using Script.GameLogic.TetrisElement;
using Script.Influence;
using UnityEngine;

public class FastElementDrop : MonoBehaviour
{
    private MovementJoystick _joystick;
    private InfluenceManager _influenceManager;

    // Start is called before the first frame update
    void Start()
    {
        _influenceManager = RealizationBox.Instance.influenceManager;
        _joystick = RealizationBox.Instance.joystick;

        ElementData.Instance.onMergeElement += ResetFastSpeed;
        RealizationBox.Instance.tapsEvents.OnDoubleTap += SetFastSpeed;
    }

    public void SetFastSpeed()
    {
        // _joystick.Hide();
        // _joystick.enabled = false;
        _influenceManager.SetSpeedMode(true);
    }

    public void ResetFastSpeed()
    {
        // _joystick.enabled = true;
         _influenceManager.SetSpeedMode(false);
    }

    private void OnDestroy()
    {
        ElementData.Instance.onMergeElement -= ResetFastSpeed;
    }
}
