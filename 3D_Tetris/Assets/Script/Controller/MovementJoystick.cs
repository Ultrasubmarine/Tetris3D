using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Script.Controller
{
    
    public struct EndAction
    {
        public int count;
        public move direction;
    }
    
    [RequireComponent(typeof(CanvasGroup))]
    public class MovementJoystick: MonoBehaviour, IDragHandler, IDropHandler
    {
        public event Action<move> onPointEnter;
        
        [SerializeField] private RectTransform _skickSpace;
        [SerializeField] private RectTransform _stick;
        [SerializeField] private float _spawnTime = 0.05f;
        
        [SerializeField] private List<MovePointUi> _points;
        
        private CanvasGroup _canvasGroup;

        private MoveTouchController _moveTouchController;
        
        private bool _isStickCanDrag;

        private float _radius2;
        

        // for delta zone
        private float _deltaRadius2;
        private float _delta;
        
        //for move 
        private float _minRadiusMove;
        private float _minAngle = 20f;

        private GameController _gameController;

        private EndAction _endAction;
        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();

            _radius2 = (_skickSpace.rect.size.x - _stick.rect.size.x) / 2;
            _minRadiusMove = _radius2 / 2;
            _delta   = _radius2 * 3;
            _radius2 *= _radius2;
            _minRadiusMove *= _minRadiusMove;

            _deltaRadius2 = _delta * _delta;
            
            for (var i = 0; i < _points.Count; i++)
            {
                _points[i].SetIndex(i);
                _points[i].onPointEnter += OnMovePointUiTouch;
            }

            _endAction.count = 0;
        }

        private void Start()
        {
            _moveTouchController = RealizationBox.Instance.moveTouchController;
            _moveTouchController.onStateChanged += OnMoveTouchControllerStateChange;
            _canvasGroup.alpha = 0;

            RealizationBox.Instance.FSM.AddListener(TetrisState.EndInfluence, OnEndInfluenseState);
            RealizationBox.Instance.FSM.AddListener(TetrisState.MergeElement, OnTetrisMergeElement);
            
            _gameController = RealizationBox.Instance.gameController;
        }

        private void OnTetrisMergeElement()
        {
            Hide();
        }
        private void OnTetrisNewelement()
        {
            _endAction.count = 0;
            OnEndInfluenseState();
        }
        private void OnEndInfluenseState()
        {
            if (!_isStickCanDrag)
                return;

            if (Input.touchCount < 1)
                return;
            
            var x = Input.GetTouch(0).position.x;
            var y = Input.GetTouch(0).position.y;

            var currentRadius = (x - _skickSpace.position.x) * (x - _skickSpace.position.x) + 
                                (y - _skickSpace.position.y) * (y - _skickSpace.position.y);
            if (currentRadius < _radius2)
            {
                if(currentRadius > _minRadiusMove)
                    CheckMove(Input.GetTouch(0).position);
            }
            
        }
        
        private void OnMoveTouchControllerStateChange( MoveTouchController.StateTouch stateTouch)
        {
            switch (stateTouch)
            {
                case MoveTouchController.StateTouch.open:
                {
                    Spawn();
                    break;
                }
                case MoveTouchController.StateTouch.none:
                {
                    Hide();
                    break;
                }
            }
        }
        
        public void Spawn()
        {
            _canvasGroup.DOFade(1, _spawnTime);
            _isStickCanDrag = true;
            
            _skickSpace.position = Input.GetTouch(0).position;  
            _stick.position = _skickSpace.position;
            EventSystem.current.SetSelectedGameObject(gameObject);
        }

        public void Hide()
        {
            _canvasGroup.DOFade(0, _spawnTime);
            _isStickCanDrag = false;
            _endAction.count = 0;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_isStickCanDrag)
                return;

            var x = eventData.position.x;
            var y = eventData.position.y;

            var currentRadius = (x - _skickSpace.position.x) * (x - _skickSpace.position.x) + 
                                (y - _skickSpace.position.y) * (y - _skickSpace.position.y);
            if (currentRadius < _radius2)
            {
                _stick.position = eventData.position;
                
                if(currentRadius > _minRadiusMove)
                    CheckMove(eventData.position);
            }
            else
            {
                if (currentRadius> _deltaRadius2)
                    Hide();
            }
        }

        private void CheckMove(Vector2 position)
        {
            if (RealizationBox.Instance.FSM.GetCurrentState() != TetrisState.WaitInfluence)
                return;
            
            var currdirection = (position - new Vector2(_skickSpace.position.x, _skickSpace.position.y)).normalized;
          //  Debug.Log(currdirection);
            var angle = Vector2.Angle(currdirection, Vector2.right);

            
            if (!(angle > _minAngle && angle < 180 - _minAngle ) ) //  отсекаем горизонталь
                return;

            if ((angle > 90 - _minAngle && angle < 90 + _minAngle)) // отсекаем вертикаль
                return;
            
            if (currdirection.y < 0)
                angle *= -1;

            angle /= 90;

            move direct;
            if(angle > 0 && angle < 1)
                direct = move.xm;
            else if (angle > 1)
                direct = move.zm;
            else if(angle < 0 && angle > -1)
                direct = move.z;
            else 
                direct = move.x;
            
     

            if (direct == _endAction.direction)
            {
                if (_endAction.count >= RealizationBox.Instance.matrix.wight)
                    return;
                _endAction.count++;
            }
            else
            {
                _endAction.direction = direct;
                _endAction.count = 1;
            }
            
            _gameController.Move(direct);
            // находим четверть и говорим, куда перемещать объект
            // Debug.Log(angle);
        }
        private void OnMovePointUiTouch(MovePointUi point)
        {
            
            onPointEnter?.Invoke(point.direction);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log("Enter");
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log("Down");
        }

        public void OnSelect(BaseEventData eventData)
        {
            Debug.Log("SELECT");
        }

        public void OnDrop(PointerEventData eventData)
        {
            
            Debug.Log("DROP");
        }
    }
}