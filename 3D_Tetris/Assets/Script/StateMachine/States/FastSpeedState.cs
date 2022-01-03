using Helper.Patterns.FSM;
using Script.Controller;

namespace Script.StateMachine.States
{
    public class SpeedDropState : AbstractState<TetrisState>
    {
        private MovementJoystick _joystick;

        public SpeedDropState()
        {
            _joystick = RealizationBox.Instance.joystick;
        }
        
        public override void Enter(TetrisState last)
        {
            _joystick.Hide();
            _joystick.enabled = false;
            base.Enter(last);
        }

        public override void Exit(TetrisState last)
        {
            _joystick.enabled = true;
        }
    }
}