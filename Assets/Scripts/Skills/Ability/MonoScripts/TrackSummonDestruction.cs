using UnityEngine;
using System;

public class TrackSummonDestruction : MonoBehaviour
{
    public Action<GameObject> onDestroy;

    private void OnDestroy()
    {
        onDestroy?.Invoke(gameObject);
    }
}