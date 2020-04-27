using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(RectTransform))]
public class BottomPanelAnimation : MonoBehaviour
{
    private CanvasGroup _canvasGroup;

    private RectTransform _rectTransform;
    
    [SerializeField] private float _time;

    [SerializeField] private float _yMove;

    private Sequence _show;
    private Sequence _hide;

    // Start is called before the first frame update
    void Start()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _rectTransform = GetComponent<RectTransform>();

        _show = DOTween.Sequence().SetAutoKill(false).Pause();
        _show.Append(_canvasGroup.DOFade(1, _time / 2).From(0))
            .Join(_rectTransform.DOAnchorPosY(0, _time / 2).From(Vector2.up * _yMove));//From(125,false))
        
        _hide = DOTween.Sequence().SetAutoKill(false).Pause();
        _hide.Append(_canvasGroup.DOFade(0, _time / 2).From(1))
            .Join(_rectTransform.DOAnchorPosY(_yMove, _time / 2).From(Vector2.zero)).OnComplete( () => gameObject.SetActive(false));
    }
    
    // Update is called once per frame
    public void Show()
    {
        gameObject.SetActive(true);
        _show.Rewind();
        _show.Play();
    }

    public void Hide()
    {
        _hide.Rewind();
        _hide.Play();
    }
}
