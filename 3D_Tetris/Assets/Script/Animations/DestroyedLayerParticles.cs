using System.Collections;
using Helper.Patterns;
using UnityEngine;

public class DestroyedLayerParticles : MonoBehaviour
{
    [SerializeField] private GameObject _particleSystem;
    
    private Pool<GameObject> _pool;
    
    private void Start()
    {
        _pool = new Pool<GameObject>(_particleSystem);
        RealizationBox.Instance.matrix.OnDestroyLayer += StartAnimationParticle;
    }

    private void StartAnimationParticle(int layer)
    {
        var boom = _pool.Pop();
        boom.transform.position = new Vector3(0, layer + 0.5f);
        
        
        StartCoroutine( DestroyParticle(boom));
    }

    private IEnumerator DestroyParticle(GameObject particle)
    {
        yield return new WaitForSeconds(5);
        _pool.Push(particle);
    }
}