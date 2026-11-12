using UnityEngine;
using Zeke.UI;
using TMPro;

public class DifficultyScreenRenderer : MonoBehaviour
{
    [SerializeField] private UIWindow windowPrefab;

    private UIWindow window;

    private void Start()
    {
        window = Instantiate(windowPrefab, Run.Instance.ScreenCanvas.transform);
    }

    private void Update()
    {
        window.TryGetElement<TextMeshProUGUI>("Difficulty").SetText(Mathf.FloorToInt(Run.Instance.Difficulty).ToString());
    }

    private void OnDestroy()
    {
        if (window == null) return;
        window.DestroyWindow();
    }
}