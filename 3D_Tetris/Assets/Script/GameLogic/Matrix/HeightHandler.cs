using System;
using UnityEngine;

public class HeightHandler : MonoBehaviour
{
    private PlaneMatrix _matrix;

    [SerializeField] [Space(20)] private int _limitHeight;
    
    [SerializeField] private int _currentHeight;

    public event Action<int, int> onHeightChange;
    
    public int limitHeight => _limitHeight;
    public int currentHeight => _currentHeight;

    private void Start()
    {
        _matrix = PlaneMatrix.Instance;
        _matrix.SetLimitHeight(_limitHeight);
        _matrix.OnDestroyLayer += DecrementHeight;
    }
    
    public bool CheckOutOfLimit()
    {
        CalculateHeight();
        return OutOfLimitHeight();
    }

    public void CalculateHeight()
    {
        _currentHeight = 0;
        int check;

        for (var x = 0; x < _matrix.wight && !OutOfLimitHeight(); x++)
        for (var z = 0; z < _matrix.wight && !OutOfLimitHeight(); z++)
        {
            check = _matrix.MinHeightInCoordinates(x, z);
            if (check > _currentHeight) _currentHeight = check;
        }
   }

    public void DecrementHeight(int layer)
    {
        _currentHeight--;
    }
    
    private bool OutOfLimitHeight()
    {
        if (_currentHeight <= limitHeight)
            return false;
        return true;
    }
}