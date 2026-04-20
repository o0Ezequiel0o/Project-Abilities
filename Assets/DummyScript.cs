using System.Collections.Generic;
using UnityEngine;

public class DummyScript : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed;

    [Header("Physics")]
    [SerializeField] private float colliderRadius;
    [SerializeField] private LayerMask collisionLayer;

    [Header("Test")]
    [SerializeField] private Vector2 testVelocity = Vector2.zero;
    [SerializeField] private GameObject prefab;

    private readonly List<RaycastHit2D> hits = new List<RaycastHit2D>();

    private const float DISTANCE_MULT = 10f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            Instantiate(prefab, transform);
        }
    }

    private void FixedUpdate()
    {
        //Vector2 force = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        //hits.Clear();

        //Vector2 forceDirection = force.normalized;
        //Vector2 circleOutterPoint = (Vector2)transform.position + ((colliderRadius * 0.99f) * forceDirection);
        //Vector3 futurePosition = transform.position + ((Vector3)force * Time.fixedDeltaTime);

        //float distance = Vector2.Distance(transform.position, futurePosition);
        //float distanceFromCenter = Vector2.Distance(transform.position, futurePosition);

        //ContactFilter2D contactFilter = new ContactFilter2D() { layerMask = collisionLayer, };
        //Physics2D.CircleCast(transform.position, colliderRadius, force.normalized, contactFilter, hits, distanceFromCenter);

        //for (int i = 0; i < hits.Count; i++)
        //{
        //    if (hits[i].transform.position == transform.position) continue;

        //    Vector2 rotatedVector = Vector2.Perpendicular(hits[i].normal);
        //    float forceMult = 1f - Vector2.Dot(-forceDirection, hits[i].normal);

        //    float direction = Mathf.Sign(Vector2.Dot(forceDirection, rotatedVector));
        //    Vector2 slideDirection = rotatedVector * direction;

        //    float distanceFromCollision = (hits[i].point - circleOutterPoint).sqrMagnitude;
        //    float slideDistance = distance - distanceFromCollision;

        //    Debug.Log($"move distance: {distance} | distance from collision: {distanceFromCollision} | slide distance: {slideDistance} | force multiplier {forceMult}");

        //    if (slideDistance < 0f) Debug.LogWarning("bro u fucked up");

        //    futurePosition = (hits[i].point + (-slideDirection * (slideDistance * forceMult))) + (colliderRadius * hits[i].normal);

        //    break;
        //}

        //transform.position = futurePosition;
    }

    private void OnDrawGizmos()
    {
        hits.Clear();

        float distance = Vector2.Distance(transform.position, transform.position + (Vector3)testVelocity) * Time.deltaTime;

        ContactFilter2D contactFilter = new ContactFilter2D() { layerMask = collisionLayer, useLayerMask = true };
        Physics2D.CircleCast(transform.position, colliderRadius, testVelocity.normalized, contactFilter, hits, DISTANCE_MULT);

        for (int i = 0; i < hits.Count; i++)
        {
            if (hits[i].transform.position == transform.position) continue;

            Vector2 rotatedVector = Vector2.Perpendicular(hits[i].normal);
            float forceMult = 1f - Vector2.Dot(-testVelocity, hits[i].normal);

            float direction = Mathf.Sign(Vector2.Dot(testVelocity, rotatedVector));
            Vector2 slideDirection = rotatedVector * direction;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, hits[i].point);

            Gizmos.color = Color.orange;
            Gizmos.DrawLine(hits[i].point, hits[i].point + (hits[i].normal * DISTANCE_MULT));

            Gizmos.color = Color.green;
            Gizmos.DrawLine(hits[i].point, hits[i].point + slideDirection * DISTANCE_MULT);

            break;
        }
    }
}