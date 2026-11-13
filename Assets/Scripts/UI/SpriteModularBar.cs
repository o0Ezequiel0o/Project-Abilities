using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class SpriteModularBar : MonoBehaviour
{
    [SerializeField] protected Transform root;
    [SerializeField] protected Transform barsRoot;
    [SerializeField] protected Transform contentTransform;

    [Space]

    [Tooltip("X: Left, Y: Right, Z: Top, W: Bottom")]
    [SerializeField] protected Vector4 padding = new Vector4(0.05f, 0.05f, 0.05f, 0.05f);

    public Transform Root => barsRoot;

    public float CombinedValue => combinedValue;
    public float CombinedMaxValue => combinedMaxValue;

    private float combinedValue = 0f;
    private float combinedMaxValue = 0f;

    private readonly List<BarIDPair> bars = new List<BarIDPair>();

    private Vector2 originalDelta = Vector2.zero;

    public virtual void ChangeSize(Vector2 size)
    {
        root.localScale *= size;
        UpdateContentTransformBounds();
    }

    public virtual void ResetValues()
    {
        root.localScale = originalDelta;
        UpdateContentTransformBounds();
    }

    protected virtual void Awake()
    {
        originalDelta = root.localScale;
        UpdateContentTransformBounds();
    }

    protected virtual void OnValidate()
    {
        UpdateContentTransformBounds();
    }

    private void UpdateContentTransformBounds()
    {
        if (contentTransform == null || root == null) return;

        float parentScaleX = root.localScale.x;
        float parentScaleY = root.localScale.y;

        if (Mathf.Approximately(parentScaleX, 0f)) parentScaleX = 1f;
        if (Mathf.Approximately(parentScaleY, 0f)) parentScaleY = 1f;

        float effLeft = padding.x / parentScaleX;
        float effRight = padding.y / parentScaleX;
        float effTop = padding.z / parentScaleY;
        float effBottom = padding.w / parentScaleY;

        float contentWidth = Mathf.Clamp(1.0f - (effLeft + effRight), 0f, 1.0f);
        float contentHeight = Mathf.Clamp(1.0f - (effTop + effBottom), 0f, 1.0f);

        float contentCenterX = (effLeft - effRight) * 0.5f;
        float contentCenterY = (effBottom - effTop) * 0.5f;

        contentTransform.localScale = new Vector2(contentWidth, contentHeight);
        contentTransform.localPosition = new Vector2(contentCenterX, contentCenterY);
    }

    public class BarID { }

    protected class Bar
    {
        public StatusBarValue values;
        public Transform transform;

        public Bar(StatusBarValue values, Transform transform)
        {
            this.values = values;
            this.transform = transform;
        }
    }

    protected readonly struct BarIDPair
    {
        public readonly BarID id;
        public readonly Bar bar;

        public BarIDPair(BarID id, Bar bar)
        {
            this.id = id;
            this.bar = bar;
        }
    }

    protected readonly struct StatusBarValue
    {
        public readonly float value;
        public readonly float maxValue;

        public readonly float Percentage
        {
            get
            {
                float percentage = value / maxValue;
                return IsInfiniteOrUndefined(percentage) ? 0f : percentage;
            }
        }

        public StatusBarValue(float value, float maxValue)
        {
            this.value = value;
            this.maxValue = maxValue;
        }

        private bool IsInfiniteOrUndefined(float value)
        {
            return float.IsNaN(value) || float.IsInfinity(value);
        }
    }

    public SpriteModularBar AddBar(BarID id, Color color, Sprite sprite)
    {
        return AddBar(id, color, 0f, 0f, sprite);
    }

    public SpriteModularBar AddBar(BarID id, Color color, float current, float max, Sprite sprite)
    {
        GameObject go = new GameObject("bar", typeof(SpriteRenderer));
        StatusBarValue values = new StatusBarValue(current, max);

        SpriteRenderer spriteRenderer = go.GetComponent<SpriteRenderer>();

        spriteRenderer.color = color;
        spriteRenderer.sprite = sprite;

        go.transform.SetParent(barsRoot.transform, false);

        Bar bar = new Bar(values, go.transform);
        bars.Add(new BarIDPair(id, bar));

        combinedMaxValue += max;
        combinedValue += current;

        UpdateBarRendering();

        return this;
    }

    public virtual void ResetBar(BarID id, float current, float max)
    {
        SetBar(id, current, max);
    }

    public virtual void UpdateBar(BarID id, float current, float max)
    {
        SetBar(id, current, max);
    }

    protected void SetBar(BarID id, float current, float max)
    {
        if (TryGetBar(id, out Bar bar))
        {
            StatusBarValue oldValues = bar.values;
            bar.values = new StatusBarValue(current, max);

            float currentDifference = current - oldValues.value;
            float maxDifference = max - oldValues.maxValue;

            combinedValue += currentDifference;
            combinedMaxValue += maxDifference;

            UpdateBarRendering();
        }
    }

    protected bool TryGetBar(BarID id, out Bar bar)
    {
        for (int i = 0; i < bars.Count; i++)
        {
            if (bars[i].id == id)
            {
                bar = bars[i].bar;
                return true;
            }
        }

        bar = null;
        return false;
    }

    protected void UpdateBarRendering()
    {
        if (barsRoot == null || barsRoot.gameObject == null) return;

        float xStartPosition = -0.5f;

        for (int i = 0; i < bars.Count; i++)
        {
            float width;

            if (combinedMaxValue > 0f)
            {
                width = bars[i].bar.values.Percentage * (bars[i].bar.values.maxValue / combinedMaxValue);
            }
            else
            {
                width = 0f;
            }

            bars[i].bar.transform.localScale = new Vector2(width, 1f);
            bars[i].bar.transform.localPosition = new Vector2(xStartPosition + width * 0.5f, 0f);

            xStartPosition += width;
        }
    }
}