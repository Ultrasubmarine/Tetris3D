using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum move
{
    x = 0,
    z = 1,
    _x = 2,
    _z = 3
}

public class Moving: MonoBehaviour {

    PlaneMatrix _matrix;
    [SerializeField] StateMachine _StateMachine;

    private void Start() {
        _matrix = PlaneMatrix.Instance;
    }

    public  void Action( Element element, move direction, float time) {

        if (element == null)
            return;

        if (CheckOpportunity(element, direction)) {

            if (!_StateMachine.ChangeState(EMachineState.Move, false))
                return;

            Logic( direction, element);

            Vector3 vectorDirection = SetVectorMove(direction);
            Vizual(element, vectorDirection, time);
            Messenger<Element>.Broadcast(GameEvent.MOVE_ELEMENT.ToString(), element);
        }
    }

    private bool CheckOpportunity(Element element, move direction) {

        Vector3Int vectorDirection = SetVectorMove(direction);
        return _matrix.CheckEmptyPlaсe(element, vectorDirection);  
    }

    public Vector3Int SetVectorMove(move direction) {

        Vector3Int vectorDirection;
        if (direction == move.x)
            vectorDirection = new Vector3Int(1, 0, 0);
        else if (direction == move._x)
            vectorDirection = new Vector3Int(-1, 0, 0);
        else if (direction == move.z)
            vectorDirection = new Vector3Int(0, 0, 1);
        else // (direction == move._z)
            vectorDirection = new Vector3Int(0, 0, -1);

        return vectorDirection;
    }

    private void Logic(move direction, Element element) {

        if (direction == move.x) {
            foreach (Block item in element.MyBlocks) {
                item.OffsetCoordinates( 1,0,0) ;
            }
        }
        else if (direction == move._x) {
            foreach (Block item in element.MyBlocks) {
                item.OffsetCoordinates( -1,0,0) ;
            }
        }
        else if (direction == move.z) {
            foreach (Block item in element.MyBlocks) {
                item.OffsetCoordinates( 0,0,1) ;
            }
        }
        else if (direction == move._z) {
            foreach (Block item in element.MyBlocks) {
                item.OffsetCoordinates( 0,0,-1) ;
            }
        }
    }

    private void Vizual( Element element, Vector3 direction,float time ) {

        StartCoroutine(VisualCoroutine(element, direction, time));
    }

    public IEnumerator VisualCoroutine( Element element, Vector3 direction, float time) {

        List<Vector3> finalPosBlock = new List<Vector3>();

        foreach (Block block in element.MyBlocks)
            finalPosBlock.Add(block.MyTransform.position + direction);

        Vector3 startPosition = Vector3.zero;
        Vector3 finalPosition = direction;

        Vector3 lastDeltaVector = Vector3.zero;

        float countTime = 0;
        do {
            if (countTime + Time.deltaTime < time)
                countTime += Time.deltaTime;
            else
                break;

            Vector3 deltaVector = Vector3.Lerp(startPosition, finalPosition, countTime / time);

            foreach (Block block in element.MyBlocks)
                block.MyTransform.position += deltaVector - lastDeltaVector;

            lastDeltaVector = deltaVector;
            yield return null;
        } while (countTime < time);

        for (int i = 0; i < element.MyBlocks.Count; i++) {
            element.MyBlocks[i].MyTransform.position =
                new Vector3(finalPosBlock[i].x, element.MyBlocks[i].transform.position.y, finalPosBlock[i].z);
        }

        finalPosBlock.Clear();

        _StateMachine.ChangeState(EMachineState.EndInfluence);
    }

    public bool MomentaryActionForGenerator( Element element, move direction)
    {
        if (CheckOpportunity(element, direction))
        {
            Logic(direction, element);
            Vector3 vectorDirection = SetVectorMove(direction);
            MomentaryVizual(element, vectorDirection);
            return true;
        }

        return false;
    }

    private void MomentaryVizual( Element element, Vector3 direction)
    {
        Vector3 finalPosition;
        for (int i = 0; i < element.MyBlocks.Count; i++)
        {
            finalPosition = element.MyBlocks[i].MyTransform.position + direction;
            element.MyBlocks[i].MyTransform.position =
                new Vector3(finalPosition.x, element.MyBlocks[i].transform.position.y, finalPosition.z);
        }
        
    }
 
}
