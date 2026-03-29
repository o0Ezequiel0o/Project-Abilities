using UnityEngine;

public class Physics : MonoBehaviour
{
    [Header("Dependency")]
    [SerializeField] protected Rigidbody2D rigidBody;

    [Header("Settings")]
    [SerializeField] protected float forceReceivedMultiplier = 1f;

    [Space]

    [SerializeField] protected float linearDamping = 0.15f;

    public float Rotation => rigidBody.rotation;

    public Vector2 Velocity => rigidBody.linearVelocity;
    public Vector2 Forces => forces;

    protected Vector2 forces;

    protected float forceStopThreshold = 0.01f;

    public void AddForce(Vector2 force)
    {
        forces += force * forceReceivedMultiplier / rigidBody.mass;
    }

    public void AddForce(float force, Vector2 direction)
    {
        AddForce(force * direction);
    }

    public void AddForce(float force, Vector2 direction, Vector2 normal)
    {
        float dotProduct = Vector2.Dot(direction, -normal);
        float forceMultiplier = Mathf.Max(0f, dotProduct - 0.5f) * 2f;

        Debug.Log(forceMultiplier);

        AddForce(force * forceMultiplier, direction);
    }

    public void Rotate(float angle, float step)
    {
        rigidBody.rotation = Mathf.MoveTowardsAngle(rigidBody.rotation, angle, step);
    }

    protected virtual void UpdateForces()
    {
        forces /= 1 + linearDamping;

        if (Mathf.Abs(forces.x) <= forceStopThreshold && Mathf.Abs(forces.y) <= forceStopThreshold)
        {
            forces = Vector2.zero;
        }
    }

    protected virtual void UpdateVelocity()
    {
        rigidBody.linearVelocity = forces;
    }

    private void Reset()
    {
        rigidBody = GetComponentInChildren<Rigidbody2D>();
    }

    private void OnValidate()
    {
        ClampValues();
    }

    private void FixedUpdate()
    {
        UpdateVelocity();
        UpdateForces();
    }

    private void ClampValues()
    {
        linearDamping = Mathf.Max(0, linearDamping);
    }

    protected virtual void OnDisable()
    {
        forces = Vector2.zero;
        rigidBody.linearVelocity = Vector2.zero;
    }
}