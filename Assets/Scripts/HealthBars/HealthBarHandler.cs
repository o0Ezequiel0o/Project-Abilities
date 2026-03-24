using UnityEngine;

public class HealthBarHandler : MonoBehaviour
{
    [Header("Dependency")]
    [SerializeField] private Damageable damageable;

    [Header("Settings")]
    [SerializeField] private GameObject healthBarPrefab;
    [SerializeField] private Transform follow;
    [Space]
    [SerializeField] private Vector2 size = Vector2.one;
    [SerializeField] private float duration = 5f;

    private StatusBar healthBar;
    private float durationTimer;

    void Reset()
    {
        damageable = GetComponentInChildren<Damageable>();
        follow = GetComponentInChildren<Transform>();
    }

    void Start()
    {
        if (damageable == null) return;

        SpawnHealthBar();
        SubscribeToEvents();
        ResetHealthBarDurationTimer();
    }

    void LateUpdate()
    {
        if (healthBar == null) return;

        HealthBarFollowTargetPosition();
        UpdateHealthBarDurationTimer();
    }

    void OnDestroy()
    {
        if (healthBar == null) return;

        Destroy(healthBar.gameObject);
    }

    void SpawnHealthBar()
    {
        Canvas worldCanvas = GameInstance.WorldCanvas;

        if (worldCanvas == null) return;

        healthBar = Instantiate(healthBarPrefab, worldCanvas.transform).GetComponent<StatusBar>();

        if (healthBar.TryGetComponent(out RectTransform rectTransform))
        {
            rectTransform.sizeDelta *= size;
        }

        HideHealthBar();
    }

    void SubscribeToEvents()
    {
        damageable.onDamageTaken += OnDamageTaken;
        damageable.onHealthReceived += OnHealthReceived;
    }

    void OnDamageTaken(Damageable.DamageEvent damageEvent)
    {
        if (healthBar == null) return;

        if (!healthBar.gameObject.activeSelf)
        {
            ShowHealthBar();
        }

        UpdateHealthBarValues(damageable.Health, damageable.MaxHealth.Value);
        ResetHealthBarDurationTimer();
    }

    void OnHealthReceived(Damageable.HealEvent healingEvent)
    {
        if (healthBar == null) return;

        UpdateHealthBarValues(damageable.Health, damageable.MaxHealth.Value);
    }

    void UpdateHealthBarValues(float current, float max)
    {
        healthBar.UpdateBar(current, max);
    }

    void HealthBarFollowTargetPosition()
    {
        healthBar.transform.position = follow.position;
    }

    void UpdateHealthBarDurationTimer()
    {
        durationTimer += Time.deltaTime;

        if (durationTimer >= duration)
        {
            HideHealthBar();
        }
    }

    void ResetHealthBarDurationTimer()
    {
        durationTimer = 0f;
    }

    void ShowHealthBar()
    {
        healthBar.gameObject.SetActive(true);
        ResetHealthBarDurationTimer();
    }

    void HideHealthBar()
    {
        healthBar.gameObject.SetActive(false);
        ResetHealthBarDurationTimer();
    }
}