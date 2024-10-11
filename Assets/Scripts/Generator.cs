using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

// made by Daehui
public class Generator : MonoBehaviour, IDamage
{
    #region References
    [Header("References")]
    [Tooltip("don't have to allocate this yet. it will be used after implement light change shader")]
    [SerializeField] GameObject _light;
    #endregion

    #region PrivateVariables
    [Header("")]
    [SerializeField] Vector2 _lightSize;
    [SerializeField] Vector2 _lightCenter;

    [SerializeField] AudioClip _generatorSFX;
    [SerializeField] AudioClip _generatorDestroyedSFX;


    private AudioSource _as;
    float _stereoPanAmount, _finalSoundNumerator;
    #endregion

    #region PublicVariables
    #endregion

    #region PrivateMethods
    void Start()
    {
        _as = GetComponent<AudioSource>();
        _stereoPanAmount = SoundManager.Instance.stereoPanAmount;
        _finalSoundNumerator = SoundManager.Instance.finalSoundNumerator / 2f;

        DetectLightables().ForEach(lightable => lightable.IsLighted = true);
        StartCoroutine(playGeneratorSound());
    }

    List<ILightable> DetectLightables()
    {
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(_lightCenter, _lightSize, 0f, LayerMask.GetMask("Enemy"));

        return hitColliders.Select(collider => collider.GetComponent<ILightable>())
            .Where(lightable => lightable != null)
            .ToList();
    }

    private void FixedUpdate()
    {
        CalcSound_Direction_Distance();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_lightCenter, _lightSize);
    }
    #endregion

    #region PublicMethods
    public void Dead()
    {
        DetectLightables().ForEach(lightable => lightable.IsLighted = false);
        Destroy(gameObject);
    }

    public void GetDamaged(Vector2 attackedDirection, int damage = 1)
    {
        Dead();
    }

    IEnumerator playGeneratorSound()
    {
        while (true) {
            GetComponent<AudioSource>().PlayOneShot(_generatorSFX);
            yield return new WaitForSeconds(_generatorSFX.length);
        }
    }

    private void CalcSound_Direction_Distance()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Vector2 playerPos = player.transform.position;
            _as.panStereo = (playerPos.x - transform.position.x) / _stereoPanAmount;
            float distance = Vector2.Distance(playerPos, transform.position);
            _as.volume = _finalSoundNumerator / distance;
        }
    }

    private void OnDestroy()
    {
        if (SoundManager.Instance == null) return;
        SoundManager.Instance.PlaySound(_generatorDestroyedSFX, transform.position);
    }
    #endregion
}
