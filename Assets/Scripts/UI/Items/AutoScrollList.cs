using UnityEngine;
using UnityEngine.UI;

public class AutoScrollList : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private float scrollSpeed = 0.15f;

    private int scrollDirection = 1;

    private void Reset()
    {
        scrollRect = GetComponent<ScrollRect>();
    }

    private void Update()
    {
        if (scrollRect == null) return;

        // Advance the normalized position (ranges from 0.0 to 1.0)
        scrollRect.horizontalNormalizedPosition += scrollDirection * scrollSpeed * Time.deltaTime;

        // Check if we hit the right end (1.0) and reverse direction
        if (scrollRect.horizontalNormalizedPosition >= 1f)
        {
            scrollRect.horizontalNormalizedPosition = 1f;
            scrollDirection = -1;
        }
        // Check if we hit the left start (0.0) and reverse direction
        else if (scrollRect.horizontalNormalizedPosition <= 0f)
        {
            scrollRect.horizontalNormalizedPosition = 0f;
            scrollDirection = 1;
        }
    }
}