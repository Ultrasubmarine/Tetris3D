using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class MiniStarUIAnimation : MonoBehaviour
{
    public Action<int> onAnimationFinished;
    
    [SerializeField] private float _time;

    [SerializeField] private Vector3 _scale = new Vector3(100f,100f,100f);
    [SerializeField] private Vector3 _fromScale = new Vector3(1f,1f,1f);
    [SerializeField] private float _timeAlphaStar = 1.5f;
    [SerializeField] private float _timeDelayBetweenAlphaStar = 1.5f;
    [SerializeField] private float _timeAlphaOreol = 1.5f;
    [SerializeField] private float _timeDisappear = 1;
    
 //   private List<Sequence> animations;
//    private List<Sequence> animationsDissapear;
    
    [SerializeField] private MiniStar[] Stars;
    
    [SerializeField] private float _starRotationSpeed = 20;
    private float _rotation;  //common oreol rotation
    
    // Start is called before the first frame update
    void Start()
    {
      //  animations = new List<Sequence>();
       // animationsDissapear = new List<Sequence>(); 
        
        var m =  Stars[0].starMesh.material;
        var m2 =  Stars[0].oreolRender.color;
        
        for (int i = 0; i < Stars.Length; i++)
        {
           // animations.Add(DOTween.Sequence().SetAutoKill(false).Pause());
          //  animationsDissapear.Add( DOTween.Sequence().SetAutoKill(false).Pause());

          Stars[i].animation.Append(Stars[i].starMesh.material
                  .DOColor(new Color(m.color.r, m.color.g, m.color.b, 1f), _timeAlphaStar)
                  .From(new Color(m.color.r, m.color.g, m.color.b, 0f))) //.SetLoops(3, LoopType.Yoyo))
              .Join(Stars[i].transform.DOScale(_scale, _time / 2).From(_fromScale));
              //  .Append(Stars[i].myTransform.DOScale(_scale, _time / 2).From(Vector3.zero));
                
            Stars[i].animationsDissapear.Append(Stars[i].oreolRender.DOColor(new Color(m2.r, m2.g, m2.b, 1f), _timeAlphaOreol)
                    .From(new Color(m2.r, m2.g, m2.b, 0f)))
                //.Join(Stars[i].myTransform.DOAnchorPosY(_deltaMove + FinishPoint, _timeMoving).From(Vector2.up * FinishPoint)
                   // .SetLoops(3, LoopType.Yoyo))
                .Append(Stars[i].oreolRender.DOColor(new Color(m2.r, m2.g, m2.b, 0f), _timeDisappear / 2))
                .Join(Stars[i].starMesh.material.DOColor(new Color(m.color.r, m.color.g, m.color.b, 0f), _timeDisappear))
                .AppendInterval(_timeDelayBetweenAlphaStar);

            Stars[i].animation.OnUpdate(() =>
            {
                _rotation += Time.deltaTime *_starRotationSpeed;
                if (_rotation > 360.0f)
                {
                    _rotation = 0.0f;
                }
                Stars[i].oreol.localRotation = Quaternion.Euler(0, 0, _rotation);
            });
            Stars[i].animationsDissapear.OnUpdate(() =>
            {
                _rotation += Time.deltaTime *_starRotationSpeed;
                if (_rotation > 360.0f)
                {
                    _rotation = 0.0f;
                }
                Stars[i].oreol.localRotation = Quaternion.Euler(0, 0, _rotation);
            });
            Stars[i].animation.OnComplete(() =>
            {
                var a = i;
                onAnimationFinished.Invoke(a);
            });
            Stars[i].animation.OnComplete(() =>
            {
                var a = i;
                onAnimationFinished.Invoke(a);
                Stars[a].gameObject.SetActive(false);
            });
            
         //   animations.Add(a);
         //   animationsDissapear.Add(ad);
         Stars[i].gameObject.SetActive(false);
        }
    }


    public void ShowMiniStar(int index)
    {
        Stars[index].gameObject.SetActive(true);
        Stars[index].animation.Rewind();
        Stars[index].animation.Play();
    }

    public void DissapearStars(int index)
    {
        for (int i = 0; i <= index; i++)
        {
            Stars[index].animation.Complete();
            Stars[index].animationsDissapear.Rewind();
            Stars[index].animationsDissapear.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
