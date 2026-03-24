using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Collider2D))]
public class Sensor : MonoBehaviour
{
    [SerializeField] private LayerMask hitLayers;

    public List<GameObject> Overlapping => overlappingGameObjectsList;

    public Action<GameObject> onTriggerEnter;
    public Action<GameObject> onTriggerExit;

    private HashSet<GameObject> overlappingGameObjects = new HashSet<GameObject>();
    private List<GameObject> overlappingGameObjectsList = new List<GameObject>();

    private void OnValidate()
    {
        GetComponent<Collider2D>().includeLayers = hitLayers;
    }

    public bool IsOverlapping(GameObject other)
    {
        return overlappingGameObjects.Contains(other);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        overlappingGameObjects.Add(collision.gameObject);
        overlappingGameObjectsList.Add(collision.gameObject);

        onTriggerEnter?.Invoke(gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        overlappingGameObjects.Remove(collision.gameObject);
        overlappingGameObjectsList.Remove(collision.gameObject);

        onTriggerExit?.Invoke(gameObject);
    }
}