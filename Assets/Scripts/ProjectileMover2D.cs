using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ProjectileMover2D : MonoBehaviour, IParriable
{
    #region References
    [Header("References")]
    public GameObject Hit;
    public GameObject Flash;

    public GameObject[] Detached;
    #endregion

    #region PrivateVariables
    private Rigidbody2D _rb;
    #endregion

    #region PublicVariables
    public float Speed = 15f;
    public float HitOffset = 0f;
    public bool UseFirePointRotation;
    public Vector3 RotationOffset = new Vector3(0, 0, 0);

    public Vector3 AimPos;

    // added by KimDaehui
    public bool IsParried { get; set; }
    public bool IsFromPlayer { get; set; }
    #endregion



    void Start()
    {
        IsParried = false;
        _rb = GetComponent<Rigidbody2D>();
        if (Flash != null)
        {
            //Instantiate flash effect on projectile position
            var flashInstance = Instantiate(Flash, transform.position, Quaternion.identity);
            flashInstance.transform.forward = gameObject.transform.forward;

            //Destroy flash effect depending on particle Duration time
            var flashPs = flashInstance.GetComponent<ParticleSystem>();
            if (flashPs != null)
            {
                Destroy(flashInstance, flashPs.main.duration);
            }
            else
            {
                var flashPsParts = flashInstance.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(flashInstance, flashPsParts.main.duration);
            }
        }
        Destroy(gameObject, 5);
    }

    void FixedUpdate()
    {
        if (Speed != 0)
        {
            _rb.velocity = transform.forward * Speed;
            //transform.position += transform.forward * (speed * Time.deltaTime);         
        }
    }

    //https ://docs.unity3d.com/ScriptReference/Rigidbody.OnCollisionEnter.html
    void OnCollisionEnter2D(Collision2D collision)
    {
        if(gameObject == null) return;

//        IDamage damagable = collision.gameObject.GetComponentInParent<IDamage>();
        IDamage damagable = collision.gameObject.GetComponent<IDamage>();

        // added by KimDaehui
        // prevent attack player self
        if (damagable != null && !IsParried && !(IsFromPlayer && collision.gameObject.CompareTag("Player")))
        {
            damagable.GetDamaged(AimPos.normalized);
        }

        //Lock all axes movement and rotation
        if(_rb != null)
        {
            _rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
        Speed = 0;

        ContactPoint2D contact = collision.contacts[0];
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
        Vector3 pos = contact.point + contact.normal * HitOffset;

        //Spawn hit effect on collision
        if (Hit != null)
        {
            var hitInstance = Instantiate(Hit, pos, rot);
            if (UseFirePointRotation) { hitInstance.transform.rotation = gameObject.transform.rotation * Quaternion.Euler(0, 180f, 0); }
            else if (RotationOffset != Vector3.zero) { hitInstance.transform.rotation = Quaternion.Euler(RotationOffset); }
            else { hitInstance.transform.LookAt(contact.point + contact.normal); }

            //Destroy hit effects depending on particle Duration time
            var hitPs = hitInstance.GetComponent<ParticleSystem>();
            if (hitPs != null)
            {
                Destroy(hitInstance, hitPs.main.duration);
            }
            else
            {
                var hitPsParts = hitInstance.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(hitInstance, hitPsParts.main.duration);
            }
        }

        //Removing trail from the projectile on cillision enter or smooth removing. Detached elements must have "AutoDestroying script"
        foreach (var detachedPrefab in Detached)
        {
            if (detachedPrefab != null)
            {
                detachedPrefab.transform.parent = null;
                Destroy(detachedPrefab, 1);
            }
        }
        //Destroy projectile on collision
        Destroy(gameObject);
    }
}
