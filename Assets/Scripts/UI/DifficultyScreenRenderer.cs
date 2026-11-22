using UnityEngine;
using Zeke.UI;
using TMPro;

public class DifficultyScreenRenderer : MonoBehaviour
{
    [SerializeField] private UIWindow window;

    private void Update()
    {
        window.TryGetElement<TextMeshProUGUI>("Difficulty").SetText(Mathf.FloorToInt(Run.Instance.Difficulty).ToString());
    }
}