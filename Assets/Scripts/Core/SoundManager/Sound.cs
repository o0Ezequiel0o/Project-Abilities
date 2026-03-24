using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "Sound", menuName = "Sound Manager/Sound", order = 1)]
public class Sound : ScriptableObject
{
    [field: SerializeField] public AudioClip AudioClip { get; private set; }
    [field: SerializeField] public AudioMixerGroup AudioMixerGroup { get; private set; }

    public void PlaySound()
    {
        SoundManager.PlaySound(this);
    }
}