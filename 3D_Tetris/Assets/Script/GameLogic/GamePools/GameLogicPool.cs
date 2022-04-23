using Helper.Patterns;
using IntegerExtension;
using Script.GameLogic.GameItems;
using UnityEngine;

public class GameLogicPool : MonoBehaviour
{
    [SerializeField] private GameObject _elementPrefab;
    [SerializeField] private GameObject _blockPrefab;
    [SerializeField] private GameObject _pickableBlockPrefab;

    [SerializeField] private Transform _elementPoolParent;
    [SerializeField] private Transform _blockPoolParent;
    [SerializeField] private Transform _pickableBlockPoolParent;
    
    private Pool<Element> _elementPool;
    private Pool<Block>   _blockPool;
    private Pool<PickableBlock> _pickableBlockPool;

    private Pool<Element> _activeElements;
    
    public Element CreateEmptyElement()
    {
        return _elementPool.Pop(true);
    }

    public void CreateBlock(Vector3Int position, Element element, Material material)
    {
        var currBlock = _blockPool.Pop(true);
        currBlock.mesh.material = material;
        currBlock.SetCoordinates(position.x.ToCoordinat(), position.y, position.z.ToCoordinat());

        currBlock.myTransform.parent = element.myTransform;
        SetBlockPosition(currBlock);
        element.AddBlock(currBlock);
    }

    public PickableBlock CreatePickableBlock(Vector3Int position)
    {
        var currBlock = _pickableBlockPool.Pop(true);
        currBlock.SetCoordinates(position.x.ToCoordinat(), position.y, position.z.ToCoordinat());

        SetBlockPosition(currBlock);
        return currBlock;
    }
    
    private void SetBlockPosition(Block block)
    {
        var position = new Vector3(block.coordinates.x, block.coordinates.y, block.coordinates.z);
        block.gameObject.transform.localPosition = position;
    }

    public void DeleteElement(Element element)
    {
        if (element.blocks.Count > 0)
            foreach (var block in element.blocks)
            {
                DeleteBlock(block);
            }

        element.blocks.Clear();
        element.gameObject.SetActive(false);
        _elementPool.Push(element);
    }

    public void DeleteBlock(Block block)
    {
        if (Equals(block, null))
            return;
        block.gameObject.SetActive(false);
        _blockPool.Push(block);
    }
    
    public void DeletePickableBlock(PickableBlock pBlock)
    {
        pBlock.gameObject.SetActive(false);
        _pickableBlockPool.Push(pBlock);
    }
    
    
    private void Awake()
    {
        _elementPool = new Pool<Element>(_elementPrefab.GetComponent<Element>(), _elementPoolParent);
        _blockPool = new Pool<Block>(_blockPrefab.GetComponent<Block>(), _blockPoolParent);
        _pickableBlockPool = new Pool<PickableBlock>(_pickableBlockPrefab.GetComponent<PickableBlock>(), _pickableBlockPoolParent);
    }
}