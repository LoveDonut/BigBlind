using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodEffect : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] GameObject _bloodPrefab;
    [SerializeField] List<Color> _colorList;
    [SerializeField] List<Sprite> _bloodSpriteList;

    [Header("Variable")]
    [SerializeField] float _createDuration;

    Sprite _randomBloodSprite => _bloodSpriteList[UnityEngine.Random.Range(0, _bloodSpriteList.Count)];
    Color _randomColor => _colorList[UnityEngine.Random.Range(0, _colorList.Count)];

    void Start()
    {
    }

   public void InstantiateBloodEffect(Transform obj,int rotationZ = 0)
    {
        var _bloodObj = Instantiate(_bloodPrefab, obj.transform.position,Quaternion.identity);
        _bloodObj.transform.rotation = Quaternion.Euler(new Vector3(0,0,rotationZ));
        _bloodObj.GetComponent<SpriteRenderer>().sortingLayerName = "Blood";
        _bloodObj.GetComponent<SpriteRenderer>().sortingOrder = GameManager.Instance.Set_New_Blood_Index();
        SpriteRenderer _bloodObjSr = _bloodPrefab.GetComponent<SpriteRenderer>();
        _bloodObjSr.sprite = _randomBloodSprite;
        _bloodObjSr.color = _randomColor;

        _bloodObj.transform.localScale = Vector3.zero;
        _bloodObj.transform.DOScale(UnityEngine.Random.Range(0.5f, 1f), _createDuration).SetEase(Ease.InExpo);

    }

    [ContextMenu("TestBlood")]
    void Test()
    {
        InstantiateBloodEffect(GameObject.Find("Player").transform,UnityEngine.Random.Range(0,360));
    }
}
