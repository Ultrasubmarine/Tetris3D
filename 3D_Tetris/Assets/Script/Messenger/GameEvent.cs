using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameEvent
{
   END_GAME,
   WIN_GAME,

   UI_PLAY,

   CURRENT_HEIGHT,

   DESTROY_LAYER,
   CURRENT_SCORE,

   MOVE,
   TURN,

   CHANGE_TIME_DROP,

   GAME_OVER,

    // AFTER Decomposition
    CREATE_NEW_ELEMENT,
    TURN_ELEMENT,
    MOVE_ELEMENT,
    END_DROP_ELEMENT,
    //
}
