using UnityEngine;
using Zeke.UI;

[RequireComponent(typeof(UIWindow))]
public class PauseWindow : MonoBehaviour
{
    [SerializeField] private UIWindow uiWindow;

    private readonly GameInstance.PauseID pauseID = new GameInstance.PauseID();

    private void Reset()
    {
        uiWindow = GetComponent<UIWindow>();
    }

    private void Awake()
    {
        uiWindow.onWindowOpened += (_) => GameInstance.Pause(pauseID);
        uiWindow.onWindowClosed += (_) => GameInstance.Unpause(pauseID);
    }
}