using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// made by KimDaehui
public class EnemyHealth : MonoBehaviour, IDamage
{
    #region References
    [Header("References")]
    [SerializeField] int _maxHp = 1;

    #endregion
    #region PrivateVariables

    #endregion

    #region PublicVariables
    public int CurrentHp { get; private set; }
    public int MaxHp { get; private set; }
    #endregion

    #region PrivateMethods

    void Start()
    {
        CurrentHp = MaxHp = _maxHp;
    }

    void MakeBlood(Vector3 aimPos)
    {
        BloodEffect bloodEffect = FindObjectsOfType<BloodEffect>().Where(x => x.IsEnemy).First();
        if (bloodEffect != null)
        {
            float angle = Mathf.Atan2(aimPos.y, aimPos.x) * Mathf.Rad2Deg - 90f;
            bloodEffect.InstantiateBloodEffect(transform.position, angle);
        }
    }
    #endregion

    #region PublicMethods
    public void GetDamaged(Vector2 attackedDirection, GameObject from, int damage = 1)
    {
        // for shielder
        ShielderAttack shielderAttack;
        if (TryGetComponent<ShielderAttack>(out shielderAttack))
        {
            Vector2 attackDirection = (from.transform.position - transform.position).normalized;

            float angleBetween = Vector2.Angle(shielderAttack._hitTransform.gameObject.transform.up, attackDirection);

//            Debug.Log($"angle Between : {angleBetween}");
            if(angleBetween <= shielderAttack.defenseAngle / 2f)
            {
                return;
            }

            //    Collider2D[] hits = shielderAttack.GetHittedColliderAtBox();
            //    foreach (Collider2D hit in hits)
            //    {
            //        if(from == hit.gameObject)
            //        {
            //            Debug.Log("Parry!");
            //            return;
            //        }
            //    }
        }


        CurrentHp -= damage;

        if (CurrentHp <= 0)
        {
            // CurrentHp = 0;
            Dead(attackedDirection);
        }
    }
    public void Dead()
    {
        Direction.Instance.Show_Flash_Effect();
        CameraShake.Instance.shakeCamera(5f, .1f);

        Destroy(gameObject);
    }
    void Dead(Vector3 attackedDirection)
    {
        if(CurrentHp == 0) MakeBlood(attackedDirection);

        Dead();
    }
    #endregion
}
