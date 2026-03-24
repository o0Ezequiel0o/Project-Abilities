using UnityEngine.InputSystem;
using UnityEngine;
using System;
using Zeke.Abilities;

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
    [SerializeField] private ItemsRenderer itemsRenderer;

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

    private AbilityInput primaryAbilityInput;
    private AbilityInput secondaryAbilityInput;
    private AbilityInput utilityAbilityInput;
    private AbilityInput ultimateAbilityInput;

    private Camera mainCam;

    private void Reset()
    {
        entityMove = GetComponentInChildren<EntityMove>();
        entityAim = GetComponentInChildren<EntityAim>();

        abilityController = GetComponentInChildren<AbilityController>();
        interactionHandler = GetComponentInChildren<InteractionHandler>();

        vehicleHandler = GetComponentInChildren<VehicleHandler>();
        itemsRenderer = GetComponentInChildren<ItemsRenderer>();

        center = GetComponentInChildren<Transform>();
    }

    private void OnEnable()
    {
        moveInput.action.performed += OnMovementInputPerformed;
        moveInput.action.canceled += OnMovementInputCanceled;

        primaryInput.action.started += primaryAbilityInput.OnButtonPressed;
        secondaryInput.action.started += secondaryAbilityInput.OnButtonPressed;
        utilityInput.action.started += utilityAbilityInput.OnButtonPressed;
        ultimateInput.action.started += ultimateAbilityInput.OnButtonPressed;

        primaryInput.action.canceled += primaryAbilityInput.OnButtonReleased;
        secondaryInput.action.canceled += secondaryAbilityInput.OnButtonReleased;
        utilityInput.action.canceled += utilityAbilityInput.OnButtonReleased;
        ultimateInput.action.canceled += ultimateAbilityInput.OnButtonReleased;

        interactInput.action.started += OnInteractionInputPerformed;
        exitVehicleInput.action.performed += OnExitVehicleInputPerformed;

        toggleItemsMenuInput.action.performed += OnToggleItemsMenuInputPerformed;
    }

    private void OnDisable()
    {
        moveInput.action.performed -= OnMovementInputPerformed;
        moveInput.action.canceled -= OnMovementInputCanceled;

        primaryInput.action.started -= primaryAbilityInput.OnButtonPressed;
        secondaryInput.action.started -= secondaryAbilityInput.OnButtonPressed;
        utilityInput.action.started -= utilityAbilityInput.OnButtonPressed;
        ultimateInput.action.started -= ultimateAbilityInput.OnButtonPressed;

        primaryInput.action.canceled -= primaryAbilityInput.OnButtonReleased;
        secondaryInput.action.canceled -= secondaryAbilityInput.OnButtonReleased;
        utilityInput.action.canceled -= utilityAbilityInput.OnButtonReleased;
        ultimateInput.action.canceled -= ultimateAbilityInput.OnButtonReleased;

        interactInput.action.started -= OnInteractionInputPerformed;
        exitVehicleInput.action.performed -= OnExitVehicleInputPerformed;

        toggleItemsMenuInput.action.performed -= OnToggleItemsMenuInputPerformed;
    }

    private void Awake()
    {
        mainCam = Camera.main;

        primaryAbilityInput = new AbilityInput(abilityController.TryUseAbility, AbilityType.Primary);
        secondaryAbilityInput = new AbilityInput(abilityController.TryUseAbility, AbilityType.Secondary);
        utilityAbilityInput = new AbilityInput(abilityController.TryUseAbility, AbilityType.Utility);
        ultimateAbilityInput = new AbilityInput(abilityController.TryUseAbility, AbilityType.Ultimate);
}

    private void Update()
    {
        UpdateAimDirection();
        UpdateAbilityInputs();
    }

    private void UpdateAimDirection()
    {
        entityAim.AimTowards(((Vector2)(GameInstance.MouseWorldPosition - center.position)).normalized);
    }

    private void UpdateAbilityInputs()
    {
        primaryAbilityInput.Update();
        secondaryAbilityInput.Update();
        utilityAbilityInput.Update();
        ultimateAbilityInput.Update();
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

    private void OnToggleItemsMenuInputPerformed(InputAction.CallbackContext context)
    {
        itemsRenderer.ToggleMenuVisibility();
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

    public void OnButtonPressed(InputAction.CallbackContext _)
    {
        action?.Invoke(abilityType, false);
        pressedThisFrame = true;
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