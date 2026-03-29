using UnityEngine;
using Zeke.PoolableGameObjects;

public class AttackIndicator : MonoBehaviour, IPoolableGameObjectConfirmator
{
    [SerializeField] private StatusBar indicatorBar;
    public DespawnAction despawnAction;

    private bool animationStarted = false;

    private float duration = 0f;
    private float animationTimer = 0f;

    private Transform target;
    private Vector2 size;

    private Vector3 distanceFromCenterCache;
    private bool offsetCenterBySize;

    public bool CanGetPoolable => true;

    public void StartAnimation(Transform target, float duration, Vector2 size, bool offsetCenterBySize)
    {
        this.offsetCenterBySize = offsetCenterBySize;
        this.size = size;

        this.duration = duration;
        this.target = target;

        animationTimer = 0f;
        animationStarted = true;

        transform.localScale = size;
    }

    public void OnRetrievedFromPool()
    {
        animationTimer = 0f;
        target = null;
        animationStarted = false;
    }

    private void Update()
    {
        if (!animationStarted) return;

        animationTimer += Time.deltaTime;
        indicatorBar.UpdateBar(animationTimer, duration);

        if (animationTimer >= duration)
        {
            Despawn();
        }
    }

    private void LateUpdate()
    {
        if (offsetCenterBySize)
        {
            distanceFromCenterCache = 0.5f * size.y * target.transform.up;
        }
        else
        {
            distanceFromCenterCache = Vector3.zero;
        }

        FollowTarget();
    }

    private void FollowTarget()
    {
        transform.SetPositionAndRotation(target.position + distanceFromCenterCache, target.rotation);
    }

    private void Despawn()
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