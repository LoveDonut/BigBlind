using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// made by Chun Jin Ha
public class BloodEffect : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] ObjectPool _bloodEffectPool;
    [SerializeField] List<Color> _colorList;
    [SerializeField] List<Sprite> _bloodSpriteList;

    [Header("Variable")]
    [SerializeField] float _createDuration;
    [SerializeField] float _maintainDuration;
    [SerializeField] float _deleteDuration;
    public bool NeverDelete;
    public bool HasDirection;
    public bool IsEnemy;

    Sprite _randomBloodSprite => _bloodSpriteList[UnityEngine.Random.Range(0, _bloodSpriteList.Count)];
    Color _randomColor => _colorList[UnityEngine.Random.Range(0, _colorList.Count)];

    void Awake()
    {
        DOTween.SetTweensCapacity(tweenersCapacity: 500, sequencesCapacity: 100);
    }

    public void InstantiateBloodEffect(Vector2 pos, float rotationZ = 0, float scale = 1f)
    {
        GameObject bloodObj = _bloodEffectPool.GetObject();
        SpriteRenderer bloodObjSr = bloodObj.GetComponent<SpriteRenderer>();

        bloodObj.transform.position = pos;
        bloodObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, rotationZ));
        bloodObj.transform.localScale = Vector3.zero;

        bloodObjSr.sortingLayerName = "Blood";
        bloodObjSr.sortingOrder = GameManager.Instance.Set_New_Blood_Index();
        bloodObjSr.sprite = _randomBloodSprite;
        bloodObjSr.color = _randomColor;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(bloodObj.transform.DOScale(Random.Range(scale/2, scale), _createDuration).SetEase(Ease.InExpo));
        if (!NeverDelete)
        {
            sequence.AppendInterval(_maintainDuration);
            sequence.Append(DOTween.ToAlpha(
                () => bloodObjSr.color, x => bloodObjSr.color = x, 0f, _deleteDuration
            ).SetEase(Ease.InExpo).OnComplete(() => {
                bloodObj.transform.DOKill();
                _bloodEffectPool.ReturnObject(bloodObj);
            }));
        }
        sequence.Play();

    }
}
