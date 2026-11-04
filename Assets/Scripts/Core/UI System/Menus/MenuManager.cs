using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Serialization;

namespace Zeke.UI.MenuManagment
{
    public class MenuManager : Singleton<MenuManager>
    {
        [SerializeField] private Dictionary<MenuIdentifier, Dictionary<MenuState, List<MenuInteraction>>> interactions;
        [SerializeField] private Dictionary<MenuIdentifier, List<MenuConstraint>> constraints;

        private readonly Dictionary<MenuIdentifier, List<Menu>> registeredMenus = new Dictionary<MenuIdentifier, List<Menu>>();

        public static void RegisterMenu(Menu menu) 
        {
            if (menu.Identifier == null)
            {
                Debug.LogWarning("Can't register menu with null identifier", menu);
                return;
            }

            if (!Instance.registeredMenus.TryGetValue(menu.Identifier, out List<Menu> menus))
            {
                Instance.registeredMenus.Add(menu.Identifier, new List<Menu>());
            }

            if (Instance.registeredMenus[menu.Identifier].Contains(menu))
            {
                Debug.LogWarning("Duplicate key and menu pair");
                return;
            }

            Instance.registeredMenus[menu.Identifier].Add(menu);

            menu.onMenuOpened += Instance.OnMenuOpened;
            menu.onMenuClosed += Instance.OnMenuClosed;
        }

        public static void UnregisterMenu(Menu menu)
        {
            if (!Instance.registeredMenus.TryGetValue(menu.Identifier, out List<Menu> menus))
            {
                return;
            }

            Instance.registeredMenus[menu.Identifier].Remove(menu);

            menu.onMenuOpened -= Instance.OnMenuOpened;
            menu.onMenuClosed -= Instance.OnMenuClosed;
        }

        public static bool CanOpen(Menu menu)
        {
            return TryPerformAction(menu, constraints => constraints.avoidsOpening);
        }

        public static bool CanClose(Menu menu)
        {
            return TryPerformAction(menu, constraints => constraints.avoidsClosing);
        }

        private static bool TryPerformAction(Menu menu, Func<MenuConstraint, bool> constraints)
        {
            if (!Instance.constraints.TryGetValue(menu.Identifier, out List<MenuConstraint> menuConstraints))
            {
                return true;
            }

            for (int i = 0; i < menuConstraints.Count; i++)
            {
                if (Instance.registeredMenus.TryGetValue(menuConstraints[i].menu, out List<Menu> menuInstances))
                {
                    for (int x = 0; x < menuInstances.Count; x++)
                    {
                        if (StateEquals(menuConstraints[i].state, menuInstances[x].Window) && constraints(menuConstraints[i]))
                        {
                            Debug.Log($"{menuInstances[x].name} ({menuInstances.Count} duplicate keys) is not letting {menu} open.");
                            return false;
                        }

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
                    SetMenusVisibility(menuInteraction.menu, true);
                    break;

                case MenuState.Close:
                    SetMenusVisibility(menuInteraction.menu, false);
                    break;
            }
        }

        private void SetMenusVisibility(MenuIdentifier identifier, bool visibility)
        {
            List<Menu> menus = registeredMenus[identifier];

            for (int i = 0; i < menus.Count; i++)
            {
                menus[i].Window.SetGraphicVisibility(visibility);
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
        private struct MenuConstraint
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