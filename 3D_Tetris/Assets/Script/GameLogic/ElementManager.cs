using System.Collections.Generic;
using System.Linq;
using Script.ObjectEngine;
using UnityEngine;

public class ElementManager : MonoBehaviour
{
    private TetrisFSM _myFSM;
    private PlaneMatrix _matrix;
    private Generator _generator;

    List<Element> _elementMarger;

    static public Element NewElement;
    Transform _myTransform;

    private bool _defferedDrop;
    private InfluenceManager _influence;

    private int _dropElementCount;
    private int _endDrop;
    void Start () {
        _elementMarger = new List<Element>();
     
        _myTransform = this.transform;

        _matrix = RealizationBox.Instance.Matrix();
        _myFSM = RealizationBox.Instance.FSM;
        _generator = RealizationBox.Instance.ElementGenerator();
        _influence = RealizationBox.Instance.InfluenceManager;
    }
    
    private void OnDestroy() {

//        Messenger.RemoveListener(StateMachine.StateMachineKey + EMachineState.NotActive, DeleteAllElements);
    }

    #region  функции падения нового эл-та ( и его слияние)

    public void StartDropElement() {
        NewElement.LogicDrop();
        _influence.AddMove( NewElement.MyTransform, Vector3.down, Speed.TimeDrop, CallDrop);
    }

    private void CallDrop() => _myFSM.SetNewState(TetrisState.Drop);
    
    public void CheckDelayDrop()
    {
//        _Machine.ChangeState(EMachineState.NewElement, false);
        Debug.Log(" CheckDelayDrop");
        if (_defferedDrop)
        {
            _defferedDrop = false;
            Debug.Log("USE deferred drop");
            StartDropElement();
        }
    }

    public void MergeNewElement() {

        _matrix.BindToMatrix(NewElement);
        
        _elementMarger.Add(NewElement);
        NewElement = null;
    }
    #endregion

    #region функции удаления

    public void ClearElementsAfterDeletedBlocks() {
        
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
            _generator.DeleteBlock(item);
        }        
    }
    
    private void DeleteEmptyElement() {       
        for (int i = 0; i < _elementMarger.Count;) {
            if (_elementMarger[i].CheckEmpty())
            {
                Element tmp = _elementMarger[i];
                _elementMarger.Remove(_elementMarger[i]);
               
                _generator.DeleteElement(tmp); 
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
            _generator.DeleteElement(tmp);
        }
        if (!Equals(NewElement, null)) {
            
            ClearDeleteBlocks(NewElement.MyBlocks.ToArray());
            NewElement.DeleteBlocksInList( NewElement.MyBlocks.ToArray() );
            _generator.DeleteElement(NewElement);
            NewElement = null;
        }
            
    }
    #endregion

    public void CutElement() {

        int k = 0;
        int countK = _elementMarger.Count;
        while (k < countK) {
            List<Block> cutBlocks = _elementMarger[k].CheckUnion();
            if (cutBlocks != null) {
                Element newElement = _generator.CreateEmptyElement();
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

    public void StartDropAllElements() {
  
        var countDropElements= DropAllElements();
        if (countDropElements > 0)
            return;

        RealizationBox.Instance.FSM.SetNewState( TetrisState.Collection);
    }

    private int DropAllElements()
    {
        _dropElementCount = 0;
        foreach (var item in _elementMarger) {
            var empty = _matrix.CheckEmptyPlaсe(item, new Vector3Int(0, -1, 0));
            if (empty) //если коллизии нет, элемент может падать вниз
            {
                if (item.IsBind)
                    _matrix.UnbindToMatrix(item);

                _dropElementCount++;
                item.LogicDrop();
                
                _influence.AddMove( item.MyTransform, Vector3.down, Speed.TimeDropAfterDestroy, DecrementDropElementsCount);
            }
            else {
                if (!item.IsBind)
                    _matrix.BindToMatrix(item);
            }
        }
        return _dropElementCount;
    }

    private void DecrementDropElementsCount()
    {
        _dropElementCount--;
        if (_dropElementCount == 0)
            StartDropAllElements();
    }

    #endregion
}
