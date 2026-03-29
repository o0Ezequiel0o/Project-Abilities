using System.Collections.Generic;
using UnityEngine;
using Zeke.PoolableGameObjects;
using Zeke.TeamSystem;

public class FireBarrier : MonoBehaviour, IPoolableGameObjectConfirmator
{
    [Header("Settings")]
    [SerializeField] private LayerMask hitLayer;
    [Space]
    [SerializeField] private StatusEffectData statusEffectToApply;
    [SerializeField] private float checkInterval;
    [Space]
    [SerializeField] private DespawnAction despawnAction;

    public bool CanGetPoolable => true;

    private GameObject source;
    private float duration;

    private float rotation;
    private Vector2 size;

    private float checkCollidersTimer = 0f;
    private float despawnTimer = 0f;

    private readonly HashSet<GameObject> ignoreGameObjects = new HashSet<GameObject>();
    private readonly List<Collider2D> hits = new List<Collider2D>();

    public void OnRetrievedFromPool()
    { 
        ignoreGameObjects.Clear();
        despawnTimer = 0f;
    }

    public void SetValues(Vector3 position, GameObject source, Vector2 direction, Vector2 size, float duration)
    {
        this.size = size;
        this.source = source;
        this.duration = duration;
        rotation = GetRotation(direction);

        transform.SetPositionAndRotation(position, Quaternion.Euler(0, 0, rotation));
    }

    void Update()
    {
        checkCollidersTimer += Time.deltaTime;
        despawnTimer += Time.deltaTime;

        if (checkCollidersTimer >= checkInterval)
        {
            CheckColliders();
            checkCollidersTimer = 0f;
        }

        if (despawnTimer >= duration)
        {
            Despawn();
        }
    }

    void CheckColliders()
    {
        hits.Clear();

        ContactFilter2D contactFilter = new ContactFilter2D() { layerMask = hitLayer };
        Physics2D.OverlapBox(transform.position, size, rotation, contactFilter, hits);

        for (int i = 0; i < hits.Count; i++)
        {
            OnHit(hits[i].gameObject);
        }
    }

    void OnHit(GameObject receiver)
    {
        if (ignoreGameObjects.Contains(receiver)) return;

        if (TeamManager.IsAlly(source, receiver)) return;

        if (receiver.TryGetComponent(out StatusEffectHandler statusEffectHandler))
        {
            statusEffectHandler.ApplyEffect(statusEffectToApply, source);
            ignoreGameObjects.Add(receiver);
        }
    }

    float GetRotation(Vector2 direction)
    {
        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
    }

    void Despawn()
    {
        switch (despawnAction)
        {
            case DespawnAction.Destroy:
                Destroy(gameObject);
                break;

            case DespawnAction.Disable:
                gameObject.SetActive(false);
                break;
        }
    }
}