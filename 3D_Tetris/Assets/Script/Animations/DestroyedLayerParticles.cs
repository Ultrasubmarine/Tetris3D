using System.Collections;
using System.Collections.Generic;
using Helper.Patterns;
using UnityEngine;

public class DestroyedLayerParticles : MonoBehaviour
{
    [SerializeField] private GameObject _particleSystem;
    
    private Pool<GameObject> _pool;
    private List<GameObject> _booms;
    
    private void Start()
    {
        _pool = new Pool<GameObject>(_particleSystem);
        _booms = new List<GameObject>();
        
        RealizationBox.Instance.matrix.OnDestroyLayer += StartAnimationParticle;
    }

    private void StartAnimationParticle(int layer)
    {
        var boom = _pool.Pop();
        boom.transform.position = new Vector3(0, layer + 0.5f);
        
        _booms.Add(boom);
        StartCoroutine( DestroyParticle(boom));
    }

    private IEnumerator DestroyParticle(GameObject particle)
    {
        yield return new WaitForSeconds(5);
        _booms.Remove(particle);
        _pool.Push(particle);
    }

    public void ClearAll()
    {
        StopAllCoroutines();
        foreach (var b in _booms)
        {
            _pool.Push(b);
        }
        _booms.Clear();
    }
}