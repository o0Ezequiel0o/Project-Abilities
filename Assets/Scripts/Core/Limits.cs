using UnityEngine;
using System;

[Serializable]
public class Limits
{
    [SerializeField] private float min = float.NegativeInfinity;
    [SerializeField] private float max = float.PositiveInfinity;

    public float Min => min;
    public float Max => max;

    public static Limits Zero => new Limits(0f, 0f);

    public Limits() { }
    
    public Limits(float min, float max)
    {
        this.min = min;
        this.max = max;
    }

    public static float Clamp(float value, Limits limits)
    {
        return Mathf.Clamp(value, limits.Min, limits.Max);
    }

    public float Clamp(float value)
    {
        return Mathf.Clamp(value, Min, Max);
    }
}