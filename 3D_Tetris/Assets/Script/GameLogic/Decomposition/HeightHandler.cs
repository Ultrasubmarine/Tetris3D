using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using IntegerExtension;

public class HeightHandler : MonoBehaviour {

    [SerializeField] PlaneMatrix Matrix;

    [SerializeField, Space(20)] int _LimitHeight;
    [SerializeField] int _CurrentHeight;

    public int LimitHeight{ get { return _LimitHeight; } }
    public int CurrentHeight { get { return _CurrentHeight; } }

	// Use this for initialization
	void Start () {
		
	}

    public void CheckHeight() {

        _CurrentHeight = 0;
        int check;

        for (int x = 0; x < Matrix.Wight; x++) {
            for (int z = 0; z < Matrix.Wight; z++) {
               check = Matrix.MinHeightInCoordinates(x, z);
               _CurrentHeight = _CurrentHeight > check? _CurrentHeight : check;
                //if( OutOfLimitHeight() )

            }
        }
        Messenger<int, int>.Broadcast(GameEvent.CURRENT_HEIGHT, _LimitHeight, _CurrentHeight +1);
    }

    private bool OutOfLimitHeight( ) {

        if (_CurrentHeight < LimitHeight)
            return false;
        return true;
    }
}
