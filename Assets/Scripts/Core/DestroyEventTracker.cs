using UnityEngine;
using System;

public class DestroyEventTracker : MonoBehaviour
{
    public Action<GameObject> onDestroy;

    private void OnDestroy()
    {
        onDestroy?.Invoke(gameObject);
    }
}