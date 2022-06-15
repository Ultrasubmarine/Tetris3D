using System;
using DG.Tweening;
using Script.GameLogic.StoneBlock;
using UnityEngine;

namespace Script.GameLogic
{
    public class NextElementUI : MonoBehaviour
    {
        [SerializeField] private Transform _nextElementParent;
        [SerializeField] private GameObject _bomb;
        [SerializeField] private GameObject _bigBomb;
        [SerializeField] private GameObject _evilBox;
        
        private Element _nextElement;
        private GameLogicPool _pool;
        private StoneBlockManager _stoneManager;
        
        private void Start()
        {
            _pool = RealizationBox.Instance.gameLogicPool;
            _stoneManager = RealizationBox.Instance.stoneBlockManager;
            
            _nextElement= _pool.CreateEmptyElement();
            _nextElement.myTransform.parent = _nextElementParent;
            _nextElement.myTransform.localPosition = Vector3.zero;
            _nextElement.myTransform.localRotation = Quaternion.identity;
            _nextElement.myTransform.localScale = Vector3.one * 70;

            RealizationBox.Instance.islandTurn.extraTurn.Add(_nextElement.myTransform);
            RealizationBox.Instance.generator.onNextElementGenerated += CreateNextElement;
        }

        private void CreateNextElement(AbstractElementInfo element)
        {
            Clear();

            switch (element.type)
            {
                case ElementType.none: return;
                case ElementType.element:
                {
                    float xMax, zMax , yMax, xMin, zMin, yMin;
                    xMax = zMax = yMax = int.MinValue;
                    xMin = zMin = yMin = int.MaxValue;
        
                    for(int i = 0; i<element.blocks.Count; i++)
                    {
                        var position = element.blocks[i];
                        Vector3Int v = new Vector3Int((int) position.x, (int) position.y, (int) position.z);
                        _pool.CreateBlock(v, _nextElement,element.material);

                        Vector3 ansPos = _nextElement.blocks[i].myTransform.localPosition;
                        xMax = xMax < ansPos.x? ansPos.x : xMax;
                        zMax = zMax < ansPos.z? ansPos.z : zMax;
                        yMax = yMax < ansPos.y? ansPos.y : yMax;
            
                        xMin = xMin > ansPos.x? ansPos.x : xMin;
                        zMin = zMin > ansPos.z? ansPos.z : zMin;
                        yMin = yMin > ansPos.y? ansPos.y : yMin;
                    }

                    float xCenter, zCenter, yCenter;
                    xCenter = (xMax + xMin) / 2f;
                    zCenter = (zMax + zMin) / 2f;
                    yCenter = (yMax + yMin) / 2f;
        
                    if (element.stone)
                    {
                        RealizationBox.Instance.stoneBlockManager.TransformToStone(_nextElement);
                    }
                    
                    foreach (var block in _nextElement.blocks)
                    {
                        Vector3 np = block.myTransform.localPosition - new Vector3(xCenter, yCenter, zCenter);
                        block.myTransform.localPosition = np;
                        block.myTransform.localScale = Vector3.one * 0.97f;
                    }
                    break;
                }
                case ElementType.bomb:
                {
                    _bomb.SetActive(true);
                    _bomb.transform.DOScale(new Vector3(110, 110, 110), 0.8f).From(Vector3.one * 100).SetLoops(-1,LoopType.Yoyo);

                    break;
                }
                case ElementType.bigBomb:
                {
                    _bigBomb.SetActive(true);
                    _bigBomb.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.8f).From(Vector3.one).SetLoops(-1,LoopType.Yoyo);

                    break;
                }
                case ElementType.evilBox:
                {
                    _evilBox.SetActive(true);
                    _evilBox.transform.DOScale(new Vector3(0.9f, 0.9f, 0.9f), 1f).From(Vector3.one).SetLoops(-1,LoopType.Yoyo);

                    break;
                }
            }
        }

        public void Clear()
        {
            foreach (var block in _nextElement.blocks)
            {
                block.myTransform.localScale = Vector3.one * 0.97f;
                _pool.DeleteBlock(block);
            }
            _nextElement.RemoveBlocksInList(_nextElement.blocks.ToArray());

            _bomb.transform.DOKill();
            _bomb.SetActive(false);
            
            _bigBomb.transform.DOKill();
            _bigBomb.SetActive(false);
            
            _evilBox.transform.DOKill();
            _evilBox.SetActive(false);
        }
    }
}