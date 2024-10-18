using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using EnemyState;

public class ShortAttack : EnemyAttack
{
    #region References
    #endregion

    #region PrivateVariables
    #endregion

    #region ProtectedVariables
    #endregion

    #region PublicVariables
    #endregion

    #region PrivateVariables
    #endregion

    #region PrivateMethods
    #endregion

    #region ProtectedMethods
    #endregion

    #region PublicMethods
    public override void InitAttack()
    {
        Weapon.SetActive(true);

        EnemyShortWeapon shortWeapon = GetComponentInChildren<EnemyShortWeapon>();
        if (shortWeapon != null)
        {
            shortWeapon.StartAttack();
        }
    }
    public override void EndAttack()
    {
        EnemyShortWeapon shortWeapon = GetComponentInChildren<EnemyShortWeapon>();
        if (shortWeapon != null)
        {
            shortWeapon.EndAttack();
        }
    }
    #endregion

}
