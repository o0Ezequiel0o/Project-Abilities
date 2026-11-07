using UnityEngine;
using Zeke.UI;
using TMPro;

public class DifficultyAndTimeScreenRenderer : MonoBehaviour
{
    [SerializeField] private UIWindow windowPrefab;

    private UIWindow window;

    private void Start()
    {
        window = Instantiate(windowPrefab, Run.Instance.ScreenCanvas.transform);
    }

    private void Update()
    {
        int minutes = Mathf.FloorToInt(Run.Instance.RunTimer / 60F);
        int seconds = Mathf.FloorToInt(Run.Instance.RunTimer - minutes * 60);

        window.TryGetElement<TextMeshProUGUI>("Time").SetText(string.Format("{0:00}:{1:00}", minutes, seconds));
        window.TryGetElement<TextMeshProUGUI>("Difficulty").SetText(Mathf.FloorToInt(Run.Instance.Difficulty).ToString());
    }

    private void OnDestroy()
    {
        if (window == null) return;
        window.DestroyWindow();
    }
}