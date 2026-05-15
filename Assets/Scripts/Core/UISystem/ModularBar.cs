using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModularBar : MonoBehaviour
{
    [SerializeField] protected RectTransform root;

    public float CombinedValue => combinedValue;
    public float CombinedMaxValue => combinedMaxValue;

    private float combinedValue = 0f;
    private float combinedMaxValue = 0f;

    private readonly List<BarIDPair> bars = new List<BarIDPair>();

    public class BarID { }

    protected class Bar
    {
        public StatusBarValue values;
        public RectTransform rectTransform;

        public Bar(StatusBarValue values, RectTransform rectTransform)
        {
            this.values = values;
            this.rectTransform = rectTransform;
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

    public ModularBar AddBar(BarID id, Color color)
    {
        return AddBar(id, color, 0f, 0f);
    }

    public ModularBar AddBar(BarID id, Color color, float current, float max)
    {
        GameObject go = new GameObject("bar", typeof(CanvasRenderer), typeof(Image));
        StatusBarValue values = new StatusBarValue(current, max);

        RectTransform rectTransform = go.GetComponent<RectTransform>();
        go.GetComponent<Image>().color = color;
        rectTransform.SetParent(root.transform, false);

        Bar bar = new Bar(values, rectTransform);
        bars.Add(new BarIDPair(id, bar));

        combinedMaxValue += max;
        combinedValue += current;

        return this;
    }

    public virtual void UpdateBar(BarID id, float current, float max)
    {
        if (TryGetBar(id, out Bar bar))
        {
            StatusBarValue oldValues = bar.values;
            bar.values = new StatusBarValue(current, max);

            float currentDifference = current - oldValues.value;
            float maxDifference = max - oldValues.maxValue;

            combinedValue += currentDifference;
            combinedMaxValue += maxDifference;
        }

        UpdateBarRendering();
    }

    protected bool TryGetBar(BarID id, out Bar bar)
    {
        for (int i = 0; i <  bars.Count; i++)
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
        if (root.gameObject == null) return;

        for (int i = 0; i < bars.Count; i++)
        {
            if (bars[i].bar.rectTransform.gameObject == null) continue;

            float width = root.rect.width * bars[i].bar.values.Percentage * (bars[i].bar.values.maxValue / combinedMaxValue);
            bars[i].bar.rectTransform.sizeDelta = new Vector2(width, root.rect.height);
        }
    }
}