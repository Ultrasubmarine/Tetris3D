using System;
using System.Collections;
using System.Collections.Generic;
using Helper.Patterns;
using IntegerExtension;
using Script.GameLogic.TetrisElement;
using UnityEngine;

namespace Script.Animations
{
    public class DropParticles : MonoBehaviour
    {
        [SerializeField] private GameObject _particleSystem;
        
        private Pool<GameObject> _pool;
        private PlaneMatrix _planeMatrix;
        private ElementData _elementData;
        private void Start()
        {
            _pool = new Pool<GameObject>(_particleSystem);
            _elementData = ElementData.Instance;
            _elementData.onMergeElement += StartAnimationParticle;
            _planeMatrix = RealizationBox.Instance.matrix;
            
        }

        private void StartAnimationParticle()
        {
            
            List<Vector3> positions = new List<Vector3>();

            foreach (var block in _elementData.newElement.blocks)
            {
                if (block._coordinates.y == 0)
                {
                    positions.Add(block.transform.position);
                    continue;
                }

                var otherBlock = _planeMatrix.GetBlockInPlace(block._coordinates.x.ToIndex(), block._coordinates.y - 1, block._coordinates.z.ToIndex());
                if (otherBlock != null && !_elementData.newElement.blocks.Contains(otherBlock))
                {
                    positions.Add(block.transform.position);
                }
            }

            var offset = new Vector3(0, 0.5f, 0);
            foreach (var pos in positions)
            {
                var boom = _pool.Pop();
                boom.transform.parent = gameObject.transform;
                boom.transform.position = pos - offset;
                StartCoroutine( DestroyParticle(boom));
            }
        }

        private IEnumerator DestroyParticle(GameObject particle)
        {
            yield return new WaitForSeconds(5);
            _pool.Push(particle);
        }

        private void OnDestroy()
        {
            _elementData.onMergeElement -= StartAnimationParticle;
        }
    }
}