using UnityEngine;
using Zeke.PoolableGameObjects;

public class SoundInstance : MonoBehaviour, IPoolableGameObjectConfirmator
{
    [SerializeField] private AudioSource audioSource;

    private bool startedPlaying = false;

    public bool CanGetPoolable => startedPlaying && !audioSource.isPlaying;

    public void OnRetrievedFromPool() { }

    private void Update()
    {
        if (audioSource.isPlaying)
        {
            if (!startedPlaying)
            {
                startedPlaying = true;
            }
        }
        else if (startedPlaying)
        {
            gameObject.SetActive(false);
        }
    }
}