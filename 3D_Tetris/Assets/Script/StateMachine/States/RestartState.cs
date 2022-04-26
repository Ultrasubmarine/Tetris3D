using Helper.Patterns.FSM;
using UnityEngine.PlayerLoop;

namespace Script.StateMachine.States
{
    public class RestartState: AbstractState<TetrisState>
    {
        public override void Enter(TetrisState last)
        {
            base.Enter(last);
            
            RealizationBox.Instance.starUIAnimation.Clear();  
            RealizationBox.Instance.starsManager.Clear();

            RealizationBox.Instance.destroyedLayerParticles.ClearAll();
            RealizationBox.Instance.islandTurn.ResetTurn();
            RealizationBox.Instance.projectionLineManager.Clear();              
            RealizationBox.Instance.projection.Destroy();                       
                    
            RealizationBox.Instance.influenceManager.ClearAllInfluences();  
            RealizationBox.Instance.elementCleaner.DeleteAllElements(); 
            RealizationBox.Instance.generator.Clear();
            RealizationBox.Instance.matrix.Clear();                             
                                                                      
            RealizationBox.Instance.slowManager.DeleteAllSlows();               
            RealizationBox.Instance.lvlElementsSetter.CreateElements();         
         //   OnReplay?.Invoke();                                                 
            RealizationBox.Instance.haightHandler.CalculateHeight();            
            RealizationBox.Instance.gameCamera.SetPositionWithoutAnimation();   
            RealizationBox.Instance.speedChanger.ResetSpeed();                  
            RealizationBox.Instance.generatorChanger.ResetGenerator();
            
          //  _FSM.StartFSM();
        }

        public override void Exit(TetrisState last)
        {
            
        }
    }
}