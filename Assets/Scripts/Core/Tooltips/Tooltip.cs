using UnityEngine;
using TMPro;

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

        public static void SetText(string text)
        {
            Instance.tooltipText.text = text;

            // Force TMP to update its measurements.
            Instance.tooltipText.ForceMeshUpdate();

            Vector2 textSize = Instance.tooltipText.GetPreferredValues(text, Instance.maxWidth, Instance.maxHeight);

            // Add padding for the tooltip background/box.
            Vector2 finalSize = textSize + Instance.padding;

            // Cap the size.
            finalSize.x = Mathf.Min(finalSize.x, Instance.maxWidth);
            finalSize.y = Mathf.Min(finalSize.y, Instance.maxHeight);

            Instance.tooltipRect.sizeDelta = finalSize;
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

            Vector2 offset = new Vector2(
                offsetX * scale,
                offsetY * scale
            );

            Vector2 size = tooltipRect.rect.size * scale;
            Vector2 pivot = tooltipRect.pivot;

            Vector2 position = mousePosition;

            // X axis
            //if (offset.x >= 0)
            //{
            //    // Offset is measured from the mouse to the LEFT edge.
            //    position.x += offset.x + size.x * pivot.x;
            //}
            //else
            //{
            //    // Offset is measured from the mouse to the RIGHT edge.
            //    position.x += offset.x - size.x * (1f - pivot.x);
            //}

            // Y axis
            if (offset.y >= 0)
            {
                // Offset is measured from the mouse to the BOTTOM edge.
                position.y += offset.y + size.y * pivot.y;
            }
            else
            {
                // Offset is measured from the mouse to the TOP edge.
                position.y += offset.y - size.y * (1f - pivot.y);
            }

            tooltipRect.position = position;

            // Keep the tooltip inside the screen.
            Vector3[] corners = new Vector3[4];
            tooltipRect.GetWorldCorners(corners);

            Vector3 correctedPosition = tooltipRect.position;

            // Left
            //if (corners[0].x < 0)
            //    correctedPosition.x += -corners[0].x;

            // Right
            //if (corners[2].x > Screen.width)
            //    correctedPosition.x -= corners[2].x - Screen.width;

            // Bottom
            if (corners[0].y < 0)
                correctedPosition.y += -corners[0].y;

            // Top
            if (corners[2].y > Screen.height)
                correctedPosition.y -= corners[2].y - Screen.height;

            tooltipRect.position = correctedPosition;
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