using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    [SerializeField] private Vector2 threshold;

    private Transform target = null;

    private bool HasTarget => target != null && target.gameObject.activeInHierarchy;

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    private void Update()
    {
        if (Run.Instance.IsPaused) return;

        if (HasTarget && Run.Instance.MainCamera != null)
        {
            Follow();
        }
    }

    private void Follow()
    {
        Vector3 targetPos = (target.position + Run.Instance.MouseWorldPosition) / 2;

        targetPos.x = Mathf.Clamp(targetPos.x, -threshold.x + target.position.x, threshold.x + target.position.x);
        targetPos.y = Mathf.Clamp(targetPos.y, -threshold.y + target.position.y, threshold.y + target.position.y);
        targetPos.z = Run.Instance.MainCamera.transform.position.z;

        transform.position = targetPos;
    }
}