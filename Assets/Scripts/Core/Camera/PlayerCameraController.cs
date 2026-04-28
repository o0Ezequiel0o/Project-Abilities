using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    [SerializeField] private Vector2 threshold;

    private Transform target = null;

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    private void Update()
    {
        if (target != null && GameInstance.MainCamera != null)
        {
            Follow();
        }
    }

    private void Follow()
    {
        Vector3 targetPos = (target.position + GameInstance.MouseWorldPosition) / 2;

        targetPos.x = Mathf.Clamp(targetPos.x, -threshold.x + target.position.x, threshold.x + target.position.x);
        targetPos.y = Mathf.Clamp(targetPos.y, -threshold.y + target.position.y, threshold.y + target.position.y);
        targetPos.z = GameInstance.MainCamera.transform.position.z;

        transform.position = targetPos;
    }
}