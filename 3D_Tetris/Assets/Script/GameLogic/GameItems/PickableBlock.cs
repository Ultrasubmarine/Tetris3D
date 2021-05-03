using UnityEngine;

namespace Script.GameLogic.GameItems
{
    public class PickableBlock : Block
    {
        public override void Pick(Element element)
        {
            Debug.Log("U pick Pickable block!");
        }

        public override bool IsPickable()
        {
            return true;
        }
    }
}