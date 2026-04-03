using UnityEngine;

public class HierarchySortingOrder : MonoBehaviour
{
    [SerializeField] private int sortingOrder;

    private void Awake()
    {
        transform.SetSiblingIndex(sortingOrder);
    }
}