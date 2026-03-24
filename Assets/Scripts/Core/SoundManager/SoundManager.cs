using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] private AudioSource audioSourcePrefab;

    [Space]

    [SerializeField] private int poolAudioSources;
    [SerializeField] private bool expandPool;

    private readonly GameObjectPool<AudioSource> audioSourcePool = new GameObjectPool<AudioSource>();

    protected override void OnInitialization()
    {
        for(int i = 0; i < poolAudioSources; i++)
        {
            AudioSource audioSource = Instantiate(audioSourcePrefab, transform);
            audioSource.gameObject.SetActive(false);
            audioSourcePool.Add(audioSource);
        }
    }

    private static AudioSource GetFreePooledSound()
    {
        if (Instance.expandPool)
        {
            return Instance.audioSourcePool.Get(Instance.audioSourcePrefab, Instance.transform);
        }
        else
        {
            return Instance.audioSourcePool.Get();
        }
    }

    public static void PlaySound(Sound sound)
    {
        AudioSource audioSource = GetFreePooledSound();

        if (audioSource == null) return;

        audioSource.gameObject.SetActive(true);

        audioSource.outputAudioMixerGroup = sound.AudioMixerGroup;
        audioSource.PlayOneShot(sound.AudioClip);
    }
}