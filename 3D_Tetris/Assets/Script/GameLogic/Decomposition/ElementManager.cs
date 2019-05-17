using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementManager : MonoBehaviour {

    [SerializeField] PlaneScript _PlaneScript;
    PlaneMatrix _matrix;
    [SerializeField] Generator _Generator;
    [SerializeField] StateMachine machine;

    //TODO delete
    [SerializeField] HeightHandler _HeightHandler;
    [SerializeField] Projection myProj;
    //

    public List<ElementScript> _elementMarger;

    public ElementScript NewElement;

    // Use this for initialization
    void Start () {
        _elementMarger = new List<ElementScript>();
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

        GameObject generationElement = _Generator.GenerationNewElement(_PlaneScript.transform);
        NewElement = generationElement.GetComponent<ElementScript>();
        NewElement.gameObject.transform.parent = _PlaneScript.gameObject.transform;

        _PlaneScript.NewElement = NewElement;

        machine.ChangeState(GameState2.NewElement);
    }

    #region  функции падения нового эл-та ( и его слияние)
    public void StartDropElement() {
        StartCoroutine(DropElement());
    }
    private IEnumerator DropElement() {

        _PlaneScript.Mystate = planeState.emptyState;
        myProj.CreateProjection(NewElement);
        
        while (true) {

            while (_PlaneScript.Mystate == planeState.turnState || _PlaneScript.Mystate == planeState.moveState) {
                yield return null;
            }

            bool empty = _matrix.CheckEmptyPlaсe(NewElement, new Vector3Int(0, -1, 0)); // проверяем может ли элемент упасть на ярус ниже

            if (empty)
            {
                NewElement.DropElement(this.gameObject); // логическое изменение координат падающего элемента
            }
            else
                break;

            yield return StartCoroutine(NewElement.DropElementVisual(NewElement.gameObject.transform.position.y - 1.0f, _PlaneScript._TimeDrop));// элемент визуально падает
        }

        Destroy(_Generator.examleElement);

        while (_PlaneScript.Mystate == planeState.moveState) {
            yield return null;
        }
        MergeElement(NewElement); // слияние элемента и поля
        NewElement = null;
        machine.ChangeState(GameState2.Merge);

        // myProj.CreateCeiling();
        // // TO DO - проверка что надо уничтожить
        yield break;
    }

    private void MergeElement( ElementScript newElement) {

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
            yield return new WaitForSeconds( _PlaneScript._TimeDelay); // слишком резко уничтожаются 
        }
        while (flagDrop); // проверяем что бы все упало, пока оно может падать

        myProj.CreateCeiling();
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
                item.DropElement(this.gameObject);
                StartCoroutine(item.DropElementVisual(item.gameObject.transform.position.y - 1.0f, _PlaneScript._TimeDropAfterDestroy)); // запускает падение элемента
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
            ElementScript b = _elementMarger[k].CheckUnion();
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
            if (_elementMarger[i].CheckEmptyElement())
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
            ElementScript tmp = _elementMarger[0];
            _matrix.UnbindToMatrix(tmp);
            _elementMarger.Remove(tmp);
            Destroy(tmp.gameObject);
        }
    }
    
}
