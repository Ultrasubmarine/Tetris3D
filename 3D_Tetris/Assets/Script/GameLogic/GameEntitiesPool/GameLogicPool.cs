using IntegerExtension;
using UnityEngine;

public class GameLogicPool : MonoBehaviour
{
    [SerializeField] private ElementPool _elementPool;
    [SerializeField] private BlockPool _blockPool;
    
    private void CreateBlock(Vector3Int position, Transform parent, Material material)
    {
        Block currBlock = _blockPool.CreateObject(Vector3Int.zero);
        currBlock.Mesh.material = material;
        currBlock.SetCoordinates(position.x.ToCoordinat(), position.y, position.z.ToCoordinat() );
       
        currBlock.MyTransform.parent = parent;
        SetBlockPosition(currBlock);
        element.AddBlock(currBlock);     
    }
}
