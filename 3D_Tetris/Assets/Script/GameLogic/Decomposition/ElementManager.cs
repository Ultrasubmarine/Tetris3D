﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementManager : MonoBehaviour {

    PlaneMatrix _matrix;
    [SerializeField] Speed _Speed;
    [SerializeField] Generator _Generator;
    [SerializeField] StateMachine machine;

    public List<Element> _elementMarger;

    static public Element NewElement;

    // Use this for initialization
    void Start () {
        _elementMarger = new List<Element>();
        _matrix = PlaneMatrix.Instance;

        Messenger.AddListener( StateMachine.StateMachineKey + GameState2.Empty, GenerateElement);
        Messenger.AddListener(StateMachine.StateMachineKey + GameState2.NewElement, StartDropElement);

        Messenger.AddListener(StateMachine.StateMachineKey + GameState2.DropAllElements, CutElement);
        Messenger.AddListener(StateMachine.StateMachineKey + GameState2.DropAllElements, StartDropAllElements);
    }

    private void OnDestroy() {
        Messenger.RemoveListener(StateMachine.StateMachineKey + GameState2.Empty, GenerateElement);
        Messenger.RemoveListener(StateMachine.StateMachineKey + GameState2.NewElement, StartDropElement);

        Messenger.RemoveListener(StateMachine.StateMachineKey + GameState2.DropAllElements, CutElement);
        Messenger.RemoveListener(StateMachine.StateMachineKey + GameState2.DropAllElements, StartDropAllElements);
    }

    public void GenerateElement() {

        GameObject generationElement = _Generator.GenerationNewElement(gameObject.transform);//_PlaneScript.transform);
        NewElement = generationElement.GetComponent<Element>();
        NewElement.gameObject.transform.parent = gameObject.transform;

        //_PlaneScript.NewElement = NewElement;

        machine.ChangeState(GameState2.NewElement);

    }

    #region  функции падения нового эл-та ( и его слияние)
    public void StartDropElement() {
        StartCoroutine(DropElement());
    }
    private IEnumerator DropElement() {

        //_PlaneScript.Mystate = planeState.emptyState;
   
        while (true) {

            while (machine.State != GameState2.NewElement){//_PlaneScript.Mystate == planeState.turnState || _PlaneScript.Mystate == planeState.moveState) {
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

        Destroy(_Generator.examleElement);

        while (machine.State != GameState2.NewElement) {//_PlaneScript.Mystate == planeState.turnState || _PlaneScript.Mystate == planeState.moveState) {
            yield return null;
        }
        //while (_PlaneScript.Mystate == planeState.moveState) {
        //    yield return null;
        //}
        MergeElement(NewElement); // слияние элемента и поля
        NewElement = null;
        machine.ChangeState(GameState2.Merge);

        // myProj.CreateCeiling();
        // // TODO - проверка что надо уничтожить
        yield break;
    }

    private void MergeElement( Element newElement) {

        _matrix.BindToMatrix(newElement);

        newElement.transform.parent = this.gameObject.transform;
        _elementMarger.Add(newElement);
    }
    #endregion

    #region  функции падения всех эл-тов ( после уничтожения слоев)
    public void StartDropAllElements() {
        StartCoroutine(DropAllElements());//_PlaneScript.DropAfterDestroy());
    }

    private IEnumerator DropAllElements() {
        bool flagDrop = false;
        bool checkDropState = true;

        do {
            flagDrop = StartAllElementDrop();

            if(flagDrop)                
                yield return new WaitUntil(AllElementsDrop);
            yield return new WaitForSeconds( _Speed._TimeDelay); // слишком резко уничтожаются 
        }
        while (flagDrop); // проверяем что бы все упало, пока оно может падать

     //   myProj.CreateCeiling();
        machine.ChangeState(GameState2.Collection);

        DestroyEmptyElement();
        yield return null;
    }

    private bool StartAllElementDrop() {

        bool flagDrop = false;
        foreach (var item in _elementMarger) {
            var empty = _matrix.CheckEmptyPlaсe(item, new Vector3Int(0, -1, 0));
            if (empty) //если коллизии нет, элемент может падать вниз
            {
                if (item.isBind)
                    _matrix.UnbindToMatrix(item);

                flagDrop = true;
                item.LogicDrop();
                StartCoroutine(item.VisualDrop(_Speed._TimeDropAfterDestroy)); // запускает падение элемента
            }
            else {
                if (!item.isBind)
                    _matrix.BindToMatrix(item);
            }
        }
        return flagDrop;
    }

    private bool AllElementsDrop() {

        foreach (var item in _elementMarger) {
            if (item.isDrop) {
                return false;
            }
        }
        return true;
        
    }

    #endregion

    public void CutElement() {

        int k = 0;
        int countK = _elementMarger.Count;
        while (k < countK) {
            Element b = _elementMarger[k].CheckUnion();
            if (b != null) {
                _matrix.UnbindToMatrix(b);
                _matrix.UnbindToMatrix(_elementMarger[k]);

                _elementMarger.Add(b);
                b.transform.parent = gameObject.transform;
                countK++;
            }
            k++;
        }
    }

    private void DestroyEmptyElement() {
        // проверка пустых элементов
        for (int i = 0; i < _elementMarger.Count;) {
            if (_elementMarger[i].CheckEmpty())
            {
                GameObject tmp = _elementMarger[i].gameObject;
                _elementMarger.Remove(_elementMarger[i]);
                Destroy(tmp);
            }
            else {
                i++;
            }
        }
    }

    public void DestroyAllElements() {
        while (_elementMarger.Count > 0) {
            Element tmp = _elementMarger[0];
            _matrix.UnbindToMatrix(tmp);
            _elementMarger.Remove(tmp);
            Destroy(tmp.gameObject);
        }
    }
    
}
