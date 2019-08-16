﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using IntegerExtension;

public class PlaneMatrix : Singleton<PlaneMatrix> {

    // DELETE
    [SerializeField] StateMachine machine;
    [SerializeField] HeightHandler _HeightHandler;
    //
    public Block[,,] _matrix;

    [Header("Size plane")]
    int _limitHeight = 18; 
    [SerializeField] int _Wight;
    [SerializeField] int _Height;

    public int Wight { get { return _Wight;  } }
    public int Height { get { return _Height - 1; } } // высота отсчитывается от 0

    public int LimitHeight { get { return _limitHeight; } }
    public int CurrentHeight { get { return _HeightHandler.CurrentHeight; } } 
   
    protected override void Init() {
        ExtensionMetodsForMatrix.SetSizePlane(_Wight);
        _matrix = new Block[_Wight, _Height, _Wight];

        for (int i = 0; i < _Wight; i++) {
            for (int j = 0; j < _Height; j++) {
                for (int k = 0; k < _Wight; k++) {
                    _matrix[i, j, k] = null;
                }
            }
        }
    }
    
    private void Start() {
        Messenger.AddListener(StateMachine.StateMachineKey + EMachineState.Collection, CheckCollections);
    }

    private void OnDestroy() {
        Messenger.RemoveListener(StateMachine.StateMachineKey + EMachineState.Collection, CheckCollections);
    }

    public void SetLimitHeight( int limit) {
        _limitHeight = limit;
    }

    public bool CheckEmptyPlaсe( Element element, Vector3Int direction) {

        if (element.MyBlocks.Count == 0) 
            return false;
        
        Vector3Int newCoordinat;
        foreach (Block item in element.MyBlocks) {
            if (!item.IsDestroy) {

                newCoordinat = new Vector3Int(item.Coordinates.x, item.Coordinates.y, item.Coordinates.z) + direction;

                if (newCoordinat.OutOfCoordinatLimit()) 
                    return false;               

                if ( !ReferenceEquals(_matrix[newCoordinat.x.ToIndex(), newCoordinat.y, newCoordinat.z.ToIndex()],null)) {
                    if (!element.IsBind)
                        return false;
                    if (!element.MyBlocks.Contains(_matrix[newCoordinat.x.ToIndex(), newCoordinat.y, newCoordinat.z.ToIndex()])) 
                        return false;                
                }
            }
        }
        return true;
    }

    public bool CheckEmptyPlace( int x_index, int y_index, int z_index) {
        return ReferenceEquals( _matrix[x_index, y_index, z_index] ,null);   
    }

    #region привязка/отвязка эл-та к матрице
    public void BindToMatrix(Element element) {

        int x, y, z;
        foreach (Block item in element.MyBlocks) {
            if (ReferenceEquals(item,null) || item.IsDestroy)
                continue;
            x = item.Coordinates.x;
            y = item.Coordinates.y;
            z = item.Coordinates.z;

            _matrix[x.ToIndex(), y, z.ToIndex()] = item;
        }
        element.IsBind = true;
    }

    public void UnbindToMatrix(Element element) {

        int x, y, z;
        foreach (Block item in element.MyBlocks) {
            if ( ReferenceEquals(item,null) || item.IsDestroy)
                continue;
            x = item.Coordinates.x;
            y = item.Coordinates.y;
            z = item.Coordinates.z;

            _matrix[x.ToIndex(), y, z.ToIndex()] = null;
        }
        element.IsBind = false;
    }
    #endregion

    #region сбор коллекций в слоях матрицы
    private void CheckCollections() {
        if (CollectLayers())
            machine.ChangeState(EMachineState.DropAllElements);
        else 
            machine.ChangeState(EMachineState.Empty);
        
    }
    private bool CollectLayers() {

        bool flag = false; ;
        for (int y = 0; y < _limitHeight; y++) {
            if (CheckCollectedInLayer(y)) {
                DestroyLayer(y);
                flag = true;
            }
        }
        return flag;
    }
    private bool CheckCollectedInLayer(int layer) {

        for (int x = 0; x < Wight; x++) {
            for (int z = 0; z < Wight; z++) {
                if ( ReferenceEquals(  _matrix[x, layer, z], null ) )
                {
                    return false;    
                }
            }
        }
        return true;       
    }
    
    private void DestroyLayer(int layer)
    {
        Messenger<int>.Broadcast(GameEvent.DESTROY_LAYER.ToString(), layer);
        for (int x = 0; x < Wight; x++) {
            for (int z = 0; z < Wight; z++) {
                _matrix[x, layer, z].IsDestroy = true;
                _matrix[x, layer, z] = null;           
            }
        }
    }
    #endregion

    public Vector3Int FindLowerAccessiblePlace() {

        int min = Height - 1;
        int curr_min;

        Vector3Int min_point = new Vector3Int(0, min, 0);

        for (int x = 0; x < Wight && min != 0; ++x) {
            for (int z = 0; z < Wight && min !=0; ++z) {
                curr_min = MinHeightInCoordinates(x, z);
                if(curr_min < min ) {
                    min = curr_min;
                    min_point = new Vector3Int(x, min, z);
                }
            }
        }
        return min_point;
    }
    public int MinHeightInCoordinates(int x_index, int z_index)
    {
        for (int y = _matrix.GetUpperBound(1) - 1; y >= 0; --y)
        {
            if ( !ReferenceEquals(  _matrix[x_index, y, z_index], null ) )
                return y+1;
        }
        return 0;        
    }
}
