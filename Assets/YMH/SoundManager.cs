using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    public float BPM;


    [SerializeField] private AudioSource[] audioSources;
    [SerializeField] private float stereoPanAmount = 10f;
    [SerializeField] private float finalSoundNumerator = 5f;

    private int currentAudioSourceIndex = 0;

    private void Awake() => Instance = this;

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
}