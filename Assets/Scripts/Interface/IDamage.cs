using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamage
{
    public abstract void GetDamaged(Vector2 attackedDirection, int damage = 1, bool WillBeInvincible = true);

    public abstract void Dead();
}
