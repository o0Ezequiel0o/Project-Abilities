using TMPro;
using UnityEngine;

namespace Zeke.Tooltips
{
    public class Tooltip : GameService<Tooltip>
    {
        [Header("Dependency")]
        [SerializeField] private RectTransform tooltipBounds;
        [SerializeField] private RectTransform tooltip;
        [SerializeField] private TMP_Text tooltipText;

        [Header("Tooltip")]
        [SerializeField] private Vector2 offset = new Vector2(0f, -15f);

        [SerializeField] private OffsetType offsetTypeX = OffsetType.Border;
        [SerializeField] private OffsetType offsetTypeY = OffsetType.Border;

        [Space]

        [SerializeField] private Vector2 maxSize = new Vector2(400f, 200f);

        [Space]

        [SerializeField] private float showDelay = 1f;

        private State state = State.Hide;
        private float timer = 0f;

        private Canvas canvas;

        private enum State
        {
            Show,
            Hide,
        }

        private enum OffsetType
        {
            Center,
            Border
        }

        public void SetText(string text)
        {
            Instance.tooltipText.text = text;
        }

        public void Show(string text)
        {
            SetText(text);
            Instance.state = State.Show;
        }

        public void Hide()
        {
            Instance.timer = 0f;
            Instance.state = State.Hide;
            Instance.tooltip.gameObject.SetActive(false);
        }

        private void Awake()
        {
            canvas = tooltip.GetComponentInParent<Canvas>();
            tooltipBounds.sizeDelta = maxSize;
            Hide();
        }

        private void Update()
        {
            UpdateState();

            if (canvas == null)
            {
                Debug.LogError("Tooltip not in canvas");
                return;
            }

            if (tooltip.gameObject.activeInHierarchy)
            {
                UpdatePosition(canvas.scaleFactor);
            }
        }

        private void UpdatePosition(float scale)
        {
            Vector2 mousePosition = Input.mousePosition;
            tooltip.position = mousePosition + GetOffset(scale);

            ClampToScreen(tooltip);
        }

        private void ClampToScreen(RectTransform rectTransform)
        {
            Vector2 size = Vector2.Scale(rectTransform.rect.size, rectTransform.lossyScale);

            Vector3 position = rectTransform.position;
            Vector2 pivot = rectTransform.pivot;

            position.x = Mathf.Clamp(position.x, size.x * pivot.x, Screen.width - size.x * (1f - pivot.x));
            position.y = Mathf.Clamp(position.y, size.y * pivot.y, Screen.height - size.y * (1f - pivot.y));

            rectTransform.position = position;
        }

        private Vector2 GetOffset(float scale)
        {
            Vector2 size = tooltip.rect.size * scale;
            Vector2 pivot = tooltip.pivot;

            Vector2 newOffset = new Vector2(offset.x, offset.y) * scale;

            if (offsetTypeX == OffsetType.Border)
            {
                if (offset.x > 0f)
                {
                    newOffset.x += size.x * pivot.x;
                }
                else if (offset.x < 0f)
                {
                    newOffset.x -= size.x * (1f - pivot.x);
                }
            }
            else if (offsetTypeX == OffsetType.Center)
            {
                newOffset.x += size.x * (pivot.x - 0.5f);
            }

            if (offsetTypeY == OffsetType.Border)
            {
                if (offset.y > 0f)
                {
                    newOffset.y += size.y * pivot.y;
                }
                else if (offset.y < 0f)
                {
                    newOffset.y -= size.y * (1f - pivot.y);
                }
            }
            else if (offsetTypeY == OffsetType.Center)
            {
                newOffset.y += size.y * (pivot.y - 0.5f);
            }

            return newOffset;
        }

        private void UpdateState()
        {
            if (state == State.Show && !tooltip.gameObject.activeSelf)
            {
                timer += Time.unscaledDeltaTime;

                if (timer > showDelay)
                {
                    tooltip.gameObject.SetActive(true);
                }
            }
        }
    }
}