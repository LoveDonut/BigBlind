using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Made By JK3WN
public class GlassBreak : MonoBehaviour
{
    #region References
    [Header("References")]
    [SerializeField] AudioClip[] _brokenGlass;
    #endregion

    private AudioSource _as;
    private Collider2D _collider;
    private SpriteRenderer _spriteRenderer;
    int _clipNum;

    // Start is called before the first frame update
    void Start()
    {
        if(GetComponent<AudioSource>() != null) _as = GetComponent<AudioSource>();
        if(GetComponent<Collider2D>() != null) _collider = GetComponent<Collider2D>();
        if(GetComponent<SpriteRenderer>() != null) _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Attack")) {
            BreakGlass();
        }
    }

    void BreakGlass()
    {
        _clipNum = Random.Range(0, _brokenGlass.Length);
        _as.PlayOneShot(_brokenGlass[_clipNum]);
        _spriteRenderer.enabled = false;
        _collider.enabled = false;
    }

    IEnumerator DestroyAfterSound()
    {
        yield return new WaitForSeconds(_brokenGlass[_clipNum].length);
        Destroy(gameObject);
    }
}
