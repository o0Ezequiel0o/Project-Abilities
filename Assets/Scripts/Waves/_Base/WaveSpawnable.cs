using UnityEngine;

[CreateAssetMenu(fileName = "Wave Spawnable", menuName = "ScriptableObjects/WaveManager/WaveSpawnable", order = 1)]
public class WaveSpawnable : ScriptableObject
{
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: Header("Settings")]
    [field: SerializeField] public GameObject Prefab { get; private set; }
    [field: SerializeField] public int Weight { get; private set; } = 50;
    [field: SerializeField] public int Cost { get; private set; } = 1;

    void OnValidate()
    {
        ClampValues();
    }

    void Reset()
    {
        ClampValues();
    }

    void ClampValues()
    {
        Weight = Mathf.Max(1, Weight);
        Cost = Mathf.Max(1, Cost);
    }
}