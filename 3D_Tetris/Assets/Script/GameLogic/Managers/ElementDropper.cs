using Script.Influence;
using Script.ObjectEngine;
using UnityEngine;

namespace Script.GameLogic.TetrisElement
{
    public class ElementDropper : MonoBehaviour
    {
        private TetrisFSM _fsm;
        private PlaneMatrix _matrix;

        private Transform _transform;

        private bool _defferedDrop;
        private InfluenceManager _influence;

        private int _dropElementCount;

        private void Start()
        {
            _transform = transform;

            _matrix = RealizationBox.Instance.matrix;
            _fsm = RealizationBox.Instance.FSM;
            _influence = RealizationBox.Instance.influenceManager;
        }

        #region  функции падения нового эл-та ( и его слияние)

        public void StartDropElement()
        {
            ElementData.NewElement.LogicDrop();
            _influence.AddDrop(ElementData.NewElement.MyTransform, Vector3.down, Speed.TimeDrop, CallDrop);
        }

        private void CallDrop()
        {
            _fsm.SetNewState(TetrisState.Drop);
        }

        public void CheckDelayDrop()
        {
//        _Machine.ChangeState(EMachineState.NewElement, false);
            if (_defferedDrop)
            {
                _defferedDrop = false;
                StartDropElement();
            }
        }
        #endregion

        #region  функции падения всех эл-тов ( после уничтожения слоев)
        public void StartDropAllElements()
        {
            var countDropElements = DropAllElements();
            if (countDropElements > 0)
                return;

            RealizationBox.Instance.FSM.SetNewState(TetrisState.Collection);
        }

        private int DropAllElements()
        {
            _dropElementCount = 0;
            foreach (var item in ElementData.MergerElements)
            {
                var empty = _matrix.CheckEmptyPlaсe(item, new Vector3Int(0, -1, 0));
                if (empty) //если коллизии нет, элемент может падать вниз
                {
                    if (item.IsBind)
                        _matrix.UnbindToMatrix(item);

                    _dropElementCount++;
                    item.LogicDrop();

                    _influence.AddDrop(item.MyTransform, Vector3.down, Speed.TimeDropAfterDestroy,
                        DecrementDropElementsCount);
                }
                else
                {
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