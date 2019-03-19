using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleAnimationScript : MonoBehaviour {

    [SerializeField] ParticleSystem _PrototipeParticle;

    private void Awake()
    {
        Messenger<int>.AddListener(GameEvent.DESTROY_LAYER, StartAnimationParticle);
    }

    private void OnDestroy()
    {
        
    }

    void StartAnimationParticle(int layer)
    {
        ParticleSystem boom = Instantiate(_PrototipeParticle, new Vector3(0, (layer + 0.5f), 0), Quaternion.identity);
        boom.Play();
        
        //TODO Destroy? Property 'UnityEngine.ParticleSystem.duration' is obsolete: duration property is deprecated.Use main.duration instead.
        Destroy(boom.gameObject, boom.duration);

    }
}
