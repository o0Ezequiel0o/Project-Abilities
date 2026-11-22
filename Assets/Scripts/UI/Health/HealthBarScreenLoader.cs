using UnityEngine;
using Zeke.UI;

public class HealthBarScreenLoader : MonoBehaviour
{
    [Header("Display")]
    [SerializeField] private UIWindow window;

    private ScreenHealthBar healthBar;
    private Damageable damageable;

    private void Awake()
    {
        Run.Instance.onPlayerAdded += OnPlayerAdded;
        healthBar = window.TryGetElement<ScreenHealthBar>("Health Bar");
    }

    private void OnPlayerAdded(Player newPlayer)
    {
        Run.Instance.onPlayerAdded -= OnPlayerAdded;

        bool playerHasRequiredComponents = newPlayer.TryGetComponent(out damageable);

        if (!playerHasRequiredComponents) return;

        damageable.onAnyHealthUpdate += UpdateBar;

        UpdateBar();
    }

    private void UpdateBar()
    {
        healthBar.UpdateBar (
            damageable.Health, damageable.MaxHealth.Value,
            damageable.Shield, damageable.MaxShield.Value,
            damageable.CombinedHealth
            );
    }
}