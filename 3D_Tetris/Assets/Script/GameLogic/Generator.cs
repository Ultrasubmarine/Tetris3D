using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


//public class Element {
//    public GameObject El;
//    public double P;
//}

public class Generator : MonoBehaviour {
    public GameObject[] PrefabElement;
    public double[] PElement;
    public GameObject PrefabBlock;

    [SerializeField] int heightGeneration;
    [SerializeField] Material[] MyMaterial;


    [SerializeField] Material _BonusMaterial;
    int minYforElement;
    public GameObject examleElement;

    [SerializeField] PlaneMatrix _Matrix;


    public GameObject GenerationNewElement( Transform elementParent){
        GameObject NewElement = CreatorElement(_Matrix._matrix); // Instantiate(generationElement());

        //устанавливаем нормальную позицию элемента
        Vector3 temp = elementParent.position;

        // инициализируем блоки элемента согласно установленной позиции
        Element Element = NewElement.GetComponent<Element>();
        Element.InitializationAfterGeneric(_Matrix.Height);//plane.Height);

        //// выравниваем элемент относительно координат y 
        var min_y = Element.MyBlocks.Min(s => s.y);
        var max_y = Element.MyBlocks.Max(s => s.y);

        int size = max_y - min_y;

        NewElement.transform.position = new Vector3(temp.x, temp.y + _Matrix.Height - size, temp.z);

        // TO DO - вычленить в отдельный метод создание дубляжа
        GameObject exElement = Instantiate(NewElement);
        exElement.name = " TUTOR";
        foreach (var item in exElement.GetComponent<Element>().MyBlocks) {
            item.GetComponent<Renderer>().material = _BonusMaterial;
            item.gameObject.transform.localScale = new Vector3(0.95f, 0.95f, 0.95f);
        }

        exElement.transform.position =
            new Vector3(temp.x, elementParent.gameObject.transform.position.y + minYforElement, temp.z);
        examleElement = exElement; // Destroy(exElement, 5f);
        ////////////

      //  ChengeBlock(Element, plane.gameObject);
        return NewElement;
    }

    private GameObject generationElement() {
        if (PElement.Length == 0) // равномерный закон распределения ( по умолчанию ) 
        {
            return PrefabElement[Random.Range(0, PrefabElement.Length)];
        }
        else // заданный закон распределения
        {
            double valueLine = 0;
            double rand = Random.value;

            for (int i = 0; i < PrefabElement.Length; i++) // просматриваем все вероятности элементов
            {
                valueLine += PElement[i];
                if (rand < valueLine)
                    return PrefabElement[i];
            }

            return PrefabElement[PrefabElement.Length - 1];
        }
    }

    private GameObject CreatorElement(Block[,,] matrix) {

        int indexmat = Random.Range(0, MyMaterial.Length - 1);
        // check min matrix element
        Vector3 min = _Matrix.FindLowerAccessiblePlace();
        minYforElement = (int) min.y;
        bool[,,] matrixCheck = CastMatrix((int) min.y);

        // create element
        GameObject elementObj = new GameObject("MY ZLO");
        Element createElement = elementObj.AddComponent<Element>();

        Vector3 lastPoint = new Vector3(min.x, 0, min.z);

        // выращиваем элемент - 1 блок
        CreatorBlock(lastPoint, createElement, indexmat);
        matrixCheck[(int) min.x, (int) 0, (int) min.z] = false;

        List<Vector3> pov; // степень свободы
        int index;
        for (int i = 0; i < 3; i++) {
            pov = PovFreeCount(lastPoint, matrixCheck, createElement);
            if (pov.Count > 0) {
                index = (int) Random.Range(0, pov.Count);

                // добавляем компонент с координатами блока
                CreatorBlock(pov[index], createElement, indexmat);
                // обновляем слепок
                matrixCheck[(int) pov[index].x, (int) pov[index].y, (int) pov[index].z] = false;
                lastPoint = pov[index];
            }
        }

        return createElement.gameObject;
    }

    private void CreatorBlock(Vector3 position, Element element, int indexmat) {
        // добавляем компонент с координатами блока
        GameObject currBlock = Instantiate(PrefabBlock);
        currBlock.AddComponent<Block>().SetCoordinat(position);
        currBlock.GetComponent<MeshRenderer>().material = MyMaterial[indexmat];

        currBlock.gameObject.transform.parent = element.gameObject.transform;
        currBlock.transform.localPosition = position;
        SettingsPositionBlock(currBlock.GetComponent<Block>());
        element.AddBlock(currBlock.GetComponent<Block>());
    }

    private void SettingsPositionBlock(Block block) {
        Vector3 position = new Vector3(block.x, block.y, block.z);
        block.gameObject.transform.localPosition = position;
    }

    // слепок матрицы
    private bool[,,] CastMatrix(int min) {

        bool[,,] castMatrix = new bool[3, 3, 3];
        bool blockLayer;

        // делаем слепок
        for (int x = 0; x < 3; x++) {
            for (int z = 0; z < 3; z++) {
                blockLayer = false;
                for (int y = min + 3 - 1; y >= min; y--) {                
                    if(!blockLayer)
                        blockLayer = !_Matrix.CheckEmptyPlace(x, y, z);
                    castMatrix[x, y - min, z] = !blockLayer;               
                }
            }
        }
        return castMatrix;
    }

    private List<Vector3> PovFreeCount(Vector3 point, bool[,,] matrix, Element currEl) {
        List<Vector3> ListPov = new List<Vector3>();

        if (point.x < 2)
            if (matrix[(int) point.x + 1, (int) point.y, (int) point.z] &&
                (point.y == 0 || !matrix[(int) point.x + 1, (int) point.y - 1, (int) point.z])) {
                ListPov.Add(new Vector3(point.x + 1, (int) point.y, (int) point.z));
            }

        if (point.x > 0)
            if (matrix[(int) point.x - 1, (int) point.y, (int) point.z] &&
                (point.y == 0 || !matrix[(int) point.x - 1, (int) point.y - 1, (int) point.z])) {
                ListPov.Add(new Vector3(point.x - 1, (int) point.y, (int) point.z));
            }

        if (point.z < 2)
            if (matrix[(int) point.x, (int) point.y, (int) point.z + 1] &&
                (point.y == 0 || !matrix[(int) point.x, (int) point.y - 1, (int) point.z + 1])) {
                ListPov.Add(new Vector3(point.x, (int) point.y, (int) point.z + 1));
            }

        if (point.z > 0)
            if (matrix[(int) point.x, (int) point.y, (int) point.z - 1] &&
                (point.y == 0 || matrix[(int) point.x, (int) point.y - 1, (int) point.z - 1])) {
                ListPov.Add(new Vector3(point.x, (int) point.y, (int) point.z - 1));
            }

        if (point.y < 2)
            if (matrix[(int) point.x, (int) point.y + 1, (int) point.z]) {
                ListPov.Add(new Vector3(point.x, (int) point.y + 1, (int) point.z));
            }

        return ListPov;
    }


    void ChengeBlock(Element element, GameObject target) {
        Random rn = new Random();

        int turnCount = Random.Range(1, 2);
        if (turnCount > 0) {
            turn direction = (turn) Random.Range(0, 1 + 1);
            Debug.Log(direction.ToString());
            while (turnCount > 0) {
                element.SetTurn(direction, target);
                turnCount--;
            }
        }

        int moveCount = Random.Range(0, 2);
        if (moveCount > 0) {
            move directionMove = (move) Random.Range(0, 4 + 1);
            while (turnCount > 0) {
                if (element.CheckMove(directionMove, 3 / 2 * (-1))) {
                    element.SetMove(directionMove);
                    moveCount--;
                }
                else
                    break;
            }
        }
    }
}