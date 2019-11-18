using System.Collections.Generic;
using Helper.Patterns;
using IntegerExtension;
using UnityEngine;

namespace Script.Projections
{
    public class Ceiling : MonoBehaviour
    {
        private TetrisFSM _fsm;
        private PlaneMatrix _matrix;
        
        [SerializeField] private GameObject _prefab;
        
        [SerializeField] private float _HeightProjection = 0.1f;
        [SerializeField] private int _MinimumLayerHeight;
        
        private List<GameObject> _ceilingList = new List<GameObject>();
        private Pool<GameObject> _pool;
        

        private void Awake()
        { 
    //        Messenger<int,int>.AddListener(GameEvent.CURRENT_HEIGHT.ToString(), CreateCeiling);
    //
    //        Messenger<Element>.AddListener(GameEvent.END_DROP_ELEMENT.ToString(), DeleteProjection);
    //        
    //        Messenger.AddListener(StateMachine.StateMachineKey + EMachineState.NotActive, ClearAllProjections);
    //        Messenger.AddListener(StateMachine.StateMachineKey + EMachineState.Win, ClearAllProjections);
    //        Messenger.AddListener(StateMachine.StateMachineKey + EMachineState.End, ClearAllProjections);
        }

        private void OnDestroy()
        {
            //        
    //        Messenger<int,int>.RemoveListener(GameEvent.CURRENT_HEIGHT.ToString(), CreateCeiling);
    //       
    //        Messenger<Element>.RemoveListener(GameEvent.END_DROP_ELEMENT.ToString(), DeleteProjection);
    //        
    //        Messenger.RemoveListener(StateMachine.StateMachineKey + EMachineState.NotActive, ClearAllProjections);
    //        Messenger.RemoveListener(StateMachine.StateMachineKey + EMachineState.Win, ClearAllProjections);
    //        Messenger.RemoveListener(StateMachine.StateMachineKey + EMachineState.End, ClearAllProjections);
        }

        private void Start()
        {
            _matrix = PlaneMatrix.Instance;
            _fsm = RealizationBox.Instance.FSM;

            _pool = new Pool<GameObject>(_prefab);
            Invoke(nameof(LastStart), 1f);
        }

        private void LastStart()
        {
            _fsm.AddListener(TetrisState.Collection, CreateCeiling);
        }
        
        private void CreateCeiling()
        {
            Debug.Log("coco");
            Destroy();
            
            Debug.Log($" currentHeight {_matrix.currentHeight} < _min {_MinimumLayerHeight}");
            if (_matrix.currentHeight < _MinimumLayerHeight)
                return;

            for (var x = 0; x < _matrix.wight; x++)
            for (var z = 0; z < _matrix.wight; z++)
            {
                var y = _matrix.MinHeightInCoordinates(x, z);
                if (y >= _MinimumLayerHeight)
                {
                    var o = _pool.Pop();
                    o.transform.position = 
                        new Vector3(x.ToCoordinat(), _matrix.limitHeight + _HeightProjection, z.ToCoordinat());

                    _ceilingList.Add(o);
                }
            }
        }
        
        private void Destroy()
        {
            foreach (var item in _ceilingList) _pool.Push(item);
            _ceilingList.Clear();
        }
    }
}