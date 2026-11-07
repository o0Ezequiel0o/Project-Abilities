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

    private void Awake()
    {
        uiWindow.onWindowOpened += (_) => Run.Instance.Pause(pauseID);
        uiWindow.onWindowClosed += (_) => Run.Instance.Unpause(pauseID);
    }
}