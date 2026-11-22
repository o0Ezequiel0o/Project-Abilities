using UnityEngine;
using Zeke.UI;
using TMPro;

public class MoneyScreenLoader : MonoBehaviour
{
    [SerializeField] private UIWindow window;

    private MoneyHandler moneyHandler;

    private void Awake()
    {
        Run.Instance.onPlayerAdded += OnPlayerAdded;
    }

    private void OnPlayerAdded(Player newPlayer)
    {
        Run.Instance.onPlayerAdded -= OnPlayerAdded;

        bool playerHasRequiredComponents = newPlayer.TryGetComponent(out moneyHandler);

        if (!playerHasRequiredComponents) return;

        moneyHandler.onUsedMoney.Subscribe(UpdateMoney);
        moneyHandler.onReceivedMoney.Subscribe(UpdateMoney);

        UpdateMoney(moneyHandler.Money);
    }

    private void UpdateMoney(int money)
    {
        window.TryGetElement<TextMeshProUGUI>("Money").text = $"$ {moneyHandler.Money}";
    }
}