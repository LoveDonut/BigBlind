using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Made by JK3WN
public class BoxBreak : MonoBehaviour, IDamage
{
    #region References
    [Header("References")]
    [SerializeField] AudioClip[] _brokenBox;
    #endregion

    [SerializeField] int CurrentHp = 1;
    private AudioSource _as;
    private Collider2D _collider;
    private SpriteRenderer _spriteRenderer;
    int _clipNum;

    // Start is called before the first frame update
    void Start()
    {
        if (GetComponent<AudioSource>() != null) _as = GetComponent<AudioSource>();
        if (GetComponent<Collider2D>() != null) _collider = GetComponent<Collider2D>();
        if (GetComponent<SpriteRenderer>() != null) _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Attack"))
        {
            Dead();
        }
    }

    public void GetDamaged(Vector2 attackedDirection, int damage = 1)
    {
        CurrentHp -= damage;
        if(CurrentHp <= 0)
        {
            CurrentHp = 0;
            Dead();
        }
    }

    public void Dead()
    {
        _clipNum = Random.Range(0, _brokenBox.Length);
        _as.PlayOneShot(_brokenBox[_clipNum]);
        if (GetComponent<WaveManager>() != null) GetComponent<WaveManager>().SpawnWave();
        _spriteRenderer.enabled = false;
        _collider.enabled = false;
        GetComponent<NavMeshPlus.Components.NavMeshModifier>().overrideArea = false;
        GameObject.Find("NavMesh").GetComponent<NavMeshPlus.Components.NavMeshSurface>().BuildNavMesh();
        Invoke("DelayedDestroy", _brokenBox.Length * 2);
    }

    void DelayedDestroy()
    {
        Destroy(gameObject);
    }
}
