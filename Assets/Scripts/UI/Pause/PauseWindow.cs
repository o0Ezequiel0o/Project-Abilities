using UnityEngine;
using Zeke.UI;

[RequireComponent(typeof(UIWindow))]
public class PauseWindow : MonoBehaviour
{
    [SerializeField] private UIWindow uiWindow;

    private readonly Run.PauseID pauseID = new Run.PauseID();

    private void Reset()
    {
        uiWindow = GetComponent<UIWindow>();
    }

    private void OnEnable()
    {
        uiWindow.onWindowOpened += OnWindowOpened;
        uiWindow.onWindowClosed += OnWindowClosed;
    }

    private void OnDisable()
    {
        uiWindow.onWindowOpened -= OnWindowOpened;
        uiWindow.onWindowClosed -= OnWindowClosed;
    }

    private void OnWindowOpened(UIWindow _)
    {
        if (Run.IsNull) return;
        Run.Instance.Unpause(pauseID);
    }

    private void OnWindowClosed(UIWindow _)
    {
        if (Run.IsNull) return;
        Run.Instance.Unpause(pauseID);
    }
}