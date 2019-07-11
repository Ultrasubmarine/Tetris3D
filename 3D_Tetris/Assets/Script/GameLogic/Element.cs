using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class Element : MonoBehaviour {
    public List<Block> MyBlocks = new List<Block>();

    public bool IsBind = false;
    public bool IsDrop = false;
    public Transform MyTransform { get; private set; }
    
    void Awake() {
        MyTransform = this.transform;
    }

    public void AddBlock(Block newBlock) {
        MyBlocks.Add(newBlock);
    }

    public void InitializationAfterGeneric(int height) {
        int maxElement = MyBlocks.Max(s => s.y);

        foreach (Block item in MyBlocks)
            item.y += height - maxElement;
    }

    #region ФУНКЦИИ ПАДЕНИЯ
    public void LogicDrop() {
        foreach (Block item in MyBlocks)
            item.y--;
    }

    public IEnumerator VisualDrop( float time) {
    
        IsDrop = true;
        yield return StartCoroutine(VizualRelocation( Vector3.down, time));
        IsDrop = false;
    }
    #endregion

    private IEnumerator VizualRelocation( Vector3 offset, float time) {

        Vector3 startPosition = MyTransform.position;
        Vector3 finalPosition = MyTransform.position + offset;

        float timer = 0;
        do {
            timer += Time.deltaTime;
            MyTransform.position = Vector3.Lerp(startPosition, finalPosition, timer / time);
            yield return null;
        } while (timer <= time);

        MyTransform.position = finalPosition;
    }

    #region ФУНКЦИИ ПОВОРОТА
    public void LogicTurn(turn direction) {
        if (direction == turn.left) // правило поворота влево
        {
            foreach (Block item in MyBlocks) {
                int temp = item.x;
                item.x = item.z;
                item.z = -temp;
            }
        }
        else {
            foreach (Block item in MyBlocks) {
                int temp = item.x;
                item.x = -item.z;
                item.z = temp;
            }
        }
    }

    public IEnumerator VizualTurn(int angle, float time, GameObject target) {
  
        float deltaAngle;
        float countAngle = 0;

        do {
            deltaAngle = angle * (Time.deltaTime / time);
            if (angle > 0 && countAngle + deltaAngle > angle || angle < 0 && countAngle + deltaAngle < angle) // если мы уже достаточно повернули и в ту и в другую сторону
            {
                deltaAngle = angle - countAngle; // узнаем сколько нам не хватает на самом деле  
                countAngle = angle;
            }
            else
                countAngle += deltaAngle;

            MyTransform.Rotate(target.transform.position, deltaAngle);

            yield return null;
        } while (angle > 0 && countAngle < angle || angle < 0 && countAngle > angle);
    }
    #endregion

    public void OffsetCoordinatBlocks(int of_x = 0, int of_y = 0, int of_z = 0) {

        foreach (var item in MyBlocks) {
            item.OffsetCoordinat(of_x, of_y, of_z);
        }
    }

    private void OnDisable()
    {
        MyTransform.rotation = Quaternion.identity;
        MyTransform.position = Vector3.zero;
    }

    public bool CheckEmpty() {
        if( MyBlocks.Count > 0)
                return false; 
        return true;
    }

    public void DeleteBlocksInList(Block[] massBlock)
    {
        MyBlocks = MyBlocks.Except( massBlock).ToList();
    }
    
    #region РАЗБИЕНИЕ ЭЛ_ТА НА 2
    public List<Block> CheckUnion() {
        if (MyBlocks.Count == 0)
            return null;
        List<Block> contactList = new List<Block>();

        Block curr = MyBlocks[0]; 

        contactList.Add(curr);
        foreach (var item in MyBlocks) {
            if (CheckContact(curr, item))
                contactList.Add(item);
        }

        if (contactList.Count == MyBlocks.Count)
            return null; 
        else {
            int k = 0;
            int countK = contactList.Count;
            while (k < countK) {
                var remainingBlocks = MyBlocks.Except(contactList).ToList();
                for (int i = 0; i < remainingBlocks.Count; i++) {
                    if (CheckContact(remainingBlocks[i], contactList[k])) {
                        contactList.Add(remainingBlocks[i]);
                        countK++;
                    }
                }
                k++;
            }
            if (contactList.Count < MyBlocks.Count) {
                var notContact = MyBlocks.Except(contactList).ToList();
                MyBlocks = contactList;
                return notContact;
            }
            else
                return null;
        }
    }

    public bool CheckContact(Block b1, Block b2) {
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
    #endregion

    #region для Generator.cs
    public void SetTurn(turn direction, GameObject target) {
        LogicTurn(direction);

        int rotate;
        if (direction == turn.left)
            rotate = 90;
        else
            rotate = -90;

        StartCoroutine(VizualTurn(rotate, 0, target));
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

        //StartCoroutine(VisualMove(vectorDirection, 0));
    }

    public bool CheckMove(move direction, int MinCoordinat) {
        if (direction == move.x) {
            foreach (Block item in MyBlocks) {
                if (item.x == Mathf.Abs(MinCoordinat))
                    return false;
            }
            return true;
        }
        if (direction == move._x) {
            foreach (Block item in MyBlocks) {
                if (item.x == MinCoordinat)
                    return false;
            }
            return true;
        }
        if (direction == move.z) {
            foreach (Block item in MyBlocks) {
                if (item.z == Mathf.Abs(MinCoordinat))
                    return false;
            }
            return true;
        }
        if (direction == move._z) {
            foreach (Block item in MyBlocks) {
                if (item.z == MinCoordinat)
                    return false;
            }
            return true;
        }
        return true;
    }
    #endregion
}