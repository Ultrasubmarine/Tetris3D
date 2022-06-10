using UnityEngine;

namespace IntegerExtension
{
    public static class ExtensionMetodsForMatrix
    {
        private static int _absMinCoordinat;
        private static int _difference; // index - coordinate
        private static int _wight;

        public static void SetSizePlane(int wight)
        {
            _absMinCoordinat = wight / 2;
            _difference = 0 - -_absMinCoordinat;
            _wight = wight;
        }

        public static int ToIndex(this int coordinat)
        {
            return coordinat + _difference;
        }

        public static int ToCoordinat(this int index)
        {
            return index - _difference;
        }

        public static Vector3Int ToIndex(this Vector3Int coordinat)
        {
            return new Vector3Int(coordinat.x.ToIndex(), coordinat.y, coordinat.z.ToIndex());
        }

        public static bool OutOfCoordinatLimit(this Vector3Int coordinat)
        {
            if (Mathf.Abs(coordinat.x) > _absMinCoordinat || coordinat.y < 0 ||
                Mathf.Abs(coordinat.z) > _absMinCoordinat)
                return true;
            return false;
        }

        public static bool OutOfCoordinatLimit(this CoordinatXZ coordinat)
        {
            if (Mathf.Abs(coordinat.x) > _absMinCoordinat ||
                Mathf.Abs(coordinat.z) > _absMinCoordinat)
                return true;
            return false;
        }
        public static bool OutOfIndexLimit(this Vector3Int index)
        {
            if (index.x < 0 || index.x >= _wight || index.y < 0 || index.z < 0 || index.z >= _wight)
                return true;
            return false;
        }
    }
}