using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    public float BPM;
    [Space]

    public AudioSource BGMaudio, HeartBeatAudio, FlashBang;
    [Space]

    [SerializeField] private AudioSource[] audioSources;
    public float stereoPanAmount = 10f;
    public float finalSoundNumerator = 5f;

    private int currentAudioSourceIndex = 0;

    [Space]

    [SerializeField] AudioMixer _mixer;
    [SerializeField] AudioMixerGroup _amg;

    private void Awake() => Instance = this;

    private void Start()
    {
        foreach (var x in audioSources)
        {
            x.outputAudioMixerGroup = _amg;
        }

        //BurstFlashBang();

    }

    public void PlaySound(AudioClip clip, Vector2 enemyPos)
    {
        AudioSource audioSource = GetNextAudioSource();
        audioSource.panStereo = 0f;
        audioSource.volume = 1f;

        if (enemyPos != Vector2.zero)
        {
            CalcSound_Direction_Distance(enemyPos, audioSource);
        }

        audioSource.PlayOneShot(clip);
    }

    private AudioSource GetNextAudioSource()
    {
        AudioSource audioSource = audioSources[currentAudioSourceIndex];
        currentAudioSourceIndex = (currentAudioSourceIndex + 1) % audioSources.Length;
        return audioSource;
    }

    private void CalcSound_Direction_Distance(Vector2 position, AudioSource audioSource)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Vector2 playerPos = player.transform.position;
            audioSource.panStereo = (playerPos.x - position.x) / stereoPanAmount;
            float distance = Vector2.Distance(playerPos, position);
            audioSource.volume = Mathf.Min(finalSoundNumerator / distance, 1f);
        }
    }

    public void BurstFlashBang()
    {
        StartCoroutine(VolumeControl(BGMaudio, 0, BGMaudio.volume, 5));
        StartCoroutine(VolumeControl(HeartBeatAudio, 0, HeartBeatAudio.volume, 5));
        StartCoroutine(StartLowPass(5));
    }


    IEnumerator StartLowPass(float time)
    {
        float elapsedTime = 0f;

        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / time;
            _mixer.SetFloat("lowpass", Mathf.Lerp(1500, 5000, t));

            yield return null;
        }
    }

    IEnumerator VolumeControl(AudioSource _as,float firstVolume, float targetVolume, float time)
    {
        _as.volume = firstVolume;
        float elapsedTime = 0f;



        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / time;
            _as.volume = Mathf.Lerp(firstVolume, targetVolume, t);
            yield return null;
        }

        _as.volume = targetVolume;
    }


}