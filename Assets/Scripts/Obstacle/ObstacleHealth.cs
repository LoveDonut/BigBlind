using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Made by JK3WN
public class ObstacleHealth : MonoBehaviour, IDamage
{
    #region References
    [Header("References")]
    [SerializeField] AudioClip[] _breakSound;
    [SerializeField] GameObject _soundMaker;
    #endregion

    #region PrivateVariables
    AudioSource _audioSource;
    Collider2D _collider;
    SpriteRenderer _spriteRenderer;

    int CurrentHp;
    int _clipNum;
    #endregion

    #region PublicVariables
    public int MaxHp = 1;
    #endregion

    #region PrivateMethods
    private void Start()
    {
        if(GetComponent<AudioSource>() != null) _audioSource = GetComponent<AudioSource>();
        if(GetComponent<Collider2D>() != null) _collider = GetComponent<Collider2D>();
        if(GetComponent<SpriteRenderer>() != null) _spriteRenderer = GetComponent<SpriteRenderer>();
        CurrentHp = MaxHp;
    }
    #endregion

    #region PublicMethods
    public void GetDamaged(Vector2 attackedDirection, GameObject gameObject, int damage = 1)
    {
        CurrentHp -= damage;
        if (CurrentHp <= 0)
        {
            CurrentHp = 0;
            Dead();
        }
    }

    public virtual void Dead()
    {
        _clipNum = Random.Range(0, _breakSound.Length);
        if (GetComponent<WaveManager>() != null) GetComponent<WaveManager>().SpawnWave();
        GetComponent<NavMeshPlus.Components.NavMeshModifier>().overrideArea = false;
        GameObject.Find("NavMesh").GetComponent<NavMeshPlus.Components.NavMeshSurface>().BuildNavMesh();
        GameObject sound = Instantiate(_soundMaker, transform.position, Quaternion.identity);
        sound.GetComponent<SoundMaker>().Clip = _breakSound[_clipNum];
        Destroy(gameObject);
    }
    #endregion
}
