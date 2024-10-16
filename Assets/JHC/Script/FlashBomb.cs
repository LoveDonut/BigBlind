using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// made by JHC
public class FlashBomb : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] List<ParticleSystem> _glowParticleSystems;
    [SerializeField] CanvasGroup _flashImage;
    [SerializeField] Image _InvertImage;

    [Header("Variable")]
  //  [SerializeField] float _scaleSize;
    [SerializeField] float _invertDuration;
    [SerializeField] float _flashIntensity;

    [Header("UpscaleVariable")]
    [SerializeField] float _upscaleDuration;
    [SerializeField] Ease _upscaleEase;


    [Header("DeleteVariable")]
    [SerializeField] float _deleteDuration;
    [SerializeField] Ease _deleteEase;

    
    public void TriggerScreenFlash()
    {
        _flashImage.DOKill();
        //_flashSpriteRenderer.transform.DOScale(_upscaleDuration, _scaleSize).SetEase(_upscaleEase);
        CameraShake.Instance.shakeCamera(1f,1f);
        StartCoroutine(_InvertScreen());
        _flashImage.DOFade(_flashIntensity, _upscaleDuration / 2).SetEase(_upscaleEase).OnComplete(() => {
            _flashImage.DOFade(0, _deleteDuration / 2).SetEase(_deleteEase);
        });
    }
    private IEnumerator _InvertScreen()
    {
        _InvertImage.DOKill();
        _InvertImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(_invertDuration+_upscaleDuration);
        _InvertImage.gameObject.SetActive(false);
        _InvertImage.transform.localScale = Vector3.zero;
    }

    //public void TriggerGlowParticles()
    //{
    //    foreach(var particle in _glowParticleSystems)
    //        particle.Play();
    //}

    [ContextMenu("Flash")]
    public void Flash()
    {
        TriggerScreenFlash();
    }
}
