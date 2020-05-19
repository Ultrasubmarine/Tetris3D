using UnityEngine;

namespace Script.Booster
{
    [CreateAssetMenu(fileName = "FreezeBooster", menuName = "ScriptableObjects/Booster/FreezeBooster", order = 1)]
    public class FreezeBooster : BoosterBase
    {
        private SlowManager _slowManager;


        [SerializeField] private float _slowTime;

        [SerializeField] private float _slowValue;

        public override void Initialize()
        {
            base.Initialize();
            _slowManager = RealizationBox.Instance.slowManager;
            _useTime = _slowTime;
        }

        public override void Apply()
        {
            if (_currentState == BoosterState.ReadyForUse)
                SetState(BoosterState.UseWithCountdown);

            _slowManager.AddedSlow(_slowTime, _slowValue);
        }

        public override void EndApply()
        {
            base.EndApply();
        }
    }
}