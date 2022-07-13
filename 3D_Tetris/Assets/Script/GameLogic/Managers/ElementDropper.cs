using System;
using Script.GameLogic.GameItems;
using Script.Influence;
using UnityEngine;

namespace Script.GameLogic.TetrisElement
{
    public class ElementDropper : MonoBehaviour
    {
        public bool isWaitingMerge => _isWaitingMerge;
        
        private TetrisFSM _fsm;
        
        private PlaneMatrix _matrix;

        private ElementData _elementData;
        
        private InfluenceManager _influence;

        private int _dropElementCount;

        private ElementCleaner _cleaner;

        private float _allDropFast = 1;
        
        private bool _isWaitingMerge = false;
        [SerializeField] private float _waitMergeTime = 0.4f;
        private Vector3Int[] vectorDirection;

        public Action OnDropEndCallback;
        
        private void Start()
        {
            _matrix = RealizationBox.Instance.matrix;
            _fsm = RealizationBox.Instance.FSM;
            _influence = RealizationBox.Instance.influenceManager;
            _cleaner = RealizationBox.Instance.elementCleaner;
            _elementData = ElementData.Instance;
            
            vectorDirection = new Vector3Int[4];
            vectorDirection[0] = new Vector3Int(1, 0, 0);
            vectorDirection[1] = new Vector3Int(-1, 0, 0);
            vectorDirection[2] = new Vector3Int(0, 0, 1);
            vectorDirection[3] = new Vector3Int(0, 0, -1);
        }

        #region  функции падения нового эл-та ( и его слияние)

        public bool WaitMerge()
        {
            if (_isWaitingMerge)
                return false;

            if (RealizationBox.Instance.influenceManager.fastSpeed)
                return false;
            
            bool can = false;
            foreach (var p in vectorDirection)
            {
                if (_matrix.CheckEmptyPlaсe( _elementData.newElement, p, false))
                {
                    can = true;
                    break;
                };
            }
            if (!can)
                return false;

            _isWaitingMerge = true;
            Invoke(nameof(CallDrop), _waitMergeTime);
            
            return true;
        }
        
        public void StartDropElement()
        {
            _isWaitingMerge = false;
            _elementData.newElement.LogicDrop();
            _influence.AddDrop( _elementData.newElement.myTransform, Vector3.down, global::Speed.timeDrop, CallDrop, false, true);
            
            var pickableBlocks = _matrix.GetPickableBlocksForElement( _elementData.newElement);
            foreach (var pBlock in pickableBlocks)
            {
                pBlock.Pick( _elementData.newElement);
                _matrix.UnbindBlock(pBlock);
                _cleaner.DeletePickableBlock((PickableBlock)pBlock);
            }
        }

        private void CallDrop()
        {
            if (_fsm.GetCurrentState() == TetrisState.Restart)
                return;
            
            if (! _elementData.newElement)
                return;
            
            OnDropEndCallback?.Invoke();
            
            if(_fsm.GetCurrentState() != TetrisState.WaitInfluence)
                InfluenceData.delayedDrop = true;
            else
                _fsm.SetNewState(TetrisState.Drop);
        }

        #endregion

        #region  функции падения всех эл-тов ( после уничтожения слоев)
        public void StartDropAllElements()
        {
            var countDropElements = DropAllElements();
            if (countDropElements > 0)
                return;

            _fsm.SetNewState(TetrisState.Collection);
        }

        public void SetAllDropFastSpeed(float fast)
        {
            _allDropFast = fast;
        }
        private int DropAllElements()
        {
            _dropElementCount = 0;
            foreach (var item in  _elementData.mergerElements)
            {
                var empty = _matrix.CheckEmptyPlaсe(item, new Vector3Int(0, -1, 0));
                if (empty && !item.isFreeze) //если коллизии нет, элемент может падать вниз
                {
                    if (item._isBind)
                        _matrix.UnbindToMatrix(item);

                    _dropElementCount++;
                    item.LogicDrop();

                    _influence.AddDrop(item.myTransform, Vector3.down, global::Speed.timeDropAfterDestroy * _allDropFast,
                        DecrementDropElementsCount, true);
                    
                    var pickableBlocks = _matrix.GetPickableBlocksForElement(item);
                    foreach (var pBlock in pickableBlocks)
                    {
                        pBlock.Pick(item);
                        _matrix.UnbindBlock(pBlock);
                        _cleaner.DeletePickableBlock((PickableBlock)pBlock);
                    }
                }
                else
                {
                    if (!item._isBind)
                        _matrix.BindToMatrix(item);
                }
            }
            
            return _dropElementCount;
        }

        private void DecrementDropElementsCount()
        {
            _dropElementCount--;
            if (_dropElementCount == 0)
                StartDropAllElements();
        }

        #endregion
    }
}