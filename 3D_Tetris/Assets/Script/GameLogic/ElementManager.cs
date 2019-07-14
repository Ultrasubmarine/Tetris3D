using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ElementManager : MonoBehaviour {

    PlaneMatrix _matrix;
    [SerializeField] Speed _Speed;
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

        Messenger.AddListener( StateMachine.StateMachineKey + GameState2.Empty, GenerateElement);
        Messenger.AddListener(StateMachine.StateMachineKey + GameState2.NewElement, StartDropElement);

        Messenger.AddListener(StateMachine.StateMachineKey + GameState2.DropAllElements, AfterCollectElement);
    }

    private void OnDestroy() {
        Messenger.RemoveListener(StateMachine.StateMachineKey + GameState2.Empty, GenerateElement);
        Messenger.RemoveListener(StateMachine.StateMachineKey + GameState2.NewElement, StartDropElement);

        Messenger.RemoveListener(StateMachine.StateMachineKey + GameState2.DropAllElements, AfterCollectElement);
    }

    public void GenerateElement() {

        NewElement = _Generator.GenerationNewElement(_myTransform);
        NewElement.MyTransform.parent = _myTransform;

        machine.ChangeState(GameState2.NewElement);
    }

    #region  функции падения нового эл-та ( и его слияние)
    public void StartDropElement() {
        StartCoroutine(DropElement());
    }
    
    private IEnumerator DropElement() {

        while (true) {
            while (machine.State != GameState2.NewElement){
                yield return null;
            }
            bool empty = _matrix.CheckEmptyPlaсe(NewElement, new Vector3Int(0, -1, 0)); // проверяем может ли элемент упасть на ярус ниже

            if (empty)
            {
                NewElement.LogicDrop(); // логическое изменение координат падающего элемента
            }
            else
                break;

            yield return StartCoroutine(NewElement.VisualDrop(_Speed._TimeDrop));// элемент визуально падает
        }

//        Destroy(_Generator.examleElement);

        while (machine.State != GameState2.NewElement) {
            yield return null;
        }
     
        MergeElement(NewElement); // слияние элемента и поля
        NewElement = null;
        machine.ChangeState(GameState2.Merge);

        // myProj.CreateCeiling();
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
            yield return new WaitForSeconds( _Speed._TimeDelay); // слишком резко уничтожаются 
        }
        while (flagDrop); // проверяем что бы все упало, пока оно может падать

        //   myProj.CreateCeiling();
        machine.ChangeState(GameState2.Collection);

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
                StartCoroutine(item.VisualDrop(_Speed._TimeDropAfterDestroy)); // запускает падение элемента
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
