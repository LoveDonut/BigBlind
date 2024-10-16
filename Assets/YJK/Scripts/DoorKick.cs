using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Made By JK3WN
public class DoorKick : MonoBehaviour
{
    #region References
    [SerializeField] Transform _left;
    [SerializeField] Transform _right;

    [Header("SFX")]
    [SerializeField] AudioClip _smashDoor;
    [SerializeField] AudioClip _enemyBurst;
    [SerializeField] AudioClip _crashDoor;
    #endregion

    #region PrivateVariables
    [SerializeField] float _kickSpeed = 10f;
    private AudioSource _as;
    private Collider2D _collider;
    private Rigidbody2D _rb;
    private SpriteRenderer _sr;
    private bool _isMoving;
    #endregion

    #region PrivateMethods
    private void Start()
    {
        if(GetComponent<Rigidbody2D>() != null) _rb = GetComponent<Rigidbody2D>();
        if(GetComponent<AudioSource>() != null) _as = GetComponent<AudioSource>();
        if(GetComponent<SpriteRenderer>() != null) _sr = GetComponent<SpriteRenderer>();
        if(GetComponent<Collider2D>() != null) _collider = GetComponent<Collider2D>();
        _isMoving = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_isMoving) return;
        if (collision.gameObject.CompareTag("Enemy"))
        {
            _as.PlayOneShot(_enemyBurst);
            collision.gameObject.GetComponent<EnemyHealth>().GetDamaged(collision.transform.position - this.transform.position);
            if (GetComponent<WaveManager>() != null) GetComponent<WaveManager>().SpawnWave();
        }
        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Obstacle"))
        {
            _as.PlayOneShot(_crashDoor);
            _sr.enabled = false;
            _collider.enabled = false;
            _rb.bodyType = RigidbodyType2D.Static;
            DelayedDestroy();
            if (GetComponent<WaveManager>() != null) GetComponent<WaveManager>().SpawnWave();
        }
    }
    
    IEnumerator DelayedDestroy()
    {
        yield return new WaitForSeconds(_crashDoor.length * 2);
        Destroy(gameObject);
    }
    #endregion

    #region PublicMethods
    public void DoorKicked(Transform kicker)
    {
        _rb.bodyType = RigidbodyType2D.Kinematic;
        _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        GetComponent<BoxCollider2D>().size = new Vector2(0.8f, 3f);
        GetComponent<Collider2D>().isTrigger = true;
        _as.PlayOneShot(_smashDoor);
        if (GetComponent<WaveManager>() != null) GetComponent<WaveManager>().SpawnWave();
        if (Vector2.Distance(_left.position, kicker.position) < Vector2.Distance(_right.position, kicker.position))
        {
            _rb.velocity = (this.transform.position - _left.position) * _kickSpeed;
        }
        else
        {
            _rb.velocity = (this.transform.position - _right.position) * _kickSpeed;
        }
        _isMoving = true;
    }
    #endregion
}
