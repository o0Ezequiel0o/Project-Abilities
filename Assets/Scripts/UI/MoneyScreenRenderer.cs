using UnityEngine;
using Zeke.UI;
using TMPro;

[RequireComponent(typeof(MoneyHandler), typeof(LevelHandler))]
public class MoneyScreenRenderer : MonoBehaviour
{
    [SerializeField] private MoneyHandler moneyHandler;

    [Space]

    [SerializeField] private UIWindow windowPrefab;

    private UIWindow window;

    private void Reset()
    {
        moneyHandler = GetComponentInChildren<MoneyHandler>();
    }

    private void Start()
    {
        window = Instantiate(windowPrefab, Run.Instance.ScreenCanvas.transform);

        moneyHandler.onUsedMoney.Subscribe(UpdateMoney);
        moneyHandler.onReceivedMoney.Subscribe(UpdateMoney);

        UpdateMoney(moneyHandler.Money);
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