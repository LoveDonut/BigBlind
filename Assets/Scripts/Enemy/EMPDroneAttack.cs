using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EMPDroneAttack : EnemyAttack
{
    public override void InitAttack()
    {
        IDamage damagable;
        EMP emp;

        if(TryGetComponent<EMP>(out emp))
        {
            emp.ParalyzeBoomBox();
        }
        if(TryGetComponent<IDamage>(out damagable))
        {
            damagable.GetDamaged(-GetDirectionToPlayer(), gameObject);
        }
    }

    public override void EndAttack()
    {
    }
}
