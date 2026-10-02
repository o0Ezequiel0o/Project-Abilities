using TMPro;
using UnityEngine;

namespace Zeke.Tooltips
{
    public class Tooltip : Singleton<Tooltip>
    {
        [SerializeField] private GameObject tooltipObject;
        [SerializeField] private TMP_Text tooltipText;
        [SerializeField] private RectTransform tooltipRect;

        [Space]

        [SerializeField] private float offsetX = 0;
        [SerializeField] private float offsetY = -15f;

        [SerializeField] private OffsetType offsetTypeX;
        [SerializeField] private OffsetType offsetTypeY;

        [Space]

        [SerializeField] private float maxWidth = 400f;
        [SerializeField] private float maxHeight = 300f;
        [SerializeField] private Vector2 padding = new Vector2(20f, 10f);

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

        public static void SetText(string text)
        {
            Instance.tooltipText.text = text;
            Instance.tooltipText.ForceMeshUpdate();

            Vector2 sizeDelta = Vector2.zero;

            Vector2 textSize = Instance.tooltipText.GetPreferredValues(text, Instance.maxWidth, Instance.maxHeight);

            sizeDelta.x = Mathf.Clamp(textSize.x + Instance.padding.x, 0f, Instance.maxWidth);
            sizeDelta.y = Mathf.Clamp(textSize.y + Instance.padding.y, 0f, Instance.maxHeight);

            Instance.tooltipRect.sizeDelta = sizeDelta;
        }

        public static void Show(string text)
        {
            SetText(text);
            Instance.state = State.Show;
        }

        public static void Hide()
        {
            Instance.timer = 0f;
            Instance.state = State.Hide;
            Instance.tooltipObject.SetActive(false);
        }

        private void Awake()
        {
            canvas = tooltipRect.GetComponentInParent<Canvas>();
            Hide();
        }

        private void Update()
        {
            UpdateState();

            if (!tooltipObject.activeSelf)
                return;

            float scale = canvas.scaleFactor;

            UpdatePosition(scale);
        }

        private void UpdatePosition(float scale)
        {
            Vector2 mousePosition = Input.mousePosition;
            tooltipRect.position = mousePosition + GetOffset(scale);

            ClampToScreen(tooltipRect);
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
            Vector2 size = tooltipRect.rect.size * scale;
            Vector2 pivot = tooltipRect.pivot;

            Vector2 offset = new Vector2(offsetX, offsetY) * scale;

            if (offsetTypeX == OffsetType.Border)
            {
                if (offsetX > 0f)
                {
                    offset.x += size.x * pivot.x;
                }
                else if (offsetX < 0f)
                {
                    offset.x -= size.x * (1f - pivot.x);
                }
            }
            else if (offsetTypeX == OffsetType.Center)
            {
                offset.x += size.x * (pivot.x - 0.5f);
            }

            if (offsetTypeY == OffsetType.Border)
            {
                if (offsetY > 0f)
                {
                    offset.y += size.y * pivot.y;
                }
                else if (offsetY < 0f)
                {
                    offset.y -= size.y * (1f - pivot.y);
                }
            }
            else if (offsetTypeY == OffsetType.Center)
            {
                offset.y += size.y * (pivot.y - 0.5f);
            }

            return offset;
        }

        private void UpdateState()
        {
            if (state == State.Show && !tooltipObject.activeSelf)
            {
                timer += Time.deltaTime;

                if (timer > showDelay)
                {
                    Instance.tooltipObject.SetActive(true);
                }
            }
        }
    }
}