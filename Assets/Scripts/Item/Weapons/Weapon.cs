using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Made by JK3WN

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
public class Weapon : ScriptableObject
{
    public string WeaponName;
    [Header("References")]
    public GameObject BulletPrefab;
    public GameObject WavePrefab;
    public Sprite BulletImage;
    [Header("Weapon Info")]
    public float RPM;
    public int Ammo;
    public float BulletSpeed;
    public AudioClip FireSound;
    [Header("Wave Info")]
    public Color WaveColor;
    public float DestroyTime;
}
