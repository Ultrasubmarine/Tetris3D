using Script.Controller;
using Script.StateMachine.States;

namespace Script.GameLogic.TetrisElement
{
    public enum InfluenceType
    {
        Move,
        Turn,
    }
    
    public static class InfluenceData
    {
        public static bool delayedDrop { get; set; }
        
        public static move direction { get; set; }
    }
}