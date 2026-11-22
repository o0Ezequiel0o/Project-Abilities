using UnityEngine;
using Zeke.UI;
using TMPro;

public class ExperienceScreenLoader : MonoBehaviour
{
    [SerializeField] private UIWindow window;

    private LevelHandler levelHandler;

    private void Awake()
    {
        Run.Instance.onPlayerAdded += OnPlayerAdded;
    }

    private void OnPlayerAdded(Player newPlayer)
    {
        Run.Instance.onPlayerAdded -= OnPlayerAdded;

        bool playerHasRequiredComponents = newPlayer.TryGetComponent(out levelHandler);

        if (!playerHasRequiredComponents) return;

        levelHandler.onReceiveExperience.Subscribe(OnReceiveExperience);
        levelHandler.onLevelUp.Subscribe(OnLevelUp);

        window.TryGetElement<TextMeshProUGUI>("Level Text").SetText(GetLevelText(levelHandler.Level));
        window.TryGetElement<StatusBar>("Bar").UpdateBar(levelHandler.Experience / Mathf.Max(float.Epsilon, levelHandler.ExperienceRequired));
    }

    private void OnReceiveExperience(int experience)
    {
        window.TryGetElement<StatusBar>("Bar").UpdateBar((float)levelHandler.Experience / levelHandler.ExperienceRequired);
    }

    private void OnLevelUp(LevelHandler.LevelUpInfo levelUpInfo)
    {
        window.TryGetElement<TextMeshProUGUI>("Level Text").SetText(GetLevelText(levelHandler.Level));
        window.TryGetElement<StatusBar>("Bar").UpdateBar((float)levelHandler.Experience / levelHandler.ExperienceRequired);
    }

    private string GetLevelText(int level) => $"Level {level}";
}