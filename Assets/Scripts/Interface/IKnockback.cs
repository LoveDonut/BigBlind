using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IKnockback
{
    public void Knockback(Vector2 knockBackDirection, StateMachine playerState);
}
