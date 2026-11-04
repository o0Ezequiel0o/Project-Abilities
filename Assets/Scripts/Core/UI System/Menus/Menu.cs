using UnityEngine;
using System;

namespace Zeke.UI.MenuManagment
{
    [RequireComponent(typeof(UIWindow))]
    public class Menu : MonoBehaviour
    {
        [field: SerializeField] public MenuIdentifier Identifier { get; private set; }
        [field: SerializeField] public UIWindow Window { get; private set; }

        public Action<Menu> onMenuOpened;
        public Action<Menu> onMenuClosed;

        private void Reset()
        {
            Window = GetComponent<UIWindow>();
        }

        private void Awake()
        {
            Window.onWindowOpened += OnWindowOpened;
            Window.onWindowClosed += OnWindowClosed;

            Window.canOpen += CanOpen;
            Window.canClose += CanClose;
        }

        private void OnEnable()
        {
            MenuManager.RegisterMenu(this);
        }

        private void OnDisable()
        {
            if (MenuManager.IsNull) return;
            MenuManager.UnregisterMenu(this);
        }

        private void OnWindowOpened(UIWindow window) => onMenuOpened.Invoke(this);
        private void OnWindowClosed(UIWindow window) => onMenuClosed.Invoke(this);

        private bool CanOpen() => MenuManager.CanOpen(this);
        private bool CanClose() => MenuManager.CanClose(this);
    }
}