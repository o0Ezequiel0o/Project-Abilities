using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-1)]
public class InputEvents : MonoBehaviour
{
    [SerializeField] private List<Element> events;

    [Serializable]
    private class Element
    {
        [field: SerializeField] public InputActionReference Input { get; private set; }

        [field: Space]

        [field: SerializeField] public UnityEvent Started { get; private set; }
        [field: SerializeField] public UnityEvent Performed { get; private set; }
        [field: SerializeField] public UnityEvent Canceled { get; private set; }

        public void SubscribeToEvents()
        {
            Input.action.started += OnStarted;
            Input.action.performed += OnPerformed;
            Input.action.canceled += OnCanceled;
        }

        public void UnsubscribeFromEvents()
        {
            Input.action.started -= OnStarted;
            Input.action.performed -= OnPerformed;
            Input.action.canceled -= OnCanceled;
        }

        private void OnStarted(InputAction.CallbackContext ctx) => Started?.Invoke();
        private void OnPerformed(InputAction.CallbackContext ctx) => Performed?.Invoke();
        private void OnCanceled(InputAction.CallbackContext ctx) => Canceled?.Invoke();
    }

    private void OnEnable()
    {
        for (int i = 0; i < events.Count; i++)
        {
            events[i].SubscribeToEvents();
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < events.Count; i++)
        {
            events[i].UnsubscribeFromEvents();
        }
    }
}