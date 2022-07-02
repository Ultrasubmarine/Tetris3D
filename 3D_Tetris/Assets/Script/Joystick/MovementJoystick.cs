using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Script.Controller
{
    public enum JoystickState
    {
        Show,
        Hide,
    }
    
    public struct LastAction
    {
        public int count;
        public bool isReverseAction;
        public move direction;
        public bool? isSuccess;

        public void Reset()
        {
            isSuccess = null;
            isReverseAction = false;
            count = 0;
        }
    }
    
    [RequireComponent(typeof(CanvasGroup))]
    public class MovementJoystick: MonoBehaviour, IDragHandler, IPointerExitHandler
    {
        public bool isCenterReverseLastAction { get; set; }
        public JoystickState state { get; private set; }
        
        [SerializeField] private RectTransform _skickSpace;
        [SerializeField] private RectTransform _stick;
        [SerializeField] private float _spawnAnimationTime = 0.05f;
        
        [SerializeField][Range(0,1)] private float _minPercentRadiusForMove = 0.15f;
        [SerializeField][Range(0,3)] private float _maxPercentOutSpaceRadius = 2.5f;

        public Action<JoystickState> onStateChange;
        
        private CanvasGroup _canvasGroup;
        
        private SlowManager _slowManager;
        
        private bool _isStickCanDrag;

        private float _squareSpaceRadius;

        // for delta zone
        private float _squareMaxRadius;
        private float _maxR;
        
        //for move 
        private float _squareMinRadiusMove;
        [SerializeField] private float _minAngle = 10f;

        private Vector2 _correctPivotStickPosition;
        private GameController _gameController;

        private LastAction _lastAction;
        
        //block
        private bool _isBlock;
        
        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            
            var spaceR =  (_skickSpace.rect.size.x - _stick.rect.size.x) / 2;
            var minRForMove =  spaceR * _minPercentRadiusForMove;
            var maxR = spaceR * _maxPercentOutSpaceRadius;
            
            _squareSpaceRadius = spaceR * spaceR;
            _squareMinRadiusMove = minRForMove * minRForMove;
            _squareMaxRadius = maxR * maxR;

            _correctPivotStickPosition = new Vector2( (0.5f - _stick.pivot.x) * _stick.rect.width,
                (0.5f - _stick.pivot.y) * _stick.rect.height) ;
            
            _lastAction.isSuccess = null;
            isCenterReverseLastAction = false;
        }

        public void Block(bool isLocked)
        {
            _isBlock = isLocked;

            if (isLocked)
            {
                Hide();
            }
        }
        
        private void Start()
        {
            _slowManager = RealizationBox.Instance.slowManager;
            
            RealizationBox.Instance.tapsEvents.OnSingleTap += Spawn;
            
            _canvasGroup.alpha = 0;

            RealizationBox.Instance.FSM.AddListener(TetrisState.EndInfluence, OnEndInfluenseState);
            RealizationBox.Instance.FSM.AddListener(TetrisState.MergeElement, OnTetrisMergeElement);
            
            _gameController = RealizationBox.Instance.gameController;
            _gameController.onMoveApply += CheckSuccessMove;
        }

        private void OnTetrisMergeElement()
        {
            Hide();
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
            if (currentRadius < _squareSpaceRadius)
            {
                if(currentRadius > _squareMinRadiusMove)
                    CheckMove(Input.GetTouch(0).position);
            }
        }

        protected void OnJoystickStateChange(JoystickState newState)
        {
            state = newState;
            onStateChange?.Invoke(state);
        }
        
        public void Spawn()
        {
            if (_isBlock)
                return;
            
            if (Input.touchCount != 1)
                return;

            OnJoystickStateChange(JoystickState.Show);
            
            _slowManager.OnJoystickTouchChange(JoystickState.Show);
            _canvasGroup.DOFade(1, _spawnAnimationTime);
            _isStickCanDrag = true;
            
            _skickSpace.position = Input.GetTouch(0).position ;  
            _stick.position = _skickSpace.position;
        }

        public void Hide()
        {
            OnJoystickStateChange(JoystickState.Hide);
            
            _slowManager.OnJoystickTouchChange(JoystickState.Hide);
            _canvasGroup.DOFade(0, _spawnAnimationTime);
            _isStickCanDrag = false;

            _lastAction.Reset();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_isStickCanDrag)
                return;

            var x = eventData.position.x;
            var y = eventData.position.y;

            var correctStickPos = (Vector2)_skickSpace.position - _correctPivotStickPosition;
            var currentRadius = (x - correctStickPos.x) * (x - correctStickPos.x) + 
                                (y - correctStickPos.y) * (y - correctStickPos.y);
          
            if (currentRadius < _squareSpaceRadius)
                _stick.position = eventData.position;
            else
            {
                var spaceR =  (_skickSpace.rect.size.x - _stick.rect.size.x) / 2;
                _stick.position = (eventData.position - (Vector2)_skickSpace.position ).normalized * spaceR + (Vector2)_skickSpace.position - _correctPivotStickPosition;
            }
            if(currentRadius > _squareMinRadiusMove)
                CheckMove(eventData.position);
            else if (isCenterReverseLastAction)
                ReverseLastAction();
        }

        private void CheckMove(Vector2 position)
        {
            if (RealizationBox.Instance.FSM.GetCurrentState() != TetrisState.WaitInfluence)
                return;
            
            var currdirection = (position - new Vector2(_skickSpace.position.x, _skickSpace.position.y)).normalized;
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

            if (direct == _lastAction.direction)
            {
                if (_lastAction.isSuccess == false)
                    return;
                _lastAction.count++;
            }
            else
            {
                _lastAction.direction = direct;
                _lastAction.count = 1;
            }
            
            _lastAction.isReverseAction = false;
            _gameController.Move(direct);
            // находим четверть и говорим, куда перемещать объект
        }

        private void CheckSuccessMove(bool isSuccess, move direction)
        {
            _lastAction.isSuccess = isSuccess;
        }

        private void ReverseLastAction()
        {
            if (RealizationBox.Instance.FSM.GetCurrentState() != TetrisState.WaitInfluence)
                return;
            
            if (_lastAction.isReverseAction)
                return;

            if ( (_lastAction.count == 1 && _lastAction.isSuccess == false ) || _lastAction.count < 1 )
                return;
            
            move reversDirect = move.x;

            switch (_lastAction.direction)
            {
                case move.x:
                {
                    reversDirect = move.xm;
                    break;
                }
                case move.z:
                {
                    reversDirect = move.zm;
                    break;
                }
                case move.xm:
                {
                    reversDirect = move.x;
                    break;
                }
                case move.zm:
                {
                    reversDirect = move.z;
                    break;
                }
            }
            
            _lastAction.direction = reversDirect;
            _lastAction.isReverseAction = true;
            
            _gameController.Move(reversDirect);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Hide();
        }
    }
}