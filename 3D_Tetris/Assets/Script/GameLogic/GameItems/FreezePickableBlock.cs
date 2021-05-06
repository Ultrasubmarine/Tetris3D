using UnityEngine;

namespace Script.GameLogic.GameItems
{
    public class FreezePickableBlock : PickableBlock
    {
        public override void Pick(Element element)
        {
            base.Pick(element);
            element.isFreeze = true;

            foreach (var block in element.blocks)
            {
                block.mesh.material = RealizationBox.Instance.generator.freezeMaterial;
            }

            RealizationBox.Instance.slowManager.FreezeElementSlowOn();
        }
    }
}