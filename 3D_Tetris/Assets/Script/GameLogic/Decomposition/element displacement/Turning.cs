using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turning : ControlBase<turn> {

    protected override bool CheckOpportunity(turn parametr) {
   
        //int x, z;
        //if (direction == turn.left) {
        //    foreach (var item in NewElement.MyBlocks) {
        //        // по правилу поворота
        //        x = item.z;
        //        z = -item.x;

        //        if (_matrix._matrix[x + 1, item.y, z + 1] != null)
        //            return false;
        //    }
        //}
        //else {
        //    foreach (var item in NewElement.MyBlocks) {
        //        // по правилу поворота
        //        x = -item.z;
        //        z = item.x;

        //        if (_matrix._matrix[x + 1, item.y, z + 1] != null)
        //            return false;
        //    }
        //}

        return true;
        
    }

    protected override void Logic(turn parametr) {
        throw new System.NotImplementedException();
    }

    protected override void Vizual(turn parametr) {
        throw new System.NotImplementedException();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


}
