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
    #endregion

    #region PublicVariables
    #endregion

    #region PrivateMethods
    void Start()
    {
        DetectLightables().ForEach(lightable => lightable.IsLighted = true);
    }

    List<ILightable> DetectLightables()
    {
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(_lightCenter, _lightSize, 0f, LayerMask.GetMask("Enemy"));

        return hitColliders.Select(collider => collider.GetComponent<ILightable>())
            .Where(lightable => lightable != null)
            .ToList();
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
    #endregion
}
