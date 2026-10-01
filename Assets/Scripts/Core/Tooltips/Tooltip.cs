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

        [Space]

        [SerializeField] private bool useOffsetX = false;
        [SerializeField] private bool useOffsetY = true;

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
            Vector2 offset = new Vector2(
                useOffsetX ? offsetX * scale : 0f,
                useOffsetY ? offsetY * scale : 0f
            );

            tooltipRect.position = (Vector2)Input.mousePosition + offset;

            // Keep it inside the screen.
            Vector3[] corners = new Vector3[4];
            tooltipRect.GetWorldCorners(corners);

            Vector2 correction = Vector2.zero;

            if (corners[0].x < 0)
                correction.x = -corners[0].x;
            else if (corners[2].x > Screen.width)
                correction.x = Screen.width - corners[2].x;

            if (corners[0].y < 0)
                correction.y = -corners[0].y;
            else if (corners[2].y > Screen.height)
                correction.y = Screen.height - corners[2].y;

            tooltipRect.position += (Vector3)correction;
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