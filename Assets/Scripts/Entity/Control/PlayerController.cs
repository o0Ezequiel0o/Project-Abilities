using UnityEngine.InputSystem;
using UnityEngine;
using System;
using Zeke.Abilities;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private EntityMove entityMove;
    [SerializeField] private EntityAim entityAim;

    [Space]

    [SerializeField] private AbilityController abilityController;
    [SerializeField] private InteractionHandler interactionHandler;

    [Space]

    [SerializeField] private VehicleHandler vehicleHandler;

    [Header("Settings")]
    [SerializeField] private Transform center;

    [Header("Controls / Movement")]
    [SerializeField] private InputActionReference moveInput;

    [Header("Controls / Ability")]
    [SerializeField] private InputActionReference primaryInput;
    [SerializeField] private InputActionReference secondaryInput;
    [SerializeField] private InputActionReference utilityInput;
    [SerializeField] private InputActionReference ultimateInput;

    [Header("Controls / Interaction")]
    [SerializeField] private InputActionReference interactInput;
    [SerializeField] private InputActionReference exitVehicleInput;

    [Header("Controls / UI")]
    [SerializeField] private InputActionReference toggleItemsMenuInput;

    private readonly Dictionary<AbilityType, AbilityInput> abilityInputs = new Dictionary<AbilityType, AbilityInput>();

    private bool gameInputEnabled = false;

    private void Reset()
    {
        entityMove = GetComponentInChildren<EntityMove>();
        entityAim = GetComponentInChildren<EntityAim>();

        abilityController = GetComponentInChildren<AbilityController>();
        interactionHandler = GetComponentInChildren<InteractionHandler>();

        vehicleHandler = GetComponentInChildren<VehicleHandler>();

        center = GetComponentInChildren<Transform>();
    }

    private void OnEnable()
    {
        if (!Run.Instance.IsPaused)
        {
            EnableGameInput();
        }
    }

    private void OnDisable()
    {
        if (Run.IsNull) return;

        if (!Run.Instance.IsPaused)
        {
            DisableGameInput();
        }
    }

    private void Awake()
    {
        abilityInputs.Add(AbilityType.Primary, new AbilityInput(abilityController.TryUseAbility, AbilityType.Primary));
        abilityInputs.Add(AbilityType.Secondary, new AbilityInput(abilityController.TryUseAbility, AbilityType.Secondary));
        abilityInputs.Add(AbilityType.Utility, new AbilityInput(abilityController.TryUseAbility, AbilityType.Utility));
        abilityInputs.Add(AbilityType.Ultimate, new AbilityInput(abilityController.TryUseAbility, AbilityType.Ultimate));

        Run.Instance.onPause += OnPaused;
        Run.Instance.onResume += OnResumed;
    }

    private void OnDestroy()
    {
        DisableGameInput();

        if (Run.IsNull) return;

        Run.Instance.onPause -= OnPaused;
        Run.Instance.onResume -= OnResumed;
    }

    private void Update()
    {
        if (!gameInputEnabled) return;

        UpdateAimDirection();
        UpdateAbilityInputs();
    }

    private void OnPaused()
    {
        if (enabled)
        {
            DisableGameInput();
        }
    }

    private void OnResumed()
    {
        if (enabled)
        {
            EnableGameInput();
        }
    }

    private void EnableGameInput()
    {
        moveInput.action.performed += OnMovementInputPerformed;
        moveInput.action.canceled += OnMovementInputCanceled;

        primaryInput.action.started += abilityInputs[AbilityType.Primary].OnButtonPressed;
        secondaryInput.action.started += abilityInputs[AbilityType.Secondary].OnButtonPressed;
        utilityInput.action.started += abilityInputs[AbilityType.Utility].OnButtonPressed;
        ultimateInput.action.started += abilityInputs[AbilityType.Ultimate].OnButtonPressed;

        primaryInput.action.canceled += abilityInputs[AbilityType.Primary].OnButtonReleased;
        secondaryInput.action.canceled += abilityInputs[AbilityType.Secondary].OnButtonReleased;
        utilityInput.action.canceled += abilityInputs[AbilityType.Utility].OnButtonReleased;
        ultimateInput.action.canceled += abilityInputs[AbilityType.Ultimate].OnButtonReleased;

        interactInput.action.started += OnInteractionInputPerformed;
        exitVehicleInput.action.performed += OnExitVehicleInputPerformed;

        abilityController.onAbilityOverriden += ResetInputState;

        gameInputEnabled = true;
    }

    private void DisableGameInput()
    {
        moveInput.action.performed -= OnMovementInputPerformed;
        moveInput.action.canceled -= OnMovementInputCanceled;

        primaryInput.action.started -= abilityInputs[AbilityType.Primary].OnButtonPressed;
        secondaryInput.action.started -= abilityInputs[AbilityType.Secondary].OnButtonPressed;
        utilityInput.action.started -= abilityInputs[AbilityType.Utility].OnButtonPressed;
        ultimateInput.action.started -= abilityInputs[AbilityType.Ultimate].OnButtonPressed;

        primaryInput.action.canceled -= abilityInputs[AbilityType.Primary].OnButtonReleased;
        secondaryInput.action.canceled -= abilityInputs[AbilityType.Secondary].OnButtonReleased;
        utilityInput.action.canceled -= abilityInputs[AbilityType.Utility].OnButtonReleased;
        ultimateInput.action.canceled -= abilityInputs[AbilityType.Ultimate].OnButtonReleased;

        interactInput.action.started -= OnInteractionInputPerformed;
        exitVehicleInput.action.performed -= OnExitVehicleInputPerformed;

        abilityController.onAbilityOverriden -= ResetInputState;

        entityMove.StopMoving();
        gameInputEnabled = false;
    }

    private void UpdateAimDirection()
    {
        entityAim.AimTowards(((Vector2)(Run.Instance.MouseWorldPosition - center.position)).normalized);
    }

    private void UpdateAbilityInputs()
    {
        foreach (AbilityInput abilityInput in abilityInputs.Values)
        {
            abilityInput.Update();
        }
    }

    private void ResetInputState(AbilityController.AbilityOverrideData abilityOverrideData)
    {
        abilityInputs[abilityOverrideData.overridenAbility.Type].ResetInputState();
    }

    private void OnMovementInputPerformed(InputAction.CallbackContext context)
    {
        entityMove.MoveTowards(context.ReadValue<Vector2>());
    }

    private void OnMovementInputCanceled(InputAction.CallbackContext _)
    {
        entityMove.StopMoving();
    }

    private void OnInteractionInputPerformed(InputAction.CallbackContext _)
    {
        interactionHandler.TryInteractWithClose();
    }

    private void OnExitVehicleInputPerformed(InputAction.CallbackContext context)
    {
        vehicleHandler.ExitVehicle();
    }
}

public class AbilityInput
{
    private readonly Action<AbilityType, bool> action;
    private readonly AbilityType abilityType;

    private bool pressedThisFrame = false;
    private bool held = false;

    public AbilityInput(Action<AbilityType, bool> action, AbilityType abilityType)
    {
        this.action = action;
        this.abilityType = abilityType;
    }

    public void ResetInputState()
    {
        pressedThisFrame = false;
        held = false;
    }

    public void OnButtonPressed(InputAction.CallbackContext _)
    {
        pressedThisFrame = true;
        action?.Invoke(abilityType, false);
    }

    public void OnButtonReleased(InputAction.CallbackContext _)
    {
        held = false;
    }

    public void Update()
    {
        if (pressedThisFrame)
        {
            pressedThisFrame = false;
            held = true;
            return;
        }

        if (held)
        {
            action?.Invoke(abilityType, true);
        }
    }
}