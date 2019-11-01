using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameEvent
{
    UI_PLAY,

    CURRENT_HEIGHT,

    DESTROY_LAYER,
    CURRENT_SCORE,

    CHANGE_TIME_DROP,

    // AFTER Decomposition
    CREATE_NEW_ELEMENT,
    TURN_ELEMENT,
    MOVE_ELEMENT,
    END_DROP_ELEMENT,
    //

    END_CAMERA_ANIMATION,
}