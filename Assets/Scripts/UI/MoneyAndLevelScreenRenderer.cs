using UnityEngine;
using Zeke.UI;
using TMPro;

[RequireComponent(typeof(MoneyHandler), typeof(LevelHandler))]
public class MoneyAndLevelRenderer : MonoBehaviour
{
    [SerializeField] private MoneyHandler moneyHandler;
    [SerializeField] private LevelHandler levelHandler;

    [Space]

    [SerializeField] private UIWindow windowPrefab;

    private UIWindow window;

    private void Reset()
    {
        moneyHandler = GetComponentInChildren<MoneyHandler>();
        levelHandler = GetComponentInChildren<LevelHandler>();
    }

    private void Start()
    {
        window = Instantiate(windowPrefab, GameInstance.ScreenCanvas.transform);

        levelHandler.onLevelUp.Subscribe(UpdateLevel);
        moneyHandler.onUsedMoney.Subscribe(UpdateMoney);
        moneyHandler.onReceivedMoney.Subscribe(UpdateMoney);

        UpdateLevel(levelHandler.Level);
        UpdateMoney(moneyHandler.Money);
    }

    private void UpdateLevel(int level)
    {
        window.TryGetElement<TextMeshProUGUI>("Level").text = levelHandler.Level.ToString();
    }

    private void UpdateMoney(int money)
    {
        window.TryGetElement<TextMeshProUGUI>("Money").text = $"$ {moneyHandler.Money}";
    }

    private void OnDestroy()
    {
        if (window == null) return;
        window.DestroyWindow();
    }
}