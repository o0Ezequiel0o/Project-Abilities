using TMPro;
using UnityEngine;
using Zeke.UI;

public class ExperienceScreenRenderer : MonoBehaviour
{
    [Header("Dependency")]
    [SerializeField] private LevelHandler levelHandler;

    [Space]

    [SerializeField] private UIWindow windowPrefab;

    private UIWindow window;

    private void Reset()
    {
        levelHandler = GetComponentInChildren<LevelHandler>();
    }

    private void Start()
    {
        window = Instantiate(windowPrefab, Run.Instance.ScreenCanvas.transform);

        window.TryGetElement<TextMeshProUGUI>("Level Text").SetText(GetLevelText(levelHandler.Level));
        window.TryGetElement<StatusBar>("Bar").UpdateBar(levelHandler.Experience / levelHandler.ExperienceRequired);
    }

    private void OnEnable()
    {
        levelHandler.onReceiveExperience.Subscribe(OnReceiveExperience);
        levelHandler.onLevelUp.Subscribe(OnLevelUp);
    }

    private void OnDisable()
    {
        levelHandler.onReceiveExperience.Unsubscribe(OnReceiveExperience);
        levelHandler.onLevelUp.Unsubscribe(OnLevelUp);
    }

    private void OnDestroy()
    {
        if (window != null)
        {
            Destroy(window.gameObject);
        }
    }

    private void OnReceiveExperience(int experience)
    {
        window.TryGetElement<StatusBar>("Bar").UpdateBar(levelHandler.Experience / levelHandler.ExperienceRequired);
    }

    private void OnLevelUp(LevelHandler.LevelUpInfo levelUpInfo)
    {
        window.TryGetElement<TextMeshProUGUI>("Level Text").SetText(GetLevelText(levelHandler.Level));
        window.TryGetElement<StatusBar>("Bar").UpdateBar(levelHandler.Experience / levelHandler.ExperienceRequired);
    }

    private string GetLevelText(int level) => $"Level {level}";
}