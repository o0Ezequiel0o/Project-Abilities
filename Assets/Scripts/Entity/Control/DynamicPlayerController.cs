using UnityEngine.InputSystem;
using UnityEngine;
using Zeke.Abilities;

public class DynamicPlayerController : MonoBehaviour
{
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

    private EntityMove entityMove;
    private EntityAim entityAim;

    private AbilityController abilityController;
    private InteractionHandler interactionHandler;

    private VehicleHandler vehicleHandler;
    private ItemsScreenRenderer itemsRenderer;

    private AbilityInput primaryAbilityInput;
    private AbilityInput secondaryAbilityInput;
    private AbilityInput utilityAbilityInput;
    private AbilityInput ultimateAbilityInput;

    private Camera mainCam;

    private void Reset()
    {
        center = GetComponentInChildren<Transform>();
    }

    private void OnEnable()
    {
        if (entityMove != null)
        {
            moveInput.action.performed += OnMovementInputPerformed;
            moveInput.action.canceled += OnMovementInputCanceled;
        }

        if (abilityController != null)
        {
            primaryInput.action.started += primaryAbilityInput.OnButtonPressed;
            secondaryInput.action.started += secondaryAbilityInput.OnButtonPressed;
            utilityInput.action.started += utilityAbilityInput.OnButtonPressed;
            ultimateInput.action.started += ultimateAbilityInput.OnButtonPressed;

            primaryInput.action.canceled += primaryAbilityInput.OnButtonReleased;
            secondaryInput.action.canceled += secondaryAbilityInput.OnButtonReleased;
            utilityInput.action.canceled += utilityAbilityInput.OnButtonReleased;
            ultimateInput.action.canceled += ultimateAbilityInput.OnButtonReleased;
        }

        if (interactionHandler != null)
        {
            interactInput.action.started += OnInteractionInputPerformed;
        }

        if (vehicleHandler != null)
        {
            exitVehicleInput.action.performed += OnExitVehicleInputPerformed;
        }

        if (itemsRenderer  != null)
        {
            toggleItemsMenuInput.action.performed += OnToggleItemsMenuInputPerformed;
        }
    }

    private void OnDisable()
    {
        if (entityMove != null)
        {
            moveInput.action.performed -= OnMovementInputPerformed;
            moveInput.action.canceled -= OnMovementInputCanceled;
        }

        if (abilityController != null)
        {
            primaryInput.action.started -= primaryAbilityInput.OnButtonPressed;
            secondaryInput.action.started -= secondaryAbilityInput.OnButtonPressed;
            utilityInput.action.started -= utilityAbilityInput.OnButtonPressed;
            ultimateInput.action.started -= ultimateAbilityInput.OnButtonPressed;

            primaryInput.action.canceled -= primaryAbilityInput.OnButtonReleased;
            secondaryInput.action.canceled -= secondaryAbilityInput.OnButtonReleased;
            utilityInput.action.canceled -= utilityAbilityInput.OnButtonReleased;
            ultimateInput.action.canceled -= ultimateAbilityInput.OnButtonReleased;
        }

        if (interactionHandler != null)
        {
            interactInput.action.started -= OnInteractionInputPerformed;
        }
        
        if (vehicleHandler != null)
        {
            exitVehicleInput.action.performed -= OnExitVehicleInputPerformed;
        }

        if (itemsRenderer != null)
        {
            toggleItemsMenuInput.action.performed -= OnToggleItemsMenuInputPerformed;
        }
    }

    private void Awake()
    {
        mainCam = Camera.main;

        entityMove = GetComponentInChildren<EntityMove>();
        entityAim = GetComponentInChildren<EntityAim>();

        abilityController = GetComponentInChildren<AbilityController>();
        interactionHandler = GetComponentInChildren<InteractionHandler>();

        vehicleHandler = GetComponentInChildren<VehicleHandler>();
        itemsRenderer = GetComponentInChildren<ItemsScreenRenderer>();

        primaryAbilityInput = new AbilityInput(abilityController.TryUseAbility, AbilityType.Primary);
        secondaryAbilityInput = new AbilityInput(abilityController.TryUseAbility, AbilityType.Secondary);
        utilityAbilityInput = new AbilityInput(abilityController.TryUseAbility, AbilityType.Utility);
        ultimateAbilityInput = new AbilityInput(abilityController.TryUseAbility, AbilityType.Ultimate);
    }

    private void Update()
    {
        if (entityAim != null)
        {
            UpdateAimDirection();
        }

        if (abilityController != null)
        {
            UpdateAbilityInputs();
        }
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