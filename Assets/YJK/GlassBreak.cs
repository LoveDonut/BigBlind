using NavMeshPlus.Components;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
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
        if(GetComponent<WaveManager>() != null) GetComponent<WaveManager>().Spawn_Wave();
        _spriteRenderer.enabled = false;
        _collider.enabled = false;
        GetComponent<NavMeshPlus.Components.NavMeshModifier>().overrideArea = false;
        GameObject.Find("NavMesh").GetComponent<NavMeshPlus.Components.NavMeshSurface>().BuildNavMesh();
    }

    IEnumerator DestroyAfterSound()
    {
        yield return new WaitForSeconds(_brokenGlass[_clipNum].length);
        Destroy(gameObject);
    }
}
