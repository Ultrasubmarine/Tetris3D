using System.Collections.Generic;
using Helper.Patterns;
using JetBrains.Annotations;

namespace CustomUpdate
{
    public class CustomUpdater : Singleton<CustomUpdater>, IUpdating
    {
        Dictionary<UpdatingMode, List<IUpdatable>> _updatableObjects = new Dictionary<UpdatingMode, List<IUpdatable>>();
        
        [CanBeNull] private IUpdatable[] _simpleUpdatables;
        [CanBeNull] private IUpdatable[] _fixedUpdatables;
        [CanBeNull] private IUpdatable[] _lateUpdatables;
        
        protected override void Init()
        {
            base.Init();
            _updatableObjects[UpdatingMode.Simple] = new List<IUpdatable>();
            _updatableObjects[UpdatingMode.Fixed] = new List<IUpdatable>();
            _updatableObjects[UpdatingMode.Late] = new List<IUpdatable>();
        }

        private void Update()
        {
            if ( ReferenceEquals(_simpleUpdatables, null))
                return;
            for (int i = 0; i < _simpleUpdatables.Length; i++)
            {
                _simpleUpdatables[i].UpdateMe();
            }
        }
        
        private void FixedUpdate()
        {
            if (ReferenceEquals(_fixedUpdatables, null))
                return;
            for (int i = 0; i < _fixedUpdatables.Length; i++)
            {
                _fixedUpdatables[i].UpdateMe();
            }
        }
        
        private void LateUpdate()
        {
            if (ReferenceEquals(_lateUpdatables, null))
                return;
            for (int i = 0; i < _lateUpdatables.Length; i++)
            {
                _lateUpdatables[i].UpdateMe();
            }
        }

        public void AddUpdatableItem(IUpdatable item,  UpdatingMode mode = UpdatingMode.Simple)
        {
            if( !_updatableObjects[mode].Contains( item))
                _updatableObjects[mode].Add(item);
            UpdateArray(mode);
        }

        public void RemoveUpdateItem(IUpdatable item, UpdatingMode mode = UpdatingMode.Simple)
        {
            _updatableObjects[mode].Remove(item);
            UpdateArray(mode);
        }

        private void UpdateArray(UpdatingMode mode)
        {
            switch (mode)
            {
                case UpdatingMode.Simple :
                { 
                    _simpleUpdatables = _updatableObjects[mode].ToArray();
                    break; 
                }
                case UpdatingMode.Fixed :
                {
                    _fixedUpdatables = _updatableObjects[mode].ToArray();
                    break;
                }
                case UpdatingMode.Late :
                {
                    _lateUpdatables = _updatableObjects[mode].ToArray();
                    break;
                }
            }
        }
    }
}
