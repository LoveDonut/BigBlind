using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamage
{
    public abstract void GetDamaged(Vector2 attackedDirection, GameObject from, int damage = 1);

    public abstract void Dead();
}
