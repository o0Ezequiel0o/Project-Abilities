using System.Collections.Generic;
using UnityEngine;
using System;

namespace Zeke.UI.MenuManagment
{
    public class MenuManager : Singleton<MenuManager>
    {
        [SerializeField] private Dictionary<MenuIdentifier, Dictionary<MenuState, List<MenuInteraction>>> interactions;
        [SerializeField] private Dictionary<MenuIdentifier, List<MenuConstraints>> constraints;

        private readonly Dictionary<MenuIdentifier, Menu> registeredMenus = new Dictionary<MenuIdentifier, Menu>();

        public static void RegisterMenu(Menu menu) 
        {
            if (menu.Identifier == null)
            {
                Debug.LogWarning("Can't register menu with null identifier", menu);
                return;
            }

            Instance.registeredMenus.Add(menu.Identifier, menu);

            menu.onMenuOpened += Instance.OnMenuOpened;
            menu.onMenuClosed += Instance.OnMenuClosed;
        }

        public static void UnregisterMenu(Menu menu)
        {
            Instance.registeredMenus.Remove(menu.Identifier);

            menu.onMenuOpened -= Instance.OnMenuOpened;
            menu.onMenuClosed -= Instance.OnMenuClosed;
        }

        public static bool CanOpen(Menu menu)
        {
            if (!Instance.constraints.TryGetValue(menu.Identifier, out List<MenuConstraints> menuConstraints))
            {
                return true;
            }

            for (int i = 0; i < menuConstraints.Count; i++)
            {
                if (Instance.registeredMenus.TryGetValue(menuConstraints[i].menu, out Menu menuInstance))
                {
                    if (StateEquals(menuConstraints[i].state, menuInstance.Window) && menuConstraints[i].avoidsOpening)
                    {
                        Debug.Log($"{menuInstance.name} is not letting {menu} open.");
                        return false;
                    }
                }
            }

            return true;
        }

        public static bool CanClose(Menu menu)
        {
            if (!Instance.constraints.TryGetValue(menu.Identifier, out List<MenuConstraints> menuConstraints))
            {
                return true;
            }

            for (int i = 0; i < menuConstraints.Count; i++)
            {
                if (Instance.registeredMenus.TryGetValue(menuConstraints[i].menu, out Menu menuInstance))
                {
                    if (StateEquals(menuConstraints[i].state, menuInstance.Window) && menuConstraints[i].avoidsClosing)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        protected override void OnInitialization()
        {
            registeredMenus.Clear();
        }

        private void OnMenuOpened(Menu menu) => ExecuteEvents(MenuState.Open, menu);
        private void OnMenuClosed(Menu menu) => ExecuteEvents(MenuState.Close, menu);

        private void ExecuteEvents(MenuState @event, Menu menu)
        {
            if (interactions.TryGetValue(menu.Identifier, out Dictionary<MenuState, List<MenuInteraction>> menuInteractions))
            {
                if (menuInteractions.TryGetValue(@event, out List<MenuInteraction> interaction))
                {
                    for (int i = 0; i < interaction.Count; i++)
                    {
                        ExecuteEvent(interaction[i]);
                    }
                }
            }
        }

        private void ExecuteEvent(MenuInteraction menuInteraction)
        {
            switch (menuInteraction.@event)
            {
                case MenuState.Open:
                    registeredMenus[menuInteraction.menu].Window.SetGraphicVisibility(true);
                    break;

                case MenuState.Close:
                    registeredMenus[menuInteraction.menu].Window.SetGraphicVisibility(false);
                    break;
            }
        }

        private static bool StateEquals(MenuState state, UIWindow window)
        {
            return (state == MenuState.Open && window.IsVisible) || (state == MenuState.Close && !window.IsVisible);
        }

        [Serializable]
        private struct MenuInteraction
        {
            public MenuIdentifier menu;
            public MenuState @event;
        }

        [Serializable]
        private struct MenuConstraints
        {
            public MenuIdentifier menu;
            public MenuState state;

            public bool avoidsOpening;
            public bool avoidsClosing;
        }

        private enum MenuState
        {
            Open,
            Close
        }
    }
}