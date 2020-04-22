using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Serialization;

public class Element : MonoBehaviour
{
    public List<Block> blocks => _blocks;
    
    public bool _isBind = false;
    
    public Transform myTransform { get; private set; }
    
    [SerializeField] private List<Block> _blocks = new List<Block>();
    

    private void Awake()
    {
        myTransform = transform;
    }

    private void OnDisable()
    {
        myTransform.localRotation = Quaternion.identity;
        myTransform.localPosition = Vector3.zero;
    }

    public void AddBlock(Block newBlock)
    {
        _blocks.Add(newBlock);
    }

    public void SetBlocks(List<Block> blocks)
    {
        this._blocks = blocks;
    }
    
    public void InitializationAfterGeneric(int height)
    {
        var maxElement = _blocks.Max(s => s.coordinates.y);

        foreach (var item in _blocks)
            item.OffsetCoordinates(0, height - maxElement, 0); //item.Coordinates.y += height - maxElement;
    }

    public void LogicDrop()
    {
        foreach (var item in _blocks)
            item.OffsetCoordinates(0, -1, 0);
    }

    public bool CheckEmpty()
    {
        if (_blocks.Count > 0)
            return false;
        return true;
    }

    public void RemoveBlocksInList(Block[] massBlock)
    {
        _blocks = _blocks.Except(massBlock).ToList();
    }

    #region РАЗБИЕНИЕ ЭЛ_ТА НА 2

    public List<Block> GetNotAttachedBlocks()
    {
        if (_blocks.Count == 0)
            return null;
        var contactList = new List<Block>();

        var curr = _blocks[0];

        contactList.Add(curr);
        foreach (var item in _blocks)
            if (CheckContact(curr, item))
                contactList.Add(item);

        if (contactList.Count == _blocks.Count)
        {
            return null;
        }
        else
        {
            var k = 0;
            var countK = contactList.Count;
            while (k < countK)
            {
                var remainingBlocks = _blocks.Except(contactList).ToList();
                for (var i = 0; i < remainingBlocks.Count; i++)
                    if (CheckContact(remainingBlocks[i], contactList[k]))
                    {
                        contactList.Add(remainingBlocks[i]);
                        countK++;
                    }

                k++;
            }

            if (contactList.Count < _blocks.Count)
            {
                var notContact = _blocks.Except(contactList).ToList();
                _blocks = contactList;
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
        var b1p = new Vector3Int(b1.coordinates.x, b1.coordinates.y, b1.coordinates.z);
        var b2p = new Vector3Int(b2.coordinates.x, b2.coordinates.y, b2.coordinates.z);

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