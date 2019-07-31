using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleAnimationScript : MonoBehaviour {
    [SerializeField] GameObjectPool _Pool;
    private void Awake() {
        Messenger<int>.AddListener(GameEvent.DESTROY_LAYER.ToString(), StartAnimationParticle);
    }

    private void OnDestroy() {
        Messenger<int>.RemoveListener(GameEvent.DESTROY_LAYER.ToString(), StartAnimationParticle);
    }

    void StartAnimationParticle(int layer) {

        GameObject boom = _Pool.CreateObject( new Vector3(0, (layer + 0.5f)));
        StartCoroutine(DestroyParticle(boom));
    }

    IEnumerator DestroyParticle( GameObject particle)
    {
        yield return new WaitForSeconds( 5);       
        _Pool.DestroyObject(particle);
    }
}