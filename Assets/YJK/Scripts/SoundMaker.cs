using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundMaker : MonoBehaviour
{
    AudioSource _as;

    public AudioClip Clip;

    private void Start()
    {
        _as = GetComponent<AudioSource>();
        _as.PlayOneShot(Clip);
        Destroy(gameObject, Clip.length);
    }
}
