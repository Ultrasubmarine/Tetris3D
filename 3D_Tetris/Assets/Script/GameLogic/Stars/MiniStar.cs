using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MiniStar : MonoBehaviour
{
    public GameObject star => _star;
    public MeshRenderer starMesh => _starMesh;
    public SpriteRenderer oreolRender => _oreolRender;
    public RectTransform myTransform => _myTransform;
    public Transform oreol => _oreol;
    
    [SerializeField] private GameObject _star;
    [SerializeField] private MeshRenderer _starMesh;
    [SerializeField] private SpriteRenderer _oreolRender;
    [SerializeField] private Transform _oreol;
    private RectTransform _myTransform;

    public Sequence animation => _animation;
    public Sequence animationsDissapear => _animationsDissapear;
    
    private Sequence _animation;
    private Sequence _animationsDissapear;
    
    // Start is called before the first frame update
    private void Awake()
    {
        _myTransform = GetComponent<RectTransform>();
        _animation = DOTween.Sequence().SetAutoKill(false).Pause();
        _animationsDissapear = DOTween.Sequence().SetAutoKill(false).Pause(); //throw new NotImplementedException();
    }

    void Start()
    {
        
    }
    
}
