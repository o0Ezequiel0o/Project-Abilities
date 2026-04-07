using System.Collections.Generic;
using UnityEngine;
using System;
using Zeke.PoolableGameObjects;

public class Spinner<T> where T : Component
{
    public float RotationSpeed { get; set; } = 0f;
    public Transform Pivot { get; private set; }

    public Action<List<T>> onInitialization;
    private readonly GameObjectPool<T> objectPool = new GameObjectPool<T>();

    public void InitializeSpinner(Transform pivotParent, T prefab, float distance, float speed, int amount)
    {
        if (Pivot == null) CreatePivot(pivotParent);
        else Pivot.parent = pivotParent;

        List<T> spawnedObjects = new List<T>();

        float anglePerObject = 360f / amount;
        float currentAngle = 0f;

        RotationSpeed = speed;

        for (int i = 0; i < amount; i++)
        {
            Vector2 direction = Quaternion.Euler(0f, 0f, currentAngle) * Vector3.up;

            T objectInstance = objectPool.Get(prefab, Pivot);
            objectInstance.gameObject.SetActive(true);
            spawnedObjects.Add(objectInstance);

            objectInstance.transform.localPosition = distance * direction;

            currentAngle += anglePerObject;
        }

        onInitialization?.Invoke(spawnedObjects);
        OnInitialization(spawnedObjects);
    }

    public void UpdateDistanceFromPivot(float distance)
    {
        if (Pivot != null)
        {
            foreach (Transform children in Pivot)
            {
                Vector3 direction = (children.position - Pivot.position).normalized;
                children.localPosition = distance * direction;
            }
        }
    }

    public void RemoveFromPivot(Transform transform)
    {
        Transform currentTransform = transform;

        while (currentTransform != null)
        {
            if (currentTransform.parent == Pivot)
            {
                currentTransform.parent = null;
            }

            currentTransform = currentTransform.parent;
        }
    }

    public void Update()
    {
        if (Pivot == null) return;

        float rotationStep = RotationSpeed * Time.deltaTime;
        Pivot.transform.eulerAngles += new Vector3(0f, 0f, rotationStep);
    }

    public void Destroy()
    {
        if (Pivot == null) return;

        objectPool.Clear();
        GameObject.Destroy(Pivot.gameObject);
    }

    public void DisablePivotChildren()
    {
        if (Pivot == null) return;

        foreach (Transform child in Pivot)
        {
            child.gameObject.SetActive(false);
        }
    }

    protected virtual void OnInitialization(List<T> spawnedObjects) { }

    private void CreatePivot(Transform parent)
    {
        Pivot = new GameObject("Pivot").transform;
        Pivot.parent = parent;

        Pivot.localPosition = Vector3.zero;
    }
}