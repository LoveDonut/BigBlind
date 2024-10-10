using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// made by Daehui
public class Generator : MonoBehaviour, IDamage
{
    #region References
    [Header("References")]
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
    void DetectEnemies()
    {
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(_lightCenter, _lightSize, 0f, LayerMask.GetMask("Enemy"));

        foreach (Collider2D hitCollider in hitColliders)
        {
        }
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
        Destroy(gameObject);
    }

    public void GetDamaged(Vector2 attackedDirection, int damage = 1)
    {
        Dead();
    }
    #endregion
}
