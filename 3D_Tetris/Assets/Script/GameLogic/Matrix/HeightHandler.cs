using UnityEngine;

public class HeightHandler : MonoBehaviour
{
    private PlaneMatrix _matrix;

    [SerializeField] [Space(20)] private int _LimitHeight;
    [SerializeField] private int _CurrentHeight;

    public int LimitHeight => _LimitHeight;
    public int CurrentHeight => _CurrentHeight;

    private void Start()
    {
        _matrix = PlaneMatrix.Instance;
        _matrix.SetLimitHeight(_LimitHeight);
    }
    
    public bool CheckOutOfLimit()
    {
        CheckHeight();
        return OutOfLimitHeight();
    }

    public void CheckHeight()
    {
        _CurrentHeight = 0;
        int check;

        for (var x = 0; x < _matrix.wight && !OutOfLimitHeight(); x++)
        for (var z = 0; z < _matrix.wight && !OutOfLimitHeight(); z++)
        {
            check = _matrix.MinHeightInCoordinates(x, z);
            if (check > _CurrentHeight) _CurrentHeight = check;
        }
   }

    private bool OutOfLimitHeight()
    {
        if (_CurrentHeight <= LimitHeight)
            return false;
        return true;
    }
}