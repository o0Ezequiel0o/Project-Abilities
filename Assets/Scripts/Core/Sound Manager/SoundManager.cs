using UnityEngine;
using Zeke.PoolableGameObjects;

public class SoundManager : GameService<SoundManager>
{
    [SerializeField] private AudioSource audioSourcePrefab;

    [Space]

    [SerializeField] private int poolAudioSources;
    [SerializeField] private bool expandPool;

    private readonly GameObjectPool<AudioSource> audioSourcePool = new GameObjectPool<AudioSource>();

    protected override void InitializeService()
    {
        for(int i = 0; i < poolAudioSources; i++)
        {
            AudioSource audioSource = Instantiate(audioSourcePrefab, transform);
            audioSource.gameObject.SetActive(false);
            audioSourcePool.Add(audioSource);
        }
    }

    private AudioSource GetFreePooledSound()
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

    public void PlaySound(Sound sound)
    {
        AudioSource audioSource = GetFreePooledSound();

        if (audioSource == null) return;

        audioSource.gameObject.SetActive(true);

        audioSource.outputAudioMixerGroup = sound.AudioMixerGroup;
        audioSource.PlayOneShot(sound.AudioClip);
    }
}