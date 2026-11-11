using UnityEngine;

public class HealthBarRenderer : MonoBehaviour
{
    [Header("Dependency")]
    [SerializeField] private Damageable damageable;

    [Header("Settings")]
    [SerializeField] private SpriteHealthBar healthBarPrefab;
    [SerializeField] private Transform center;
    [Space]
    [SerializeField] private Vector3 offset = Vector3.zero;
    [SerializeField] private Vector2 size = Vector2.one;
    [SerializeField] private float duration = 5f;

    private SpriteHealthBar healthBar;

    private float durationTimer = 0f;
    private int healthBarRequestID = 0;
    private bool pendingHealthBar = false;

    private void Reset()
    {
        damageable = GetComponentInChildren<Damageable>();
        center = GetComponentInChildren<Transform>();
    }

    private void Start()
    {
        if (damageable == null) return;
        SubscribeToEvents();
    }

    private void LateUpdate()
    {
        if (healthBar == null) return;
        HealthBarFollowTargetPosition();
        UpdateHealthBarDurationTimer();
    }

    private void OnDestroy()
    {
        if (HealthBarManager.IsNull) return;

        if (healthBar != null)
        {
            HealthBarManager.Instance.StaggeredDisableAndReturn(healthBarPrefab, healthBar);
            healthBar = null;
        }

        if (pendingHealthBar)
        {
            HealthBarManager.Instance.CancelGet(healthBarRequestID);
            pendingHealthBar = false;
        }
    }

    private void OnDisable()
    {
        if (HealthBarManager.IsNull) return;

        if (healthBar != null)
        {
            HealthBarManager.Instance.StaggeredDisableAndReturn(healthBarPrefab, healthBar);
            healthBar = null;
        }

        if (pendingHealthBar)
        {
            HealthBarManager.Instance.CancelGet(healthBarRequestID);
            pendingHealthBar = false;
        }
    }

    private void SubscribeToEvents()
    {
        damageable.onTakenDamage.Subscribe(OnDamageTaken);
        damageable.onAnyHealthUpdate += UpdateHealthBar;
    }

    private void OnDamageTaken(Damageable.DamageEvent damageEvent)
    {
        if (healthBar == null && !pendingHealthBar)
        {
            RequestHealthBar();
        }

        ResetHealthBarDurationTimer();
    }

    private void RequestHealthBar()
    {
        healthBarRequestID = HealthBarManager.Instance.StaggeredGet(healthBarPrefab, OnHealthBarReceived);
        pendingHealthBar = true;
    }

    private void OnHealthBarReceived(SpriteHealthBar instance)
    {
        healthBar = instance;

        SetUpHealthBar();

        HealthBarFollowTargetPosition();
        healthBar.gameObject.SetActive(true);

        pendingHealthBar = false;
    }

    private void SetUpHealthBar()
    {
        healthBar.transform.localScale *= size;
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        if (healthBar == null) return;

        healthBar.UpdateBar (
            damageable.Health, damageable.MaxHealth.Value,
            damageable.Shield, damageable.MaxShield.Value
            );
    }

    private void HealthBarFollowTargetPosition()
    {
        healthBar.transform.position = transform.position + offset;
    }

    private void UpdateHealthBarDurationTimer()
    {
        durationTimer += Time.deltaTime;

        if (durationTimer >= duration)
        {
            HealthBarManager.Instance.StaggeredDisableAndReturn(healthBarPrefab, healthBar);
            healthBar = null;
        }
    }

    private void ResetHealthBarDurationTimer()
    {
        durationTimer = 0f;
    }
}