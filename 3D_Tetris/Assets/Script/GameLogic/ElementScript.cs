using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class ElementScript : MonoBehaviour {
    public List<BlockScript> MyBlocks = new List<BlockScript>(); //[] MyBlocks;

    public bool isBind = false;
    public bool isDrop = false;
    Transform _myTransform;

    void Awake() {
        _myTransform = GetComponent<Transform>();
    }

    public void AddBlock(BlockScript newBlock) {
        MyBlocks.Add(newBlock);
    }

    //public void CorrectElementChild()
    //{
    //    Vector3 min = new Vector3();
    //    Vector3 max = new Vector3();

    //    min.x = MyBlocks.Min(s => s.x);
    //    min.y = MyBlocks.Min(s => s.y);
    //    min.z = MyBlocks.Min(s => s.z);

    //    max.x = MyBlocks.Max(s => s.x);
    //    max.y = MyBlocks.Max(s => s.y);
    //    max.z = MyBlocks.Max(s => s.z);

    //}

    public void InitializationAfterGeneric(int height) {
        int maxElement = MyBlocks.Max(s => s.y);

        foreach (BlockScript item in MyBlocks)
            item.y += height - maxElement;
    }

    // ФУНКЦИИ ПАДЕНИЯ
    public void DropElement(GameObject plane) {
        //падение элемента вниз на 1 ярус. логическое
        foreach (BlockScript item in MyBlocks)
            item.y--;
    }

    public IEnumerator DropElementVisual(float finishY, float time) {
        //Speed)
        isDrop = true;
        Vector3 startPosition = _myTransform.position;
        Vector3 finalPosition = new Vector3(startPosition.x, finishY, startPosition.z);

        float timer = 0;
        do {
            timer += Time.deltaTime;
            _myTransform.position = new Vector3(gameObject.transform.position.x,
                Vector3.Lerp(startPosition, finalPosition, timer / time).y,
                transform.position.z); //Vector3.Lerp(startPosition, finalPosition, countTime / time); 
            yield return null;
        } while (timer <= time);

        _myTransform.position =
            new Vector3(_myTransform.position.x, finalPosition.y, _myTransform.position.z); //finalPosition;

        isDrop = false;
    }

    // ФУНКЦИИ ПОВОРОТА
    public void TurnElement(turn direction, GameObject target) {
        if (direction == turn.left) // правило поворота влево
        {
            foreach (BlockScript item in MyBlocks) {
                int temp = item.x;
                item.x = item.z;
                item.z = -temp;
            }
        }
        else {
            foreach (BlockScript item in MyBlocks) {
                int temp = item.x;
                item.x = -item.z;
                item.z = temp;
            }
        }
    }

    public IEnumerator TurnElementVizual(int angle, float TimeFor, GameObject target) {
        float fff = Time.time;

        float deltaAngle;
        float countAngle = 0;

        do {
            deltaAngle = angle * (Time.deltaTime / TimeFor);
            if (angle > 0 && countAngle + deltaAngle > angle || angle < 0 && countAngle + deltaAngle < angle
            ) // если мы уже достаточно повернули и в ту и в другую сторону
            {
                deltaAngle = angle - countAngle; // узнаем сколько нам не хватает на самом деле  
                countAngle = angle;
            }
            else
                countAngle += deltaAngle;

            _myTransform.Rotate(target.transform.position, deltaAngle);

            yield return null;
        } while (angle > 0 && countAngle < angle || angle < 0 && countAngle > angle);

        // Debug.Log(" Время поворота: " + (Time.time - fff));
    }

    // ФУНКЦИИ ПЕРЕМЕЩЕНИЯ
    public void MoveElement(move direction) {
        if (direction == move.x) {
            foreach (BlockScript item in MyBlocks) {
                item.x++;
            }
        }
        else if (direction == move._x) {
            foreach (BlockScript item in MyBlocks) {
                item.x--;
            }
        }
        else if (direction == move.z) {
            foreach (BlockScript item in MyBlocks) {
                item.z++;
            }
        }
        else if (direction == move._z) {
            foreach (BlockScript item in MyBlocks) {
                item.z--;
            }
        }
    }

    public IEnumerator MoveElementVisual(Vector3 direction, float timeFor) {
        List<Vector3> finalPosBlock = new List<Vector3>();

        foreach (BlockScript block in MyBlocks)
            finalPosBlock.Add(block.transform.position + direction);

        Vector3 startPosition = Vector3.zero;
        Vector3 finalPosition = direction;

        Vector3 lastDeltaVector = Vector3.zero;

        float countTime = 0;
        do {
            if (countTime + Time.deltaTime < timeFor)
                countTime += Time.deltaTime;
            else
                break;

            // tyt bilo eshe  = gameObject.transform.position = 
            Vector3 deltaVector = Vector3.Lerp(startPosition, finalPosition, countTime / timeFor);

            //foreach (var item in MyBlocks)
            foreach (BlockScript block in MyBlocks)
                block.transform.position += deltaVector - lastDeltaVector;


            lastDeltaVector = deltaVector;
            yield return null;
        } while (countTime < timeFor);

        for (int i = 0; i < MyBlocks.Count; i++) {
            MyBlocks[i].transform.position =
                new Vector3(finalPosBlock[i].x, MyBlocks[i].transform.position.y, finalPosBlock[i].z);
        }

        finalPosBlock.Clear();
    }

    public bool CheckMove(move direction, int MinCoordinat) {
        if (direction == move.x) {
            foreach (BlockScript item in MyBlocks) {
                if (item.x == Mathf.Abs(MinCoordinat))
                    return false;
            }

            return true;
        }

        if (direction == move._x) {
            foreach (BlockScript item in MyBlocks) {
                if (item.x == MinCoordinat)
                    return false;
            }

            return true;
        }

        if (direction == move.z) {
            foreach (BlockScript item in MyBlocks) {
                if (item.z == Mathf.Abs(MinCoordinat))
                    return false;
            }

            return true;
        }

        if (direction == move._z) {
            foreach (BlockScript item in MyBlocks) {
                if (item.z == MinCoordinat)
                    return false;
            }

            return true;
        }

        return true;
    }

    public bool CheckEmptyElement() {
        for (int i = 0; i < MyBlocks.Count; i++) {
            if (MyBlocks[i] != null)
                return false; // не пуст
        }

        Debug.Log("I,m ALONE");
        return true; // пуст
    }

    public void DeleteBlock(BlockScript block) {
        if (MyBlocks.Contains(block)) {
            MyBlocks.Remove(block);

            //TODO Возвращать блоки в пул?
            Destroy(block.gameObject);
        }
    }

    public ElementScript CheckUnion() {
        if (MyBlocks.Count == 0)
            return null;
        List<BlockScript> m1 = new List<BlockScript>();
        // MMM

        BlockScript curr = MyBlocks[0]; // точка отсчета

        m1.Add(curr);
        foreach (var item in MyBlocks) {
            if (Sliti_Li(curr, item))
                m1.Add(item);
        }

        if (m1.Count == MyBlocks.Count)
            return null; // все норм. мы объединили
        else {
            int k = 0;
            int countK = m1.Count;
            while (k < countK) {
                var Ost = MyBlocks.Except(m1).ToList();
                for (int i = 0; i < Ost.Count; i++) {
                    if (Sliti_Li(Ost[i], m1[k])) {
                        m1.Add(Ost[i]);
                        countK++;
                    }
                }

                k++;
            }


            // создаем новый элемент
            if (m1.Count < MyBlocks.Count) {
                this.name = "ZLO CUT CUT CUT";
                Debug.Log("Create ++++ ELEMENT");
                GameObject newEl = new GameObject("ZLO DOUBLE");
                newEl.transform.position = Vector3.zero;

                var Ost = MyBlocks.Except(m1).ToList();
                newEl.AddComponent<ElementScript>().MyBlocks = Ost;

                for (int i = 0; i < Ost.Count; i++) {
                    Ost[i].gameObject.transform.parent = newEl.transform;
                    MyBlocks.Remove(Ost[i]);
                }

                return newEl.GetComponent<ElementScript>();
            }
            else
                return null;
        }
    }

    public bool Sliti_Li(BlockScript b1, BlockScript b2) {
        Vector3 b1p = new Vector3(b1.x, b1.y, b1.z);
        Vector3 b2p = new Vector3(b2.x, b2.y, b2.z);


        if (b1p.x == b2p.x && b1p.y == b2p.y && b1p.z == b2p.z + 1)
            return true;
        if (b1p.x == b2p.x && b1p.y == b2p.y && b1p.z == b2p.z - 1)
            return true;
        if (b1p.x == b2p.x + 1 && b1p.y == b2p.y && b1p.z == b2p.z)
            return true;
        if (b1p.x == b2p.x - 1 && b1p.y == b2p.y && b1p.z == b2p.z)
            return true;
        if (b1p.x == b2p.x && b1p.y == b2p.y + 1 && b1p.z == b2p.z)
            return true;
        if (b1p.x == b2p.x && b1p.y == b2p.y - 1 && b1p.z == b2p.z)
            return true;

        return false;
    }

    public void SetTurn(turn direction, GameObject target) {
        TurnElement(direction, target);

        int rotate;
        if (direction == turn.left)
            rotate = 90;
        else
            rotate = -90;

        StartCoroutine(TurnElementVizual(rotate, 0, target));
    }

    public void SetMove(move direction) {
        Vector3 vectorDirection;

        if (direction == move.x)
            vectorDirection = new Vector3(1.0f, 0f, 0f);
        else if (direction == move._x)
            vectorDirection = new Vector3(-1.0f, 0f, 0f);
        else if (direction == move.z)
            vectorDirection = new Vector3(0f, 0f, 1.0f);
        else // (direction == move._z)
            vectorDirection = new Vector3(0f, 0f, -1.0f);

        StartCoroutine(MoveElementVisual(vectorDirection, 0));
    }
}