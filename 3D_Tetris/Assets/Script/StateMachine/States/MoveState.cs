using Helper.Patterns.FSM;
using Script.GameLogic.TetrisElement;
using Script.Influence;
using Script.ObjectEngine;
using UnityEngine;

namespace Script.StateMachine.States
{
    public enum move
    {
        x = 0,
        z = 1,
        _x = 2,
        _z = 3
    }
    
    public class MoveState : AbstractState<TetrisState>
    {
        private InfluenceManager _influence;
        private PlaneMatrix _matrix;
        
        public MoveState()
        {
            _influence = RealizationBox.Instance.influenceManager;
            _matrix = RealizationBox.Instance.matrix;
        }
        
        public override void Enter(TetrisState last)
        {
            if (CheckOpportunity(ElementData.NewElement, InfluenceData.direction))
            {
                Logic(InfluenceData.direction, ElementData.NewElement);

                Vector3Int vectorDirection = SetVectorMove(InfluenceData.direction);

                _influence.AddMove(ElementData.NewElement, vectorDirection, Speed.TimeMove,
                    () => { _FSM.SetNewState(TetrisState.EndInfluence); });
                base.Enter(last);
            }
            else
            {
                base.Enter(last);
                _FSM.SetNewState(TetrisState.WaitInfluence);
            }
        }

        public override void Exit(TetrisState last)
        {
        }

        private bool CheckOpportunity(Element element, move direction)
        {
            if (element == null) return false;
            var vectorDirection = SetVectorMove(direction);
            return _matrix.CheckEmptyPlaсe(element, vectorDirection);
        }
        
        private Vector3Int SetVectorMove(move direction)
        {
            Vector3Int vectorDirection;
            if (direction == move.x)
                vectorDirection = new Vector3Int(1, 0, 0);
            else if (direction == move._x)
                vectorDirection = new Vector3Int(-1, 0, 0);
            else if (direction == move.z)
                vectorDirection = new Vector3Int(0, 0, 1);
            else // (direction == move._z)
                vectorDirection = new Vector3Int(0, 0, -1);

            return vectorDirection;
        }
        
        private void Logic(move direction, Element element)
        {
            if (direction == move.x)
                foreach (var item in element.MyBlocks)
                    item.OffsetCoordinates(1, 0, 0);
            else if (direction == move._x)
                foreach (var item in element.MyBlocks)
                    item.OffsetCoordinates(-1, 0, 0);
            else if (direction == move.z)
                foreach (var item in element.MyBlocks)
                    item.OffsetCoordinates(0, 0, 1);
            else if (direction == move._z)
                foreach (var item in element.MyBlocks)
                    item.OffsetCoordinates(0, 0, -1);
        }
    }
}