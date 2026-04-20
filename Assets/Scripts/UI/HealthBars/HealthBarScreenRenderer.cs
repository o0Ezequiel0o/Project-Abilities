using UnityEngine;
using Zeke.UI;

[RequireComponent(typeof(Damageable))]
public class HealthBarScreenRenderer : MonoBehaviour
{
    [Header("Dependency")]
    [SerializeField] private Damageable damageable;

    [Header("Display")]
    [SerializeField] private UIWindow windowPrefab;

    private UIWindow window;
    private ScreenHealthBar healthBar;

    private void Reset()
    {
        damageable = GetComponentInChildren<Damageable>();
    }

    private void Awake()
    {
        window = Instantiate(windowPrefab, GameInstance.ScreenCanvas.transform);
        healthBar = window.TryGetElement<ScreenHealthBar>("Health Bar");

        damageable.onAnyHealthUpdate += UpdateBar;
    }

    private void Start()
    {
        UpdateBar();
    }

    private void OnDestroy()
    {
        if (window == null) return;
        Destroy(window.gameObject);
    }

    private void UpdateBar()
    {
        float healthRatio = damageable.Health / damageable.MaxHealth.Value;
        float shieldRatio = damageable.Shield / damageable.MaxShield.Value;

        healthBar.UpdateBar(healthRatio, shieldRatio, damageable.CombinedHealth);
    }
}