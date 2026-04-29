using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Zeke.UI;
using System;
using TMPro;

public class DifficultySelector : MonoBehaviour
{
    [Tooltip("Where the selected difficulty will be saved (for persistent data)")]
    [SerializeField] private GameDifficulty selectedSave;

    [Space]

    [SerializeField] private ToggleGroup toggleGroup;
    [SerializeField] private UIWindow optionPrefab;
    [SerializeField] private RectTransform root;

    [Space]

    [SerializeField] private GameDifficulty defaultDifficulty;
    [SerializeField] private List<GameDifficulty> difficulties;

    public GameDifficulty SelectedDifficulty { get; private set; }
    public Action<GameDifficulty> onDifficultySelected;

    private void Awake()
    {
        for (int i = 0; i < difficulties.Count; i++)
        {
            UIWindow window = Instantiate(optionPrefab, root);

            GameDifficulty difficulty = difficulties[i];

            Toggle toggle = window.TryGetElement<Toggle>("Toggle");
            toggle.onValueChanged.AddListener((state) => OnToggleValueChanged(state, difficulty));
            toggle.group = toggleGroup;

            window.TryGetElement<TextMeshProUGUI>("Name").text = difficulty.Name;

            if (difficulty == defaultDifficulty || difficulties.Count == 1 || (defaultDifficulty == null && i == 0))
            {
                toggle.isOn = true;
            }
        }
    }

    private void OnToggleValueChanged(bool state, GameDifficulty difficulty)
    {
        if (state == false) return;
        OnDifficultySelected(difficulty);
    }

    private void OnDifficultySelected(GameDifficulty difficulty)
    {
        SelectedDifficulty = difficulty;
        selectedSave.LoadDifficulty(difficulty);
        onDifficultySelected?.Invoke(difficulty);
    }
}