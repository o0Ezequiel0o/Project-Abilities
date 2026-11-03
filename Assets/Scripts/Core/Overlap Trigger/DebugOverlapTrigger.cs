using UnityEngine;

public class DebugOverlapTrigger : MonoBehaviour
{
    [SerializeField] private CircleOverlapTrigger circleOverlapTrigger;
    [SerializeField] private ConeOverlapTrigger coneOverlapTrigger;

    private void OnDrawGizmos()
    {
        circleOverlapTrigger.DrawGizmos(transform.position, transform.up);
        coneOverlapTrigger.DrawGizmos(transform.position, transform.up);
    }
}