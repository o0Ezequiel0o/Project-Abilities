using UnityEngine;
using UnityEngine.UI;

public class AutoScrollList : MonoBehaviour
{
    [SerializeField] protected ScrollRect scrollRect;
    [SerializeField] protected float scrollSpeed = 0.15f;

    protected virtual float CurrentScrollSpeed => scrollSpeed;

    private int scrollDirection = 1;

    protected virtual void Reset()
    {
        scrollRect = GetComponent<ScrollRect>();
    }

    protected virtual void Update()
    {
        if (scrollRect == null) return;

        scrollRect.horizontalNormalizedPosition += scrollDirection * CurrentScrollSpeed * Time.deltaTime;

        if (scrollRect.horizontalNormalizedPosition >= 1f)
        {
            scrollRect.horizontalNormalizedPosition = 1f;
            scrollDirection = -1;
        }
        else if (scrollRect.horizontalNormalizedPosition <= 0f)
        {
            scrollRect.horizontalNormalizedPosition = 0f;
            scrollDirection = 1;
        }
    }
}