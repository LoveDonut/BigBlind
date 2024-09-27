using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Made by JK3WN
public class DoorKick : MonoBehaviour
{
    private AudioSource _as;
    private Rigidbody2D _rb;

    private void Start()
    {
        if(GetComponent<Rigidbody2D>() != null) _rb = GetComponent<Rigidbody2D>();
        if(GetComponent<AudioSource>() != null) _as = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_rb.angularVelocity > 200 || _rb.angularVelocity < -200) _as.Play();
    }
}
