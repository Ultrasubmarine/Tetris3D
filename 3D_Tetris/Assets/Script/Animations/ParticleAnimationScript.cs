using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleAnimationScript : MonoBehaviour {
    [SerializeField] ParticleSystem _PrototipeParticle;

    private void Awake() {
        Messenger<int>.AddListener(GameEvent.DESTROY_LAYER.ToString(), StartAnimationParticle);
    }

    private void OnDestroy() {
    }

    void StartAnimationParticle(int layer) {
        //TODO Instantiate и Destroy при уничтожении слоя = каждый раз создается мусор

        ParticleSystem boom = Instantiate(_PrototipeParticle, new Vector3(0, (layer + 0.5f), 0), Quaternion.identity);
        boom.Play();

        Destroy(boom.gameObject, boom.duration); //TODO rider ругается на ParticleSystem.duration;
    }
}