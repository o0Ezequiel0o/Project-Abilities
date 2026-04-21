using UnityEngine;
using UnityEngine.AI;

public class Seeker : MonoBehaviour
{
    [SerializeField] private float minDistance;

    private int currentIndex = 0;
    private NavMeshPath path;

    public Vector2 Direction { get; private set; }

    private bool pathUpdateDone = false;

    private void Awake()
    {
        path = new NavMeshPath();
    }

    public void UpdatePath(Vector3 target)
    {
        NavMesh.CalculatePath(transform.position, target, NavMesh.AllAreas, path);
        currentIndex = 0;
        UpdateDirection();
        UpdatePathState();
    }

    private void UpdateDirection()
    {
        Direction = (path.corners[currentIndex] - transform.position).normalized;
    }

    private void UpdatePathState()
    {
        if (currentIndex >= path.corners.Length)
        {
            return;
        }

        Vector3 corner = path.corners[currentIndex];

        if (Vector2.Distance(corner, transform.position) < minDistance)
        {
            currentIndex += 1;

            if (currentIndex < path.corners.Length)
            {
                UpdateDirection();
            }
        }

        pathUpdateDone = true;
    }
}