using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class MiniStarUIAnimation : MonoBehaviour
{
    private bool _isShowStars = false;
    public Action<int> onAnimationFinished;
    
    [SerializeField] private float _time;

    [SerializeField] private Vector3 _scale = new Vector3(100f,100f,100f);
    [SerializeField] private Vector3 _fromScale = new Vector3(1f,1f,1f);
    [SerializeField] private float _timeDisappear = 1;

    [SerializeField] private MiniStar[] Stars;
    
    [SerializeField] private float _starRotationSpeed = 20;
    private float _rotation;  //common oreol rotation
    
    // Start is called before the first frame update
    void Start()
    {
        var m =  Stars[0].starMesh.material;
        var m2 =  Stars[0].oreolRender.color;
        
        for (int i = 0; i < Stars.Length; i++)
        {
            Stars[i].animation.Append(Stars[i].starMesh.material
                  .DOColor(new Color(m.color.r, m.color.g, m.color.b, 1f), _time / 3)
                  .From(new Color(m.color.r, m.color.g, m.color.b, 0f))) //.SetLoops(3, LoopType.Yoyo))
              .Join(Stars[i].transform.DOScale(_scale * 1.2f, _time / 3).From(_fromScale))
              .Append(Stars[i].transform.DOScale(_scale, _time / 3))
              .Join(Stars[i].oreolRender.DOColor(new Color(m2.r, m2.g, m2.b, 1f), _timeDisappear / 2).From(new Color(m2.r, m2.g, m2.b, 0f)));

          int ind = i;
          Stars[i].animation.OnComplete(() =>
          {
              onAnimationFinished.Invoke(ind);
          });
          
          Stars[i].animationsDissapear
              .Append(Stars[i].oreolRender.DOColor(new Color(m2.r, m2.g, m2.b, 0f), _timeDisappear/3))
                //.Join(Stars[i].myTransform.DOAnchorPosY(_deltaMove + FinishPoint, _timeMoving).From(Vector2.up * FinishPoint)
                   // .SetLoops(3, LoopType.Yoyo))
              //  .Append(Stars[i].oreolRender.DOColor(new Color(m2.r, m2.g, m2.b, 0f), _timeDisappear / 2))
                .Append(Stars[i].starMesh.material.DOColor(new Color(m.color.r, m.color.g, m.color.b, 0f), _timeDisappear));
               // .AppendInterval(_timeDelayBetweenAlphaStar);
          
          Stars[i].animationsDissapear.OnComplete(() =>
            {
                _isShowStars = false;
                Stars[ind].gameObject.SetActive(false);
            });

          Stars[i].gameObject.SetActive(false);
        }
    }


    public void ShowMiniStar(int index)
    {
        _isShowStars = true;
        Stars[index].gameObject.SetActive(true);
        Stars[index].animation.Rewind();
        Stars[index].animation.Play();
    }

    public void DissapearStars(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Stars[i].animation.Complete();
            Stars[i].animationsDissapear.Rewind();
            Stars[i].animationsDissapear.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isShowStars) return;
        
        foreach (var s in Stars)
        {
            _rotation += Time.deltaTime *_starRotationSpeed;
            if (_rotation > 360.0f)
            {
                _rotation = 0.0f;
            }
            s.oreol.localRotation = Quaternion.Euler(0, 0, _rotation);
        }
    }
}
