using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup), typeof(RectTransform))]
public class ItemLayoutGroup : MonoBehaviour
{
    [SerializeField] private GridLayoutGroup layout;
    [SerializeField] private RectTransform rectTransform;

    [Header("Debug")]
    [SerializeField] private GameObject addChildren;
    [SerializeField] private KeyCode key;

    private Vector2 layoutSize = Vector2.zero;

    private float HorizontalPadding => layout.padding.right + layout.padding.left;
    private float VerticalPadding => layout.padding.top + layout.padding.bottom;

    private int Cells => rectTransform.childCount;

    private void Update()
    {
        layoutSize.x = Mathf.Abs(rectTransform.rect.width);
        layoutSize.y = Mathf.Abs(rectTransform.rect.height);

        if (Input.GetKeyDown(key))
        {
            Instantiate(addChildren, rectTransform);
        }

        RescaleCellsToFitHeight();
    }

    private void LateUpdate()
    {
        RescaleCellsToFitHeight();
    }

    private Vector2 GetPaddingSpaceUsed()
    {
        Vector2 spaceUsed = Vector2.zero;

        spaceUsed.x += HorizontalPadding;
        spaceUsed.y += VerticalPadding;

        return spaceUsed;
    }

    private Vector2Int CalculateMaxCells(Vector2 freeSpace, Vector2 spacing, Vector2 cellSize)
    {
        Vector2Int maxCells = new Vector2Int
        {
            x = CalculateMaxCells(freeSpace.x, spacing.x, cellSize.x),
            y = CalculateMaxCells(freeSpace.y, spacing.y, cellSize.y)
        };

        return maxCells;
    }

    private int CalculateMaxCells(float freeSpace, float spacing, float cellSize)
    {
        return Mathf.FloorToInt((freeSpace + spacing) / (cellSize + spacing));
    }

    private void RescaleCellsToFitHeight()
    {
        Vector2 paddingSpaceUsed = GetPaddingSpaceUsed();
        Vector2 freeSpace = layoutSize - paddingSpaceUsed;

        Vector2Int maxCells = CalculateMaxCells(freeSpace, layout.spacing, layout.cellSize);

        if (Cells > maxCells.x * maxCells.y)
        {
            freeSpace.y -= Mathf.Max(0f, ((layout.cellSize.y + layout.spacing.y) * maxCells.y) - layout.spacing.y);

            float yRequired = layout.cellSize.y + layout.spacing.y - freeSpace.y;

            float cellRatioRequired = 1f - (yRequired / (layoutSize.y - paddingSpaceUsed.y + yRequired));
            float cellWidthHeightRatio = (layout.cellSize.x / layout.cellSize.y);

            layout.cellSize = new Vector2()
            {
                x = layout.cellSize.x * cellRatioRequired * cellWidthHeightRatio,
                y = layout.cellSize.y * cellRatioRequired
            };
        }
    }
}