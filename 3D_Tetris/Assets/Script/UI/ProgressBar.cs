using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] protected Image _fillImage;

    [SerializeField] private float _fullLengthDuration;

    [SerializeField] private bool _inverse;

    public float value { get; private set; }

    public void Initialize(float startValue = 0f)
    {
        value = startValue;
        _fillImage.DOKill();
        _fillImage.fillAmount = value;
    }

    public virtual void SetProgress(float newValue)
    {
        value = Mathf.Clamp01(newValue);

        if (_inverse)
            value = 1f - value;

        var duration = Math.Abs(value - _fillImage.fillAmount) * _fullLengthDuration;

        _fillImage.DOKill();
        _fillImage.DOFillAmount(value, duration).SetUpdate(UpdateType.Normal, true);
    }

    protected virtual void OnDestroy()
    {
        if (_fillImage != null)
            _fillImage.DOKill();
    }
}
