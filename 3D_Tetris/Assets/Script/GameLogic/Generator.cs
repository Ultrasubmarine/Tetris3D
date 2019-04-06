using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class Element {
    public GameObject El;
    public double P;
}

public class Generator : MonoBehaviour {
    public GameObject[] PrefabElement;
    public double[] PElement;
    public GameObject PrefabBlock;

    [SerializeField] int heightGeneration;
    [SerializeField] Material[] MyMaterial;


    [SerializeField] Material _BonusMaterial;
    int minYforElement;
    public GameObject examleElement;


    public GameObject GenerationNewElement(PlaneScript plane) {
        //    Destroy(exElement);
        GameObject NewElement = CreatorElement(plane._block); // Instantiate(generationElement());


        //устанавливаем нормальную позицию элемента
        Vector3 temp = plane.gameObject.transform.position;

        // инициализируем блоки элемента согласно установленной позиции
        ElementScript Element = NewElement.GetComponent<ElementScript>();
        Element.InitializationAfterGeneric(plane.Height);


        //// выравниваем элемент относительно координат y 
        var min_y = Element.MyBlocks.Min(s => s.y);
        var max_y = Element.MyBlocks.Max(s => s.y);

        int size = max_y - min_y;

        NewElement.transform.position = new Vector3(temp.x, temp.y + plane.Height - size, temp.z);


        // TO DO - вычленить в отдельный метод создание дубляжа
        GameObject exElement = Instantiate(NewElement);
        exElement.name = " TUTOR";
        foreach (var item in exElement.GetComponent<ElementScript>().MyBlocks) {
            item.GetComponent<Renderer>().material = _BonusMaterial;
            item.gameObject.transform.localScale = new Vector3(0.95f, 0.95f, 0.95f);
        }

        exElement.transform.position =
            new Vector3(temp.x, plane.gameObject.transform.position.y + minYforElement, temp.z);
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

    private GameObject CreatorElement(BlockScript[,,] matrix) {
        int indexmat = Random.Range(0, MyMaterial.Length - 1);
        // check min matrix element
        Vector3 min = CheckMatrixMinimum(matrix);
        minYforElement = (int) min.y;
        bool[,,] matrixCheck = CastMatrix((int) min.y, matrix);

        // create element
        GameObject elementObj = new GameObject("MY ZLO");
        ElementScript createElement = elementObj.AddComponent<ElementScript>();

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


    private void CreatorBlock(Vector3 position, ElementScript element, int indexmat) {
        // добавляем компонент с координатами блока
        GameObject currBlock = Instantiate(PrefabBlock);
        currBlock.AddComponent<BlockScript>().SetCoordinat(position);
        currBlock.GetComponent<MeshRenderer>().material = MyMaterial[indexmat];

        currBlock.gameObject.transform.parent = element.gameObject.transform;
        currBlock.transform.localPosition = position;
        SettingsPositionBlock(currBlock.GetComponent<BlockScript>());
        element.AddBlock(currBlock.GetComponent<BlockScript>());
    }

    private void SettingsPositionBlock(BlockScript block) {
        Vector3 position = new Vector3(block.x, block.y, block.z);
        block.gameObject.transform.localPosition = position;
    }

    // chek matrix min
    private Vector3 CheckMatrixMinimum(BlockScript[,,] matrix) {
        int min = matrix.GetUpperBound(1) - 1;
        int curr_min;
        Vector3 min_point = new Vector3(matrix.GetUpperBound(0), matrix.GetUpperBound(1) - 1, matrix.GetUpperBound(2));

        // но спускаться нужно сверху вниз! 
        for (int x = 0; x < matrix.GetUpperBound(0) + 1; ++x) {
            curr_min = matrix.GetUpperBound(1) - 1;
            for (int z = 0; z < matrix.GetUpperBound(2) + 1; ++z) {
                for (int y = matrix.GetUpperBound(1) - 1; y >= 0; --y) {
                    if (matrix[x, y, z] == null)
                        curr_min = y;
                    else
                        break;
                }

                if (min > curr_min) {
                    min = curr_min;
                    min_point = new Vector3(x, min, z);
                }
            }
        }

        return min_point;
    }

    // слепок матрицы
    private bool[,,] CastMatrix(int min, BlockScript[,,] matrix) {
        bool[,,] castMatrix = new bool[3, 3, 3];
        bool blockLayer;

        //  Debug.Log("min -"+ min);
        // делаем слепок
        for (int x = 0; x < 3; x++) {
            for (int z = 0; z < 3; z++) {
                blockLayer = false;
                //for (int y = 2; y >= 0; y--)
                //{
                //    if( min + y < matrix.GetUpperBound(1))
                //    {
                //        if (matrix[x, min + y, z] == null && !blockLayer)
                //        {
                //            castMatrix[x, y, z] = true;
                //        }
                //        else
                //        {
                //            blockLayer = true;
                //            castMatrix[x, y, z] = false;
                //        }

                //    }
                //    else
                //    {
                //        castMatrix[x, y, z] = true;
                //    }

                //}
                for (int y = matrix.GetUpperBound(1) - 1; y >= min; y--) {
                    //if (min + y < matrix.GetUpperBound(1))
                    {
                        if (matrix[x, y, z] == null && !blockLayer) {
                            if (y <= min + 2 && y >= min)
                                castMatrix[x, y - min, z] = true;
                        }
                        else {
                            blockLayer = true;
                            if (y <= min + 2 && y >= min) {
                                //   Debug.Log("block layer x " + x.ToString() + " y " + y.ToString() + " z " + z.ToString());

                                castMatrix[x, y - min, z] = false;
                            }
                        }
                    }
                    //else
                    //{
                    //    castMatrix[x, y, z] = true;
                    //}
                }
            }
        }

        return castMatrix;
    }

    private List<Vector3> PovFreeCount(Vector3 point, bool[,,] matrix, ElementScript currEl) {
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


    void ChengeBlock(ElementScript element, GameObject target) {
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