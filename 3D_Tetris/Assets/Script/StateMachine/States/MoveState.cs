﻿using Helper.Patterns.FSM;
using Script.Controller;
using Script.GameLogic.TetrisElement;
using Script.Influence;
using Script.ObjectEngine;
using UnityEngine;
namespace Script.StateMachine.States
{
    public class MoveState : AbstractState<TetrisState>
    {
        private InfluenceManager _influence;
        private PlaneMatrix _matrix;
        private GameController _gameController;
        
        public MoveState()
        {
            _influence = RealizationBox.Instance.influenceManager;
            _matrix = RealizationBox.Instance.matrix;
            _gameController = RealizationBox.Instance.gameController;
        }
        
        public override void Enter(TetrisState last)
        {
            if (CheckOpportunity(ElementData.newElement, InfluenceData.direction))
            {
                _gameController.OnMoveActionApply(true, InfluenceData.direction);
                Logic(InfluenceData.direction, ElementData.newElement);

                Vector3Int vectorDirection = SetVectorMove(InfluenceData.direction);

                _influence.AddMove(ElementData.newElement, vectorDirection, global::Speed.timeMove,
                    () =>
                    {
                        _FSM.SetNewState(TetrisState.EndInfluence);
                    });
                base.Enter(last);
            }
            else
            {
                _gameController.OnMoveActionApply(false, InfluenceData.direction);
                base.Enter(last);
                _FSM.SetNewState(TetrisState.EndInfluence);
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
            else if (direction == move.xm)
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
                foreach (var item in element.blocks)
                    item.OffsetCoordinates(1, 0, 0);
            else if (direction == move.xm)
                foreach (var item in element.blocks)
                    item.OffsetCoordinates(-1, 0, 0);
            else if (direction == move.z)
                foreach (var item in element.blocks)
                    item.OffsetCoordinates(0, 0, 1);
            else if (direction == move.zm)
                foreach (var item in element.blocks)
                    item.OffsetCoordinates(0, 0, -1);
        }
    }
}