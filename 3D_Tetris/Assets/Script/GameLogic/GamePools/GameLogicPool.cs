using IntegerExtension;
using UnityEngine;

public class GameLogicPool : MonoBehaviour
{
    [SerializeField] private ElementPool _elementPool;
    [SerializeField] private BlockPool _blockPool;

    public Element CreateEmptyElement()
    {
        return _elementPool.CreateObject(Vector3.zero);
    }

    public void CreateBlock(Vector3Int position, Element element, Material material)
    {
        var currBlock = _blockPool.CreateObject(Vector3Int.zero);
        currBlock.Mesh.material = material;
        currBlock.SetCoordinates(position.x.ToCoordinat(), position.y, position.z.ToCoordinat());

        currBlock.MyTransform.parent = element.MyTransform;
        SetBlockPosition(currBlock);
        element.AddBlock(currBlock);
    }

    private void SetBlockPosition(Block block)
    {
        var position = new Vector3(block.Coordinates.x, block.Coordinates.y, block.Coordinates.z);
        block.gameObject.transform.localPosition = position;
    }

    public void DeleteElement(Element element)
    {
        if (element.MyBlocks.Count > 0)
            foreach (var block in element.MyBlocks)
            {
                DeleteBlock(block);
            }

        _elementPool.DestroyObject(element);
    }

    public void DeleteBlock(Block block)
    {
        _blockPool.DestroyObject(block);
    }
}