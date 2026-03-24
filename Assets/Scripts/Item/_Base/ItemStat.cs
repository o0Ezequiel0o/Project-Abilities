using System;
using UnityEngine;

[Serializable]
public struct ItemStat
{
    [SerializeField] private ScalingMode scalingMode;
    
    [field: SerializeField] private float BaseValue;
    [field: SerializeField] private float Increase;

    public readonly float CalculateValue(int stacks)
    {
        if (stacks == 0) return 0f;

        switch (scalingMode)
        {
            case ScalingMode.Linear:
                return BaseValue + (Increase * stacks);

            case ScalingMode.Exponential:
                return BaseValue + Mathf.Pow(Increase, stacks);

            case ScalingMode.Hyperbolic:
                throw new NotImplementedException();

            case ScalingMode.Reciprocal:
                throw new NotImplementedException();
        }

        throw new ArgumentOutOfRangeException();
    }
    
    private enum ScalingMode
    {
        Linear,
        Exponential,
        Hyperbolic,
        Reciprocal
    }
}