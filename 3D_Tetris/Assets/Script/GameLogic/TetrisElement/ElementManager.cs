using System.Collections.Generic;
using System.Linq;
using Script.ObjectEngine;
using UnityEngine;

namespace Script.GameLogic.TetrisElement
{
    public class ElementManager : MonoBehaviour
    {
        private TetrisFSM _myFSM;
        private PlaneMatrix _matrix;

        Transform _myTransform;

        private bool _defferedDrop;
        private InfluenceManager _influence;

        private int _dropElementCount;
        private int _endDrop;
        void Start () {
            _myTransform = this.transform;

            _matrix = RealizationBox.Instance.Matrix();
            _myFSM = RealizationBox.Instance.FSM;
            _influence = RealizationBox.Instance.InfluenceManager;
        }
    
        private void OnDestroy() {

//        Messenger.RemoveListener(StateMachine.StateMachineKey + EMachineState.NotActive, DeleteAllElements);
        }

        #region  функции падения нового эл-та ( и его слияние)

        public void StartDropElement() {
            ElementData.NewElement.LogicDrop();
            _influence.AddMove( ElementData.NewElement.MyTransform, Vector3.down, Speed.TimeDrop, CallDrop);
        }

        private void CallDrop() => _myFSM.SetNewState(TetrisState.Drop);
    
        public void CheckDelayDrop()
        {
//        _Machine.ChangeState(EMachineState.NewElement, false);
            Debug.Log(" CheckDelayDrop");
            if (_defferedDrop)
            {
                _defferedDrop = false;
                Debug.Log("USE deferred drop");
                StartDropElement();
            }
        }

        public void MergeNewElement() {

            _matrix.BindToMatrix(ElementData.NewElement);
            ElementData.MergeNewElement();
        }
        #endregion
        
        #region  функции падения всех эл-тов ( после уничтожения слоев)

        public void StartDropAllElements() {
  
            var countDropElements= DropAllElements();
            if (countDropElements > 0)
                return;

            RealizationBox.Instance.FSM.SetNewState( TetrisState.Collection);
        }

        private int DropAllElements()
        {
            _dropElementCount = 0;
            foreach (var item in ElementData.MergerElements) {
                var empty = _matrix.CheckEmptyPlaсe(item, new Vector3Int(0, -1, 0));
                if (empty) //если коллизии нет, элемент может падать вниз
                {
                    if (item.IsBind)
                        _matrix.UnbindToMatrix(item);

                    _dropElementCount++;
                    item.LogicDrop();
                
                    _influence.AddMove( item.MyTransform, Vector3.down, Speed.TimeDropAfterDestroy, DecrementDropElementsCount);
                }
                else {
                    if (!item.IsBind)
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
