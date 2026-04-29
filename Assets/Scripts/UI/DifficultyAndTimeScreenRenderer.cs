using UnityEngine;
using Zeke.UI;
using TMPro;

public class DifficultyAndTimeScreenRenderer : MonoBehaviour
{
    [SerializeField] private UIWindow windowPrefab;

    private UIWindow window;

    private void Start()
    {
        window = Instantiate(windowPrefab, GameInstance.ScreenCanvas.transform);
    }

    private void Update()
    {
        int minutes = Mathf.FloorToInt(GameInstance.RunTimer / 60F);
        int seconds = Mathf.FloorToInt(GameInstance.RunTimer - minutes * 60);

        window.TryGetElement<TextMeshProUGUI>("Time").text = string.Format("{0:00}:{1:00}", minutes, seconds);
        window.TryGetElement<TextMeshProUGUI>("Difficulty").text = Mathf.FloorToInt(GameInstance.Difficulty).ToString();
    }

    private void OnDestroy()
    {
        if (window == null) return;
        window.DestroyWindow();
    }
}