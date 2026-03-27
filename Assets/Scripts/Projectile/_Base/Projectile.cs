using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

public class Projectile : MonoBehaviour, IPoolableGameObjectConfirmator
{
    [Header("Collision")]
    [SerializeField] private float hitRadius;
    [SerializeField] private float tipDistance;

    [Space]

    [SerializeField] private Color gizmosColor = Color.white;

    [Space]

    [SerializeField] protected LayerMask hitLayer;
    [SerializeField] protected LayerMask blockLayer;

    [Header("Despawn")]

    [SerializeField] private DespawnAction despawnAction;

    public virtual bool CanGetPoolable => true;

    public float Speed { get; set; }
    public float MaxRange { get; set; }

    public float Radius => hitRadius;

    public Vector3 CurrentPosition => currentPosition;
    public Vector3 PreviousPosition => lastPosition;

    public virtual Vector3 Direction { get; protected set; }

    public Action<Projectile> onDespawn;

    protected readonly QuickLookupList<GameObject, IProjectileTrigger> objectsNotExited = new QuickLookupList<GameObject, IProjectileTrigger>();
    protected readonly HashSet<GameObject> objectsHit = new HashSet<GameObject>();

    protected Vector3 TipPosition => transform.position + TipDistance;
    protected Vector3 TipDistance => transform.up * tipDistance;
    protected Vector3 Center => transform.position;

    protected Vector3 lastPosition;
    protected Vector3 currentPosition;

    protected float distanceTravelled = 0f;

    private bool despawnCalled = false;
    private bool despawnCanceled = false;

    private bool stopLoopingHits = false;
    private bool despawnThisFrame = false;

    private readonly List<RaycastHit2D> hits = new List<RaycastHit2D>();

    private LayerMask CollideLayers
    {
        get
        {
            return hitLayer | blockLayer;
        }
    }

    public virtual void OnPoolableGet()
    {
        Speed = 0f;
        MaxRange = 0f;

        distanceTravelled = 0f;

        stopLoopingHits = false;
        despawnThisFrame = false;
        despawnCanceled = false;
        despawnCalled = false;

        objectsHit.Clear();
        objectsNotExited.Clear();
    }

    public void Launch(Vector3 position, float speed, Vector2 direction, float maxRange)
    {
        Speed = speed;
        MaxRange = maxRange;

        Direction = direction;

        ResetPositionAndRotation(position, GetRotation(direction));
        OnLaunch(position, speed, direction, maxRange);
    }

    public void Teleport(Vector3 newPosition)
    {
        transform.position = newPosition;

        lastPosition = TipPosition;
        currentPosition = TipPosition;
    }

    public void CancelDespawn()
    {
        despawnCanceled = true;
    }

    protected virtual void OnDespawn() { }

    protected virtual void OnMaxDistanceReached()
    {
        Despawn();
    }

    protected virtual void OnCollision(RaycastHit2D hit) { }

    protected virtual void OnLaunch(Vector3 startPosition, float speed, Vector2 direction, float maxRange) { }

    protected virtual void OnDestroy()
    {
        if (despawnCalled) return;
        TriggerAllProjectilesExit();
        onDespawn?.Invoke(this);
    }

    protected bool InLayerMask(GameObject hit, LayerMask layerMask)
    {
        return (layerMask & 1 << hit.layer) != 0;
    }

    protected void TeleportToHitPoint(Vector3 hitPoint)
    {
        SetPosition(hitPoint - (hitRadius + tipDistance) * Direction);
    }

    protected void Move(Vector3 direction, float step)
    {
        transform.position += step * direction;
    }

    protected virtual void UpdateMovement()
    {
        lastPosition = TipPosition;

        Move(Direction, Speed * Time.deltaTime);

        currentPosition = TipPosition;
        distanceTravelled += Vector3.Distance(lastPosition, currentPosition);

        if (distanceTravelled > MaxRange)
        {
            Move(-Direction, (distanceTravelled - MaxRange));
            currentPosition = TipPosition;
            OnMaxDistanceReached();
        }
    }

    protected void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    protected void Despawn()
    {
        StopLoopingHits();
        despawnCanceled = false;
        despawnThisFrame = true;
    }

    private void ProccessDespawn()
    {
        despawnThisFrame = false;

        if (despawnCanceled)
        {
            despawnCanceled = false;
            return;
        }

        TriggerAllProjectilesExit();

        switch (despawnAction)
        {
            case DespawnAction.Destroy:
                Destroy(gameObject);
                break;

            case DespawnAction.Disable:
                gameObject.SetActive(false);
                break;
        }

        onDespawn?.Invoke(this);
        despawnCalled = true;
        OnDespawn();
    }

    protected virtual void Update()
    {
        UpdateMovement();
        UpdateCollision();
    }

    protected float GetRotation(Vector2 direction)
    {
        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
    }

    protected Quaternion GetRotationQuaternion(Vector2 direction)
    {
        return Quaternion.Euler(0f, 0f, GetRotation(direction));
    }

    protected void StopLoopingHits()
    {
        stopLoopingHits = true;
    }

    protected List<RaycastHit2D> GetAllHitsUnfiltered()
    {
        return hits;
    }

    private void UpdateCollision()
    {
        hits.Clear();

        Vector3 lastPositionCenter = GetCircleCastCenterPosition(lastPosition);
        Vector3 currentPositionCenter = GetCircleCastCenterPosition(currentPosition);

        ContactFilter2D contactFilter = new ContactFilter2D() { layerMask = CollideLayers, useTriggers = true };
        Physics2D.CircleCast(lastPositionCenter, hitRadius, Direction, contactFilter, hits, Vector3.Distance(lastPositionCenter, currentPositionCenter));

        CheckColliderExitedProjectile();
        CheckCollidersEnteredProjectile();

        if (despawnThisFrame) ProccessDespawn();
    }

    private void TriggerAllProjectilesExit()
    {
        for (int i = objectsNotExited.Count - 1; i >= 0; i--)
        {
            if (objectsNotExited.TryGetValueByIndex(i, out IProjectileTrigger projectileDetector))
            {
                projectileDetector?.OnProjectileExit(this);
            }
        }
    }

    private void CheckColliderExitedProjectile()
    {
        for (int i = objectsNotExited.Count - 1; i >= 0; i--)
        {
            if (!InHits(objectsNotExited[i]))
            {
                if (objectsNotExited.TryGetValueByIndex(i, out IProjectileTrigger projectileTrigger))
                {
                    projectileTrigger?.OnProjectileExit(this);
                }

                objectsNotExited.RemoveAt(i);
            }
        }
    }

    private void CheckCollidersEnteredProjectile()
    {
        stopLoopingHits = false;

        for (int i = hits.Count - 1; i >= 0; i--)
        {
            GameObject objectHit = hits[i].collider.gameObject;

            if (!objectsHit.Contains(objectHit))
            {
                objectsHit.Add(objectHit);
            }

            if (!hits[i].collider.isTrigger)
            {
                OnCollision(hits[i]);
            }

            if (stopLoopingHits)
            {
                return;
            }

            if (!objectsNotExited.Contains(objectHit))
            {
                if (objectHit.TryGetComponent(out IProjectileTrigger projectileTrigger))
                {
                    projectileTrigger.OnProjectileEnter(this);
                }

                objectsNotExited.Add(objectHit, projectileTrigger);
            }
        }
    }

    private bool InHits(GameObject gameObject)
    {
        for (int i = 0; i < hits.Count; i++)
        {
            if (hits[i].collider.gameObject == gameObject)
            {
                return true;
            }
        }

        return false;
    }

    private Vector3 GetCircleCastCenterPosition(Vector3 position)
    {
        return position - (transform.up * hitRadius);
    }

    private void ResetPositionAndRotation(Vector3 position, float rotation)
    {
        transform.SetPositionAndRotation(position, Quaternion.Euler(0, 0, rotation));
        distanceTravelled = 0f;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = gizmosColor;
        Gizmos.DrawWireSphere(GetCircleCastCenterPosition(TipPosition), hitRadius);
    }

    protected class QuickLookupList<Key, Value> : IEnumerable<Key>
    {
        private readonly List<Key> list;
        private readonly Dictionary<Key, Value> dictionary;

        public int Count => list.Count;

        public QuickLookupList()
        {
            list = new List<Key>();
            dictionary = new Dictionary<Key, Value>();
        }

        public IEnumerator<Key> GetEnumerator()
        {
            foreach (Key key in list)
            {
                yield return key;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Key this[int index]
        {
            get
            {
                return list[index];
            }
        }

        public bool Contains(Key gameObject)
        {
            return dictionary.ContainsKey(gameObject);
        }

        public bool TryGetValue(Key key, out Value value)
        {
            return dictionary.TryGetValue(key, out value);
        }

        public bool TryGetValueByIndex(int index, out Value value)
        {
            return dictionary.TryGetValue(list[index], out value);
        }

        public void Add(Key key, Value value)
        {
            if (!dictionary.ContainsKey(key))
            {
                list.Add(key);
                dictionary.Add(key, value);
            }
        }

        public void Remove(Key key)
        {
            list.Remove(key);
            dictionary.Remove(key);
        }

        public void RemoveAt(int index)
        {
            Key key = list[index];

            dictionary.Remove(key);
            list.RemoveAt(index);
        }
        
        public void Clear()
        {
            dictionary.Clear();
            list.Clear();
        }
    }
}