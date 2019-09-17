using System;
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
    
    private void OnDisable()
    {
        MyTransform.rotation = Quaternion.identity;
        MyTransform.position = Vector3.zero;
        gameObject.name = "element";
    }

    public void AddBlock(Block newBlock) {
        MyBlocks.Add(newBlock);
    }

    public void InitializationAfterGeneric(int height) {
        int maxElement = MyBlocks.Max(s => s.Coordinates.y);

        foreach (Block item in MyBlocks)
            item.OffsetCoordinates(0,  height - maxElement, 0); //item.Coordinates.y += height - maxElement;
    }

    #region ФУНКЦИИ ПАДЕНИЯ

    public void DropInOneLayer() {
        LogicDrop();
        VisualDrop(Speed.TimeDrop);
    }
    
    public void LogicDrop() {
        foreach (Block item in MyBlocks)
            item.OffsetCoordinates(0,  -1, 0);//item.Coordinates.y--;
    }

    public void VisualDrop(float time) {
        IsDrop = true;
        StartCoroutine(VizualRelocation(Vector3.down, time, () => { IsDrop = false; Messenger.Broadcast("EndVizual");}) );
}
    #endregion

    private IEnumerator VizualRelocation( Vector3 offset, float time, Action callBack) {

        Vector3 startPosition = MyTransform.position;
        Vector3 finalPosition = startPosition + offset;

        float timer = 0;
        do {
            timer += Time.deltaTime;
            MyTransform.position = Vector3.Lerp(startPosition, finalPosition, timer / time);
            yield return null;
        } while (timer <= time);

        MyTransform.position = finalPosition;

        if (!Equals(callBack)) {
            callBack.Invoke();
        }
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
        Vector3 b1p = new Vector3(b1.Coordinates.x, b1.Coordinates.y, b1.Coordinates.z);
        Vector3 b2p = new Vector3(b2.Coordinates.x, b2.Coordinates.y, b2.Coordinates.z);

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

}