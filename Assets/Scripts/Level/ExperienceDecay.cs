using UnityEngine;

[RequireComponent(typeof(LevelHandler))]
public class ExperienceDecay : MonoBehaviour
{
    [Header("Dependency")]
    [SerializeField] private LevelHandler levelHandler;

    [Header("Settings")]
    [SerializeField, Min(0)] private float decay = 0.001f;

    private readonly Stat.Multiplier multiplier = new Stat.Multiplier(1f);

    private void Reset()
    {
        levelHandler = GetComponentInChildren<LevelHandler>();
    }

    private void Awake()
    {
        levelHandler.ExperienceMultiplier.AddMultiplier(multiplier);
    }

    private void Update()
    {
        multiplier.UpdateMultiplier(multiplier.Value * Mathf.Clamp01(1f - (decay * Time.deltaTime)));
    }
}