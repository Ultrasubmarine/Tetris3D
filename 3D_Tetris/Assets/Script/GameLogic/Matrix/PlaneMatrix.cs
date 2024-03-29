﻿using System;
using System.Collections.Generic;
using UnityEngine;
using IntegerExtension;
using Script.GameLogic.GameItems;

public class PlaneMatrix : Singleton<PlaneMatrix>
{
    private Block[,,] _matrix;

    [Header("Size plane")] private int _limitHeight = 18;
    [SerializeField] private int _wight;
    [SerializeField] private int _height;

    public int wight => _wight;

    public int height // высота отсчитывается от 0
        => _height - 1;

    public int limitHeight => _limitHeight;

    public event Action<int> OnDestroyLayer;
    public event Action<List<Vector3>, bool> OnDestroyBlock;
    public event Action<bool> OnDestroyLayerEnd; // true - if destroy lvl
    
    protected override void Init()
    {
        ExtensionMetodsForMatrix.SetSizePlane(_wight);
        _matrix = new Block[_wight, _height, _wight];

        for (var i = 0; i < _wight; i++)
        for (var j = 0; j < _height; j++)
        for (var k = 0; k < _wight; k++)
            _matrix[i, j, k] = null;
    }

    public void SetLimitHeight(int limit)
    {
        _limitHeight = limit;
    }

    #region FOR_PICKABLE_BLOCKS
    
    public List<Block> GetPickableBlocksForElement(Element element)
    {
        if (!element)
            return null;
        if (element.blocks.Count == 0)
            return null;

        List<Block> PickableBlocks = new List<Block>();
        Vector3Int newCoordinat;
        foreach (var item in element.blocks)
            if (!item.isDestroy)
            {
                newCoordinat = new Vector3Int(item.coordinates.x, item.coordinates.y, item.coordinates.z);

                if (newCoordinat.OutOfCoordinatLimit())
                    return null;

                if (!ReferenceEquals(_matrix[newCoordinat.x.ToIndex(), newCoordinat.y, newCoordinat.z.ToIndex()], null))
                {
                    if (_matrix[newCoordinat.x.ToIndex(), newCoordinat.y, newCoordinat.z.ToIndex()].IsPickable())
                        PickableBlocks.Add(_matrix[newCoordinat.x.ToIndex(), newCoordinat.y, newCoordinat.z.ToIndex()]);
                }
            }
        return PickableBlocks;
    }

    public bool BindBlock(Block block) //for pickable blocks
    {
        if (ReferenceEquals(_matrix[block.coordinates.x.ToIndex(), block.coordinates.y, block.coordinates.z.ToIndex()], null))
        {
            _matrix[block.coordinates.x.ToIndex(), block.coordinates.y, block.coordinates.z.ToIndex()] = block;
            return true;
        }
        return false;
    }

    public void UnbindBlock(Block block)
    {
        if (ReferenceEquals(_matrix[block.coordinates.x.ToIndex(), block.coordinates.y, block.coordinates.z.ToIndex()], block))
        {
            _matrix[block.coordinates.x.ToIndex(), block.coordinates.y, block.coordinates.z.ToIndex()] = null;
        }
    }
    #endregion
    
    public bool CheckEmptyPlaсe(Element element, Vector3Int direction, bool checkUpperBlocks = false)
    {
        if (!element)
            return false;
        if (element.blocks.Count == 0)
            return false;

        Vector3Int newCoordinat;
        foreach (var item in element.blocks)
            if (!item.isDestroy)
            {
                newCoordinat = new Vector3Int(item.coordinates.x, item.coordinates.y, item.coordinates.z) + direction;

                if (newCoordinat.OutOfCoordinatLimit())
                    return false;

                if (!ReferenceEquals(_matrix[newCoordinat.x.ToIndex(), newCoordinat.y, newCoordinat.z.ToIndex()], null))
                {
                    if (_matrix[newCoordinat.x.ToIndex(), newCoordinat.y, newCoordinat.z.ToIndex()].IsPickable())
                        continue;
                    if (!element._isBind)
                        return false;
                    if (!element.blocks.Contains(_matrix[newCoordinat.x.ToIndex(), newCoordinat.y,
                        newCoordinat.z.ToIndex()]))
                        return false;
                }
                if (checkUpperBlocks && newCoordinat.y + 1 < _height && !RealizationBox.Instance.elementDropper.isWaitingMerge)
                {
                    if (!ReferenceEquals(_matrix[newCoordinat.x.ToIndex(), newCoordinat.y + 1, newCoordinat.z.ToIndex()], null))
                        return false;
                }
            }
        return true;
    }
    
    public bool CheckEmptyPlace(int x_index, int y_index, int z_index)
    {
        return ReferenceEquals(_matrix[x_index, y_index, z_index], null);
    }

    public Block GetBlockInPlace(int x_index, int y_index, int z_index)
    {
        return _matrix[x_index, y_index, z_index];
    }
    
    #region привязка/отвязка эл-та к матрице

    public void BindToMatrix(Element element)
    {
        int x, y, z;
        foreach (var item in element.blocks)
        {
            if (ReferenceEquals(item, null) || item.isDestroy)
                continue;
            x = item.coordinates.x;
            y = item.coordinates.y;
            z = item.coordinates.z;

            _matrix[x.ToIndex(), y, z.ToIndex()] = item;
        }

        element._isBind = true;
    }

    public void UnbindToMatrix(Element element)
    {
        int x, y, z;
        foreach (var item in element.blocks)
        {
            if (ReferenceEquals(item, null) || item.isDestroy)
                continue;
            x = item.coordinates.x;
            y = item.coordinates.y;
            z = item.coordinates.z;

            _matrix[x.ToIndex(), y, z.ToIndex()] = null;
        }

        element._isBind = false;
    }

    #endregion

    #region сбор коллекций в слоях матрицы

    public void CollectLayers()
    {
        for (var y = 0; y < _limitHeight; y++)
            if (CheckCollectedInLayer(y))
            {
                CollectLayer(y);
                RealizationBox.Instance.gameCamera.onStabilizationEnd += OnCameraStabilizationEnd;
                RealizationBox.Instance.gameCamera.SetStabilization();
                return;
            }
       OnDestroyLayerEnd?.Invoke(false);
    }

    public void OnCameraStabilizationEnd()
    {
        RealizationBox.Instance.gameCamera.onStabilizationEnd -= OnCameraStabilizationEnd;
        OnDestroyLayerEnd?.Invoke(true);
    }

    private bool CheckCollectedInLayer(int layer)
    {
        int stoneAmount = 0;
        
        for (var x = 0; x < wight; x++)
        for (var z = 0; z < wight; z++)
        {
            if (ReferenceEquals(_matrix[x, layer, z], null) || _matrix[x, layer, z].IsPickable())
                return false;
            if (_matrix[x, layer, z].blockType == BlockType.stone)
                stoneAmount++;
        }

        if (stoneAmount == 9)
            return false;
        
        return true;
    }

    private void CollectLayer(int layer)
    {
        OnDestroyLayer?.Invoke(layer);
        
        for (var x = 0; x < wight; x++)
        for (var z = 0; z < wight; z++)
        {
            if (_matrix[x, layer, z].blockType == BlockType.stone)
            {
                int currLives = _matrix[x, layer, z].DecreaseLives();
                if (currLives > 0)
                    continue;
            }
            
            _matrix[x, layer, z].isDestroy = true;
            _matrix[x, layer, z].Hide();
            
            if(_matrix[x, layer, z].IsPickable())
                _matrix[x, layer, z].Pick(null);
            
            _matrix[x, layer, z].Collect();
            _matrix[x, layer, z] = null;
        }
    }

    public void DestroyBlocksAround(Vector3Int point, List<Vector3Int> directions, bool collectStars = false)
    {
        List<Vector3> destroyPos = new List<Vector3>();
        
        foreach (var d in directions)
        {
            Vector3Int pos = new Vector3Int(point.x + d.x, point.y + d.y, point.z + d.z);
            if(pos.OutOfIndexLimit())
                continue;
            if(CheckEmptyPlace(pos.x,pos.y, pos.z))
                continue;
            
          //  if(collectStars)
          //     _matrix[pos.x, pos.y, pos.z].Collect();
          //else
            _matrix[pos.x, pos.y, pos.z].Destroy();
            
            _matrix[pos.x, pos.y, pos.z].isDestroy = true;

            destroyPos.Add(_matrix[pos.x, pos.y, pos.z].myTransform.position);
            _matrix[pos.x, pos.y, pos.z] = null;
        }
        
        destroyPos.Add(_matrix[point.x, point.y, point.z].myTransform.position);
        _matrix[point.x, point.y, point.z].isDestroy = true;
        _matrix[point.x, point.y, point.z] = null;
        
        OnDestroyBlock?.Invoke(destroyPos, false);
    }
    
    public void DestroyBlocksInLayers(Vector3Int point, int layersAmount, bool collectStars = false)
    {
        List<Vector3> destroyPos = new List<Vector3>();

        for (int y = point.y; y > point.y - layersAmount && y >= 0; y--)
        {
            for (var x = 0; x < wight; x++)
            for (var z = 0; z < wight; z++)
            {
                Vector3Int pos = new Vector3Int(x, y, z);
                
                if(pos.OutOfIndexLimit())
                    continue;
                if(CheckEmptyPlace(pos.x,pos.y, pos.z))
                    continue;
            
                if(collectStars)
                    _matrix[pos.x, pos.y, pos.z].Collect();
                else
                    _matrix[pos.x, pos.y, pos.z].Destroy();
            
                _matrix[pos.x, pos.y, pos.z].isDestroy = true;

                if (point.x == x && point.y == y && point.z == z)
                    continue;
                
                destroyPos.Add(_matrix[pos.x, pos.y, pos.z].myTransform.position);
                _matrix[pos.x, pos.y, pos.z] = null;
            }
        }
        
        destroyPos.Add(_matrix[point.x, point.y, point.z].myTransform.position);
        _matrix[point.x, point.y, point.z].isDestroy = true;
        _matrix[point.x, point.y, point.z] = null;
        
        OnDestroyBlock?.Invoke(destroyPos, true);
    }
    #endregion

    public Vector3Int FindLowerAccessiblePlace()
    {
        var min = height - 1;
        int curr_min;

        var min_point = new Vector3Int(0, min, 0);

        for (var x = 0; x < wight && min != 0; ++x)
        for (var z = 0; z < wight && min != 0; ++z)
        {
            curr_min = MinHeightInCoordinates(x, z);
            if (curr_min < min)
            {
                min = curr_min;
                min_point = new Vector3Int(x, min, z);
            }
        }

        return min_point;
    }

    public int MinHeightInCoordinates(int x_index, int z_index)
    {
        for (var y = _matrix.GetUpperBound(1) - 1; y >= 0; --y)
            if (!ReferenceEquals(_matrix[x_index, y, z_index], null) && !_matrix[x_index, y, z_index].IsPickable())
                return y + 1;
        return 0;
    }
    
    public int MinHeightInCoordinatesAfterPoint(int x_index, int z_index, int y_point)
    {
        for (var y = y_point; y >= 0; --y)
            if (!ReferenceEquals(_matrix[x_index, y, z_index], null) && !_matrix[x_index, y, z_index].IsPickable())
                return y + 1;
        return 0;
    }

    public void Clear()
    {
        _matrix = new Block[_wight, _height, _wight];
    }
}