using System;
using UnityEngine;

namespace Script.GameLogic.GameItems
{
    public class PickableBlock : Block
    {
        public Action<PickableBlock> onPick;
        
        public override void Pick(Element element)
        {
            Debug.Log("U pick Pickable block!");
            onPick.Invoke(this);
        }

        public override bool IsPickable()
        {
            return true;
        }
    }
}