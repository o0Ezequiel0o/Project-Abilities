using UnityEngine;
using System;

public class GraphicVisibilityTracker : MonoBehaviour
{
    public Action onEnable;
    public Action onDisable;

    private void OnEnable()
    {
        onEnable?.Invoke();
    }

    private void OnDisable()
    {
        onDisable?.Invoke();
    }
}