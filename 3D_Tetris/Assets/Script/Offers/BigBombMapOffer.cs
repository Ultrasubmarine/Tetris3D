using DG.Tweening;
using Script.GameLogic.TetrisElement;
using Script.PlayerProfile;
using UnityEngine;

namespace Script.Offers
{
    public class BigBombMapOffer : MonoBehaviour
    {
        private static int useInGame;
        [SerializeField] private int maxUseInGame = 2;
        
        
        [SerializeField] private RectTransform _bombIcon;
        [SerializeField] private CanvasGroup _offer;
        [SerializeField] private GameObject _particles;
        
        private bool _isShow = false;
        
        //Animation
        private Sequence _show, _hide;
        [SerializeField] private float _yMove = -100;
        [SerializeField] private float _time = 0.5f;
        
        private void Start()
        {
            var rectTransform = _offer.GetComponent<RectTransform>();
            
            _show = DOTween.Sequence().SetAutoKill(false).Pause();
            _show.Append(_offer.DOFade(1, _time / 2).From(0))
                .Join(rectTransform.DOAnchorPosY(0, _time / 2).From(Vector2.up * _yMove));

            _hide = DOTween.Sequence().SetAutoKill(false).Pause();
            _hide.Append(rectTransform.DOScale(Vector3.one * 1.3f, 0.5f).From(Vector3.one))
                .Append(rectTransform.DOScale(Vector3.one, _time / 2))
                .Join(_offer.DOFade(0, _time / 2).From(1))
                .Join(rectTransform.DOAnchorPosY(_yMove, _time / 2).From(Vector2.zero)).OnComplete( () =>
                {
                    _bombIcon.DOKill();
                    
                    rectTransform.localScale = Vector3.one;
                    _offer.gameObject.SetActive(false);
                    _hide.Rewind();
                });
            
            Clear();
            if(useInGame <maxUseInGame)
                Show();
        }

        public void CheckShowOffer()
        {
        }

        private void Show()
        {
            _isShow = true;
            _offer.gameObject.SetActive(true);
            _show.Rewind();
            _show.Play();
            
            _bombIcon.DOScale(new Vector3(0.8f, 0.8f, 0.8f), 0.8f).From(Vector3.one * 1.2f).SetLoops(-1,LoopType.Yoyo);
        }

        private void Hide()
        {
            _isShow = false;
            _hide.Rewind();
            _hide.Play();
        }

        public void Apply()
        {
            useInGame++;
            //todo ads
            PlayerSaveProfile.instance.SetBombAmount(PlayerSaveProfile.instance._bombAmount + 1);
            Hide();
        }
        
        public void Clear()
        {
            _offer.gameObject.SetActive(false);
            _isShow = false;
        }
    }
}