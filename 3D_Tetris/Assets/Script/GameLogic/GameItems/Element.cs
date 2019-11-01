using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Element : MonoBehaviour
{
    public List<Block> MyBlocks = new List<Block>();

    public bool IsBind = false;
    public Transform MyTransform { get; private set; }

    private void Awake()
    {
        MyTransform = transform;
    }

    private void OnDisable()
    {
        MyTransform.rotation = Quaternion.identity;
        MyTransform.position = Vector3.zero;
    }

    public void AddBlock(Block newBlock)
    {
        MyBlocks.Add(newBlock);
    }

    public void InitializationAfterGeneric(int height)
    {
        var maxElement = MyBlocks.Max(s => s.Coordinates.y);

        foreach (var item in MyBlocks)
            item.OffsetCoordinates(0, height - maxElement, 0); //item.Coordinates.y += height - maxElement;
    }

    public void LogicDrop()
    {
        foreach (var item in MyBlocks)
            item.OffsetCoordinates(0, -1, 0);
    }

    public bool CheckEmpty()
    {
        if (MyBlocks.Count > 0)
            return false;
        return true;
    }

    public void RemoveBlocksInList(Block[] massBlock)
    {
        MyBlocks = MyBlocks.Except(massBlock).ToList();
    }

    #region РАЗБИЕНИЕ ЭЛ_ТА НА 2

    public List<Block> GetNotAttachedBlocks()
    {
        if (MyBlocks.Count == 0)
            return null;
        var contactList = new List<Block>();

        var curr = MyBlocks[0];

        contactList.Add(curr);
        foreach (var item in MyBlocks)
            if (CheckContact(curr, item))
                contactList.Add(item);

        if (contactList.Count == MyBlocks.Count)
        {
            return null;
        }
        else
        {
            var k = 0;
            var countK = contactList.Count;
            while (k < countK)
            {
                var remainingBlocks = MyBlocks.Except(contactList).ToList();
                for (var i = 0; i < remainingBlocks.Count; i++)
                    if (CheckContact(remainingBlocks[i], contactList[k]))
                    {
                        contactList.Add(remainingBlocks[i]);
                        countK++;
                    }

                k++;
            }

            if (contactList.Count < MyBlocks.Count)
            {
                var notContact = MyBlocks.Except(contactList).ToList();
                MyBlocks = contactList;
                return notContact;
            }
            else
            {
                return null;
            }
        }
    }

    private bool CheckContact(Block b1, Block b2)
    {
        var b1p = new Vector3Int(b1.Coordinates.x, b1.Coordinates.y, b1.Coordinates.z);
        var b2p = new Vector3Int(b2.Coordinates.x, b2.Coordinates.y, b2.Coordinates.z);

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