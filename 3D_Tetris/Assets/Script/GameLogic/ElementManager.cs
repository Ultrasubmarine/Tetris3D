using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ElementManager : MonoBehaviour {

    PlaneMatrix _matrix;
    [SerializeField] Generator _Generator;
    [SerializeField] StateMachine machine;

    List<Element> _elementMarger;

    static public Element NewElement;
    Transform _myTransform;

    // Use this for initialization
    void Start () {
        _elementMarger = new List<Element>();
        _matrix = PlaneMatrix.Instance;
        _myTransform = this.transform;

        Messenger.AddListener( StateMachine.StateMachineKey + EMachineState.Empty, GenerateElement);
        Messenger.AddListener(StateMachine.StateMachineKey + EMachineState.NewElement, StartDropElement);

        Messenger.AddListener(StateMachine.StateMachineKey + EMachineState.DropAllElements, AfterCollectElement);
    }

    private void OnDestroy() {
        Messenger.RemoveListener(StateMachine.StateMachineKey + EMachineState.Empty, GenerateElement);
        Messenger.RemoveListener(StateMachine.StateMachineKey + EMachineState.NewElement, StartDropElement);

        Messenger.RemoveListener(StateMachine.StateMachineKey + EMachineState.DropAllElements, AfterCollectElement);
    }

    public void GenerateElement() {

        NewElement = _Generator.GenerationNewElement(_myTransform);
        NewElement.MyTransform.parent = _myTransform;

        machine.ChangeState(EMachineState.NewElement);
        Messenger<Element>.Broadcast(GameEvent.CREATE_NEW_ELEMENT.ToString(), NewElement);
    }

    #region  функции падения нового эл-та ( и его слияние)
    public void StartDropElement() {
        StartCoroutine(DropElement());
    }
    
    private IEnumerator DropElement() {

        while (true) {
            while (machine.State != EMachineState.NewElement){
                yield return null;
            }
            bool empty = _matrix.CheckEmptyPlaсe(NewElement, new Vector3Int(0, -1, 0)); 

            if (empty)
            {
                NewElement.LogicDrop(); 
            }
            else
                break;

            yield return StartCoroutine(NewElement.VisualDrop(Speed.TimeDrop));
        }

//        Destroy(_Generator.examleElement);

        while (machine.State != EMachineState.NewElement) {
            yield return null;
        }
        
        Messenger<Element>.Broadcast(GameEvent.END_DROP_ELEMENT.ToString(), NewElement);
        MergeElement(NewElement);
        NewElement = null;
        machine.ChangeState(EMachineState.Merge);
        
        yield break;
    }

    private void MergeElement( Element newElement) {

        _matrix.BindToMatrix(newElement);

        newElement.transform.parent = this.gameObject.transform;
        _elementMarger.Add(newElement);
    }
    #endregion 
  
    public void AfterCollectElement() {
        
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
    
    public void DeleteAllElements() {
        while (_elementMarger.Count > 0) {
            Element tmp = _elementMarger[0];
            
            _matrix.UnbindToMatrix(tmp);
            _elementMarger.Remove(tmp);
            _Generator.DeleteElement(tmp);
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
        machine.ChangeState(EMachineState.Collection);

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
                StartCoroutine(item.VisualDrop(Speed.TimeDropAfterDestroy)); // запускает падение элемента
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
