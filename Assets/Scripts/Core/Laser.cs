using System.Collections.Generic;
using UnityEngine;

public abstract class Laser : MonoBehaviour
{
    [Header("Visual")]
    [SerializeField] private GameObject impactPrefab;

    [Header("Settings")]
    [SerializeField] protected LayerMask hitLayers;
    [SerializeField] protected LayerMask blockLayers;

    [Header("Stats")]
    [SerializeField] protected float armorPenetration;
    [SerializeField] protected float procCoefficient;

    protected float damage = 0f;
    protected int maxPierce = 0;

    protected GameObject source;
    private GameObject impactInstance;

    private float damageCooldown;
    private float damageTimer = 0f;

    private readonly List<RaycastHit2D> hits = new List<RaycastHit2D>();

    public void SetLaserValues(GameObject source, float damage, int maxPierce, float damageCooldown)
    {
        this.source = source;
        this.damage = damage;

        this.maxPierce = maxPierce;
        this.damageCooldown = damageCooldown;
    }

    public void UpdateLaser(Vector3 position, Quaternion rotation, Vector3 direction, float radius, float maxRange)
    {
        if (source == null) throw new System.Exception($"Laser source is null, perhaps you forgot to call {nameof(SetLaserValues)}?");

        damageTimer += Time.deltaTime;

        float laserDistance = UpdateCollision(position, rotation, direction, radius, maxRange);
        UpdateLaserRendering(position, rotation, direction, radius, laserDistance);

        if (damageTimer > damageCooldown) damageTimer = 0f;
    }

    protected bool InLayerMask(GameObject hit, LayerMask layerMask)
    {
        return (layerMask & 1 << hit.layer) != 0;
    }

    protected virtual void OnCollision(GameObject hit) { }

    private float UpdateCollision(Vector3 position, Quaternion rotation, Vector3 direction, float radius, float maxRange)
    {
        hits.Clear();

        int targetsPierced = 0;

        ContactFilter2D contactFilter = new ContactFilter2D() { layerMask = hitLayers | blockLayers, useLayerMask = true };
        Physics2D.CircleCast(position, radius, direction, contactFilter, hits, maxRange);

        for (int i = 0; i < hits.Count; i++)
        {
            GameObject hitObject = hits[i].collider.gameObject;

            if (hitObject == source) continue;

            if (InLayerMask(hitObject, blockLayers))
            {
                OnCollision(hitObject);
                return Vector2.Distance(position, hits[i].point);
            }

            if (InLayerMask(hitObject, hitLayers))
            {
                if (damageTimer > damageCooldown)
                {
                    OnCollision(hitObject);
                }

                targetsPierced += 1;

                if (targetsPierced > maxPierce)
                {
                    return Vector2.Distance(position, hits[i].point);
                }
            }
        }

        return maxRange;
    }

    private void UpdateLaserRendering(Vector3 position, Quaternion rotation, Vector3 direction, float radius, float distance)
    {
        transform.localScale = new Vector3(radius * 2f, distance, transform.localScale.z);
        transform.SetPositionAndRotation(position + direction * (distance * 0.5f), rotation);

        impactInstance.transform.position = position + (distance * direction);
    }

    private void Awake()
    {
        transform.localScale = new Vector3(transform.localScale.x, 1f, transform.localScale.z);
        impactInstance = Instantiate(impactPrefab, transform.position, Quaternion.identity);
    }

    private void OnEnable()
    {
        impactInstance.SetActive(true);
    }

    private void OnDisable()
    {
        if (impactInstance == null) return;
        impactInstance.SetActive(false);
    }

    private void OnDestroy()
    {
        if (impactInstance == null) return;
        Destroy(impactInstance);
    }
}