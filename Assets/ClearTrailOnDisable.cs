using UnityEngine;

public class ClearTrailOnDisable : MonoBehaviour
{
    [SerializeField] private TrailRenderer trailRenderer;

    private void OnDisable()
    {
        trailRenderer.Clear();
    }
}