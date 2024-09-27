using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Revolver_UI : MonoBehaviour
{
    public List<Image> Bullets;
    public int _ammo = 0;

    public void FireBullet()
    {
        Bullets[Bullets.Count - _ammo - 1].GetComponent<Animator>().Play("Bullet_Shoot");
    }

    public void ReloadBullet(bool isReloadAll)
    {
        if (isReloadAll)
        {
            for(int i = 0; i < Bullets.Count - _ammo; i++) Bullets[i].GetComponent<Animator>().Play("Bullet_Reload");
        }
        else
        {
            Bullets[Bullets.Count - _ammo].GetComponent<Animator>().Play("Bullet_Reload");
        }
    }
}
