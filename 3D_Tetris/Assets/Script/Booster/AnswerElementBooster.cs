using UnityEngine;

namespace Script.Booster
{
    [CreateAssetMenu(fileName = "AnswerElementBooster", menuName = "ScriptableObjects/Booster/AnswerElementBooster", order = 1)]
    public class AnswerElementBooster : BoosterBase
    {
        private Generator _generator;

        public override void Initialize()
        {
            base.Initialize();
            _generator = RealizationBox.Instance.generator;
        }

        public override void Apply()
        {
            base.Apply();
          //  _generator.ShowAnswerElement();
        }
    }
}