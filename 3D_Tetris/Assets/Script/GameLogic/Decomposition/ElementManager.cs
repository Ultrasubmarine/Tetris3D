using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementManager : MonoBehaviour {

    PlaneScript _PlaneScript;
    PlaneMatrix _matrix;
    public List<ElementScript> _elementMarger;

    public ElementScript NewElement;

    // Use this for initialization
    void Start () {
        _elementMarger = new List<ElementScript>();
        _matrix = PlaneMatrix.Instance;
    }
	
    public void GenerateElement() {

    }

    public IEnumerator DropElement() {
        while (true) {
         
            while (_PlaneScript.Mystate == planeState.turnState || _PlaneScript.Mystate == planeState.moveState) {
                yield return null;// мы не можем спустить элемент на метр ниже, пока у нас идет визуальный поворот или перемещение. ждем пока он закончится
            }

            bool empty = _matrix.CheckEmptyPlaсe(NewElement, new Vector3Int(0, -1, 0)); // проверяем может ли элемент упасть на ярус ниже

            if (empty)//!collision)
            {
                NewElement.DropElement(this.gameObject); // логическое изменение координат падающего элемента
            }
            else
                break;

            yield return StartCoroutine(NewElement.DropElementVisual(NewElement.gameObject.transform.position.y - 1.0f, _PlaneScript._TimeDrop));// элемент визуально падает
        }

        MergeElement(NewElement); // слияние элемента и поля
        NewElement = null;
    }

    public void MergeElement( ElementScript newElement) {

        _matrix.BindToMatrix(newElement);

        newElement.transform.parent = this.gameObject.transform;
        _elementMarger.Add(newElement);
    }

    public void ElementDrop() {

    }
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

    public void DestroyEmptyElement() {
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
