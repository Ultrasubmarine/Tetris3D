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
    [SerializeField] private float _timeDisappear = 0.9f;

    [SerializeField] private MiniStar[] Stars;
    
    [SerializeField] private float _deltaMove = 20f;
    [SerializeField] private float _starRotationSpeed = 20;
    private float _rotation;  //common oreol rotation
    
    // Start is called before the first frame update
    void Start()
    {
        var m =  Stars[0].starMesh.material;
        var m2 =  Stars[0].oreolRender.color;
        
        for (int i = 0; i < Stars.Length; i++)
        {
            float FinishPoint = -Stars[i].myTransform.sizeDelta.y + Stars[i].myTransform.anchoredPosition.y;


            Stars[i].animation.Append(Stars[i].starMesh.material
                    .DOColor(new Color(m.color.r, m.color.g, m.color.b, 1f), _time / 3)
                    .From(new Color(m.color.r, m.color.g, m.color.b, 0f))) 
                .Join(Stars[i].myTransform.DOAnchorPosY(Stars[i].myTransform.anchoredPosition.y, _time / 2).From(Vector2.down * Screen.height / 2));

          int ind = i;
          Stars[i].animation.OnComplete(() =>
          {
              onAnimationFinished?.Invoke(ind);
          });
          
          Stars[i].animationsDissapear
              .Append(Stars[i].oreolRender.DOColor(new Color(m2.r, m2.g, m2.b, 0f), _timeDisappear/3))
                .Append(Stars[i].starMesh.material.DOColor(new Color(m.color.r, m.color.g, m.color.b, 0f), _timeDisappear));
          
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

    public void Clear(int amount)
    {
        _isShowStars = false;
        var m =  Stars[0].starMesh.material;
        
        for (int i = 0; i < amount; i++)
        {
            Stars[i].animation.Rewind();
            Stars[i].animation.Complete();
            Stars[i].animationsDissapear.Rewind();
            Stars[i].animationsDissapear.Complete();
            
         //   Stars[i].starMesh.material.SetColor(0,new Color(m.color.r, m.color.g, m.color.b,0f));
        }
    }
}
