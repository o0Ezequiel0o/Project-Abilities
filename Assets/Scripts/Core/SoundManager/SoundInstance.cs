using UnityEngine;

public class SoundInstance : MonoBehaviour, IPoolableGameObjectConfirmator
{
    [SerializeField] private AudioSource audioSource;

    private bool startedPlaying = false;

    public bool CanGetPoolable => startedPlaying && !audioSource.isPlaying;

    public void OnPoolableGet() { }

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