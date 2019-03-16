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

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void StartAnimationParticle( int layer)
    {
        ParticleSystem boom;

        boom = Instantiate(_PrototipeParticle, new Vector3(0, (layer + 0.5f), 0), Quaternion.identity);
        boom.Play();
        Destroy(boom.gameObject, boom.duration);

    }
}
