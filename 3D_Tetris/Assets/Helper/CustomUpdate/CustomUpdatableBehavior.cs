using UnityEngine;

namespace CustomUpdate
{
    public class CustomUpdatableBehavior : MonoBehaviour, IUpdatable
    {
        protected UpdatingMode _updatingMode = UpdatingMode.Simple;
        
        private void Start()
        {
            EnableUpdate();
            CustomStart();
        }
        
        public virtual void CustomStart()
        {
        }
        
        private void OnEnable()
        {
            if (ReferenceEquals(CustomUpdater.Instance, null))
                return;
            EnableUpdate();
        }

        private void OnDisable()
        {
            if (ReferenceEquals(CustomUpdater.Instance, null))
                return;
            DisableUpdate();
        }

        public void EnableUpdate()
        {
            CustomUpdater.Instance.AddUpdatableItem( this, _updatingMode);
        }

        public void DisableUpdate()
        {
            if (ReferenceEquals(CustomUpdater.Instance, null))
                return;
            CustomUpdater.Instance.RemoveUpdateItem( this, _updatingMode);
        }

        public virtual void UpdateMe()
        {
            Debug.LogError("CustomUpdatableBehavior.UpdateMe() not override");
        }
    }
}
