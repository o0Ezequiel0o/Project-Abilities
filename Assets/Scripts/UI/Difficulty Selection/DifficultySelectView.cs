using UnityEngine;
using TMPro;

public class DifficultySelectView : MonoBehaviour
{
    [Header("Dependency")]
    [SerializeField] private DifficultySelector difficultySelector;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI descriptionText;

    private void Awake()
    {
        difficultySelector.onDifficultySelected += UpdateDisplay;
    }

    private void UpdateDisplay(GameDifficulty gameDifficulty)
    {
        descriptionText.SetText(gameDifficulty.Description);
    }
}