﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IntegerExtension {
    public static class ExtensionMetodsForMatrix {

        static int _absMinCoordinat;
        static int _difference; // index - coordinate
        static int wight;
        public static void SetSizePlane( int wight) {
            _absMinCoordinat = wight / 2;
            _difference = 0 - (-_absMinCoordinat);
            ExtensionMetodsForMatrix.wight = wight;
        }
        public static int ToIndex( this int coordinat) {
            return coordinat + _difference;
        }

        public static int ToCoordinat(this int index) {
            return index - _difference;
        }

        public static bool OutOfCoordinatLimit( this Vector3Int coordinat) {

            if (Mathf.Abs(coordinat.x) > _absMinCoordinat || coordinat.y < 0 || Mathf.Abs(coordinat.z) > _absMinCoordinat)
                return true;
            return false;
        }

        public static bool OutOfIndexLimit(this Vector3Int index) {

            if ( index.x < 0 || index.x >=wight || index.y < 0 || index.z < 0 || index.z >= wight )
                return true;
            return false;
        }


    }
}
