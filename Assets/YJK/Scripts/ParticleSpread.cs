using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSpread : MonoBehaviour
{
    #region References
    [SerializeField] UnityEngine.GameObject _particlePrefab;
    #endregion

    UnityEngine.GameObject _particle;

    public int Segments = 20;
    public float BPM = 60f;
    public float DestroyTime = 1f;
    public float Speed = 2f;
    public Color ParticleColor = Color.white;
    public bool Repeat = true;

    private void Start()
    {
        if (Repeat) InvokeRepeating("SpawnParticleWave", 0, 60f / BPM);
    }

    public void SpawnParticleWave()
    {
        for(float i = 0; i < 360f; i += (360f / Segments))
        {
            _particle = Instantiate(_particlePrefab, transform.position, Quaternion.identity);
            _particle.GetComponent<SpriteRenderer>().color = ParticleColor;
            _particle.GetComponent<Particle>().DestroyTime = DestroyTime;
            _particle.GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Cos(Mathf.Deg2Rad * i), Mathf.Sin(Mathf.Deg2Rad * i)) * Speed;
        }
    }
}
