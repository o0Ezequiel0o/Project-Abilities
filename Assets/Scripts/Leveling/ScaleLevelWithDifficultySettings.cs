using UnityEngine;

[CreateAssetMenu(fileName = "Level Scaling Settings", menuName = "Level Scaling Settings", order = 1)]
public class ScaleLevelWithDifficultySettings : ScriptableObject
{
    [field: SerializeField] public float LevelPerDifficulty { get; private set; }
}