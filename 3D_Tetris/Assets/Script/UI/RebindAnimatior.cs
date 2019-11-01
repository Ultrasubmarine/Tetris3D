using System.Collections;
using DG.Tweening;
using UnityEngine;

public class RebindAnimatior : MonoBehaviour
{
    private Animator _animator;

    // Use this for initialization
    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private Tween g;

    private void Awake()
    {
        g = transform.DOMove(Vector3.up, 1);
    }

    private void OnDisable()
    {
        _animator.Rebind();
    }

    private void OnEnable()
    {
        Debug.Log("Start pause panel");

        var ht = iTween.Hash("from", 0, "to", .92f, "time", .5f, "onupdate", "changeMotionBlur");

//make iTween call:
        iTween.ValueTo(gameObject, ht);
        var ht2 = iTween.Hash("from", 1, "to", 0f, "time", .5f, "onupdate", "changeMotionBlur");

        iTween.FadeTo(gameObject, ht2);
    }
}