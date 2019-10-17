using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ElementManager : MonoBehaviour
{
    private TetrisFSM myFSM;
    PlaneMatrix _matrix;
    [SerializeField] Generator _Generator;
//    [FormerlySerializedAs("machine")] [SerializeField] StateMachine _Machine;

    List<Element> _elementMarger;

    static public Element NewElement;
    Transform _myTransform;

    private bool _defferedDrop;
    
    void Start () {
        _elementMarger = new List<Element>();
        _matrix = PlaneMatrix.Instance;
        _myTransform = this.transform;

        myFSM = RealizationBox.Instance.FSM;

//        Messenger.AddListener( StateMachine.StateMachineKey + EMachineState.Empty, GenerateElement);
//        Messenger.AddListener(StateMachine.StateMachineKey + EMachineState.NewElement, StartDropElement);
//
//        Messenger.AddListener(StateMachine.StateMachineKey + EMachineState.DropAllElements, AfterCollectElement);
// 
//        Messenger.AddListener(StateMachine.StateMachineKey + EMachineState.NotActive, DeleteAllElements);
//        Messenger.AddListener("EndVizual", AfterEndVisual);
//        Messenger.AddListener(StateMachine.StateMachineKey + EMachineState.EndInfluence, CheckDelayDrop);
    }

    private void OnDestroy() {
//        Messenger.RemoveListener(StateMachine.StateMachineKey + EMachineState.Empty, GenerateElement);
//        Messenger.RemoveListener(StateMachine.StateMachineKey + EMachineState.NewElement, StartDropElement);
//
//        Messenger.RemoveListener(StateMachine.StateMachineKey + EMachineState.DropAllElements, AfterCollectElement);
//        
//        Messenger.RemoveListener(StateMachine.StateMachineKey + EMachineState.NotActive, DeleteAllElements);
    }

    #region  функции падения нового эл-та ( и его слияние)

    public void StartDropElement() {
        DropElement();
    }

    public void AfterEndVisual()
    {
//        if (_Machine.State != EMachineState.NewElement)
        {
            Debug.Log("ADD deferred Drop");
            _defferedDrop = true;
            return;
        }
        DropElement();
    }
    
    private void DropElement() {
        
        NewElement.LogicDrop();
        // TODO change DOTween method
        NewElement.transform.DOMove( NewElement.transform.position +  Vector3.down, Speed.TimeDrop).SetEase( Ease.Linear).OnComplete( CallFSMDrop);//.DropInOneLayer();
    }

    private void CallFSMDrop()
    {
        Debug.Log(" отложенный вызов отложенного дропа");
        myFSM.SetNewState(TetrisState.Drop);
    }
    
    public void CheckDelayDrop()
    {
//        _Machine.ChangeState(EMachineState.NewElement, false);
        Debug.Log(" CheckDelayDrop");
        if (_defferedDrop)
        {
            _defferedDrop = false;
            Debug.Log("USE deferred drop");
            DropElement();
        }
    }

    public void MergeNewElement() {

        _matrix.BindToMatrix(NewElement);
        
        _elementMarger.Add(NewElement);
        NewElement = null;
    }
    #endregion

    void AfterCollectElement() {
        
        ClearElementsAfterDeletedBlocks();
        CutElement();
        StartCoroutine(StartDropAllElements());
    }

    #region функции удаления
    private void ClearElementsAfterDeletedBlocks() {
        
        foreach (var element in _elementMarger) {
            var deletedList = element.MyBlocks.Where(s => s.IsDestroy).ToArray();
            if (deletedList.ToArray().Length > 0) {               
                element.DeleteBlocksInList(deletedList);
                ClearDeleteBlocks(deletedList);
            }
        }
        DeleteEmptyElement();
    }
    
    private void ClearDeleteBlocks(Block[] deletedList) {
        foreach (var item in deletedList) {
            _Generator.DeleteBlock(item);
        }        
    }
    
    private void DeleteEmptyElement() {       
        for (int i = 0; i < _elementMarger.Count;) {
            if (_elementMarger[i].CheckEmpty())
            {
                Element tmp = _elementMarger[i];
                _elementMarger.Remove(_elementMarger[i]);
               
                _Generator.DeleteElement(tmp); 
            }
            else {
                i++;
            }
        }
    }

    void DeleteAllElements() {
        while (_elementMarger.Count > 0) {
            Element tmp = _elementMarger[0];
            
            _matrix.UnbindToMatrix(tmp);
            _elementMarger.Remove(tmp);
            
            ClearDeleteBlocks( tmp.MyBlocks.ToArray() );
            tmp.DeleteBlocksInList( tmp.MyBlocks.ToArray() );
            _Generator.DeleteElement(tmp);
        }
        if (!Equals(NewElement, null)) {
            
            ClearDeleteBlocks(NewElement.MyBlocks.ToArray());
            NewElement.DeleteBlocksInList( NewElement.MyBlocks.ToArray() );
            _Generator.DeleteElement(NewElement);
            NewElement = null;
        }
            
    }
    #endregion
    
    private void CutElement() {

        int k = 0;
        int countK = _elementMarger.Count;
        while (k < countK) {
            List<Block> cutBlocks = _elementMarger[k].CheckUnion();
            if (cutBlocks != null) {
                Element newElement = _Generator.CreateEmptyElement();
                newElement.MyTransform.position = _elementMarger[k].MyTransform.position;
                newElement.MyBlocks = cutBlocks;
                foreach (var block in newElement.MyBlocks)
                {
                    block.MyTransform.parent = newElement.MyTransform;
                }
                
                _matrix.UnbindToMatrix(newElement);
                _matrix.UnbindToMatrix(_elementMarger[k]);

                _elementMarger.Add(newElement);
                newElement.MyTransform.parent = _myTransform;
                countK++;
            }
            k++;
        }
    }
    
    #region  функции падения всех эл-тов ( после уничтожения слоев)

    private IEnumerator StartDropAllElements() {
        bool flagDrop = false;
        do {
            flagDrop = DropAllElements();

            if(flagDrop)                
                yield return new WaitUntil(CheckAllElementsDrop);
            yield return new WaitForSeconds( Speed.TimeDelay); // слишком резко уничтожаются 
        }
        while (flagDrop); // проверяем что бы все упало, пока оно может падать

        //   myProj.CreateCeiling();
//        _Machine.ChangeState(EMachineState.Collection);

        yield return null;
    }

    private bool DropAllElements() {
        bool flagDrop = false;
        foreach (var item in _elementMarger) {
            var empty = _matrix.CheckEmptyPlaсe(item, new Vector3Int(0, -1, 0));
            if (empty) //если коллизии нет, элемент может падать вниз
            {
                if (item.IsBind)
                    _matrix.UnbindToMatrix(item);

                flagDrop = true;
                item.LogicDrop();
                item.VisualDrop(Speed.TimeDropAfterDestroy); // запускает падение элемента
            }
            else {
                if (!item.IsBind)
                    _matrix.BindToMatrix(item);
            }
        }
        return flagDrop;
    }

    private bool CheckAllElementsDrop() {
        foreach (var item in _elementMarger) {
            if (item.IsDrop) {
                return false;
            }
        }
        return true;     
    }

    #endregion
}
