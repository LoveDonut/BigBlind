using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Made by JK3WN
public class WeaponPickup : MonoBehaviour
{
    #region References
    [Header("References")]
    public Weapon WeaponData;
    [SerializeField] AudioClip _pickUpSound;
    #endregion

    #region PrivateVariables
    private SpriteRenderer _spriteRenderer;
    private Collider2D _collider;
    #endregion

    #region PrivateMethods
    private void Start()
    {
        if(GetComponent<SpriteRenderer>() != null) _spriteRenderer = GetComponent<SpriteRenderer>();
        if(GetComponent<Collider2D>() != null) _collider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if(_pickUpSound != null) collision.GetComponent<AudioSource>().PlayOneShot(_pickUpSound);
            _spriteRenderer.enabled = false;
            _collider.enabled = false;
            collision.GetComponent<PlayerShoot>().ChangeWeapon(WeaponData);
            if(_pickUpSound != null) Invoke("DelayedDestroy", _pickUpSound.length * 2);
            else DelayedDestroy();
        }
    }

    void DelayedDestroy()
    {
        Destroy(gameObject);
    }
    #endregion
}
