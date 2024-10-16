using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//made by JHC
public class FlashBangBox : ObstacleHealth
{
    [Header("Reference")]
    [SerializeField] FlashBomb bomb;
    
    public void Flash()
    {
        bomb.Flash();
    }
    public override void Dead()
    {
        Flash();
        base.Dead();
    }
}
