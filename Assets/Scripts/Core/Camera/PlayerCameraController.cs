using UnityEngine;

[RequireComponent (typeof(Camera))]
public class PlayerCameraController : MonoBehaviour
{
    [Header("Panning")]
    [SerializeField] private Vector2 threshold;

    [Header("Zooming")]
    [SerializeField] private float minZoom = 0.5f;
    [SerializeField] private float maxZoom = 2f;

    private Transform target = null;

    private bool HasTarget => target != null && target.gameObject.activeInHierarchy;

    public float Zoom
    {
        get
        {
            return zoom;
        }
        set
        {
            zoom = Mathf.Clamp(value, minZoom, maxZoom);
            camera.orthographicSize = defaultSize * zoom;
        }
    }

    private float zoom = 1f;

    private float defaultSize;

    private Camera camera;

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    private void Awake()
    {
        camera = GetComponent<Camera>();
        defaultSize = camera.orthographicSize;
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