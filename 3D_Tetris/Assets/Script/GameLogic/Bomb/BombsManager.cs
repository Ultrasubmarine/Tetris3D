using System;
using System.Collections.Generic;
using IntegerExtension;
using UnityEngine;

namespace Script.GameLogic.Bomb
{
    public class BombsManager : MonoBehaviour
    {   
        private GameLogicPool _pool;
        private TetrisFSM _fsm;
        
       [SerializeField] private Material _material;
       [SerializeField] private Mesh _bombMesh;

       [SerializeField] private bool _ignoreSlow = true;

       private List<Block> _bombs;

       [SerializeField] private List<Vector3Int> _directions;

       [SerializeField] private int _stepForBomb;
       private int _currentStep = 1000;
        private void Start()
        {
            _pool = RealizationBox.Instance.gameLogicPool;
            _fsm = RealizationBox.Instance.FSM;

            _bombs = new List<Block>();
        }

        public Element MakeBomb()
        {
            if (_currentStep < _stepForBomb)
            {
                _currentStep++;
                return null;
            }
            else
            {
                _currentStep = 0;
            }
            
            var element = _pool.CreateEmptyElement();
            _pool.CreateBlock(Vector3Int.zero, element, _material);

            element.blocks[0].TransformToBomb(_bombMesh, _material);
            _bombs.Add(element.blocks[0]);
            return element;
        }

        public bool BoomBombs()
        {
            bool boom = false;
            foreach (var b in _bombs)
            {
                RealizationBox.Instance.matrix.DestroyBlocksAround(b._coordinates.ToIndex(), _directions);
                boom = true;
            }
            _bombs.Clear();
            return boom;
        }
    }
}