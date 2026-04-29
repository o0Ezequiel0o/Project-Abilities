using UnityEngine;

public class HierarchySortingOrder : MonoBehaviour
{
    [SerializeField] private int sortingOrder;

    private void Awake()
    {
        transform.SetSiblingIndex(sortingOrder);
    }

    private void Update()
    {
        if (transform.parent != null && transform.parent.hasChanged)
        {
            transform.SetSiblingIndex(sortingOrder);
            transform.parent.hasChanged = false;
        }
    }
}