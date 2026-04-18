using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Zeke.Abilities;
using Zeke.Items;
using Zeke.UI;
using TMPro;

public class DebugEnemyViewer : MonoBehaviour
{
    [SerializeField] private UIWindow window;
    [SerializeField] private RenderTexture renderTexture;
    [SerializeField] private LayerMask interactionLayers;
    [SerializeReference] private InputActionReference selectInput;

    private Camera mainCamera;
    private Camera entityCamera;

    private GameObject currentObj;
    private Vector3 startPosition;
    private Bounds targetBounds;

    private UIWindow windowInstance;

    private void Awake()
    {
        selectInput.action.started += OnClick;
        mainCamera = Camera.main;
        CreateEntityCamera();
    }

    private void Update()
    {
        if (windowInstance != null)
        {
            UpdateValues();
        }
    }

    private void LateUpdate()
    {
        if (windowInstance != null && currentObj != null)
        {
            AdjustCameraToBounds(targetBounds);
        }
    }

    private void CreateEntityCamera()
    {
        entityCamera = new GameObject("Entity View Cam").AddComponent<Camera>();
        entityCamera.targetTexture = renderTexture;
        entityCamera.orthographic = true;
        entityCamera.targetDisplay = 8;
    }

    private Bounds GetBounds(GameObject target)
    {
        startPosition = target.transform.position;

        Bounds bounds = new Bounds(target.transform.position, Vector2.zero);
        SpriteRenderer[] renderers = target.GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer renderer in renderers)
        {
            bounds.Encapsulate(renderer.bounds);
        }

        return bounds;
    }

    private void AdjustCameraToBounds(Bounds bounds)
    {
        float size = Mathf.Sqrt((targetBounds.size.x * targetBounds.size.x) + (targetBounds.size.y * targetBounds.size.y));

        entityCamera.orthographicSize = size * 0.5f;

        Vector3 position = (currentObj.transform.position - startPosition) + targetBounds.center;
        position.z = -1f;

        entityCamera.transform.position = position;
    }

    private void OnClick(InputAction.CallbackContext context)
    {
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorldPosition.z = 0f;

        Collider2D hit = Physics2D.OverlapPoint(mouseWorldPosition, interactionLayers);

        if (hit != null)
        {
            currentObj = hit.gameObject;
            targetBounds = GetBounds(currentObj);
        }

        if (windowInstance != null)
        {
            Destroy(windowInstance.gameObject);
        }

        windowInstance = Instantiate(window, GameInstance.ScreenCanvas.transform);

        windowInstance.TryGetElement<AbilityDisplaySlot>("Primary Ability").gameObject.SetActive(false);
        windowInstance.TryGetElement<AbilityDisplaySlot>("Secondary Ability").gameObject.SetActive(false);
        windowInstance.TryGetElement<AbilityDisplaySlot>("Utility Ability").gameObject.SetActive(false);
        windowInstance.TryGetElement<AbilityDisplaySlot>("Ultimate Ability").gameObject.SetActive(false);

        UpdateValues();
    }

    private void UpdateValues()
    {
        if (currentObj == null) return;

        Damageable damageable = currentObj.GetComponent<Damageable>();
        EntityMove entityMove = currentObj.GetComponent<EntityMove>();
        ItemHandler itemHandler = currentObj.GetComponent<ItemHandler>();
        AbilityController abilityController = currentObj.GetComponent<AbilityController>();
        StatusEffectHandler statusEffectHandler = currentObj.GetComponent<StatusEffectHandler>();

        windowInstance.TryGetElement<TextMeshProUGUI>("Name").text = currentObj.name;
        
        if (damageable != null)
        {
            windowInstance.TryGetElement<TextMeshProUGUI>("Max Health").text = damageable.MaxHealth.ToString();
            windowInstance.TryGetElement<TextMeshProUGUI>("Health").text = damageable.Health.ToString();
            windowInstance.TryGetElement<TextMeshProUGUI>("Regen").text = damageable.HealthRegen.ToString();
            windowInstance.TryGetElement<TextMeshProUGUI>("Armor").text = damageable.Armor.ToString();

            string damageReductionText = (Damageable.CalculateDamageReduction(damageable.Armor.Value) * 100f).ToString() + "%";
            windowInstance.TryGetElement<TextMeshProUGUI>("Damage Reduction").text = damageReductionText;
        }

        if (entityMove != null)
        {
            windowInstance.TryGetElement<TextMeshProUGUI>("Move Speed").text = entityMove.MoveSpeed.ToString();
        }

        if (itemHandler != null)
        {
            windowInstance.TryGetElement<TextMeshProUGUI>("Luck").text = itemHandler.Luck.ToString();
        }

        if (statusEffectHandler != null)
        {
            Transform transform = windowInstance.TryGetElement("Status Effect Layout Group").transform;
            List<StatusEffect> statusEffects = statusEffectHandler.StatusEffects;
            int counter = 0;

            foreach (Transform children in transform)
            {
                counter += 1;

                if (counter > statusEffects.Count)
                {
                    children.gameObject.SetActive(false);
                }
                else
                {
                    if (children.TryGetComponent(out StatusEffectDisplaySlot statusEffectDisplaySlot))
                    {
                        statusEffectDisplaySlot.Icon = statusEffects[counter - 1].Data.Icon;
                        statusEffectDisplaySlot.UpdateStacksAmount(statusEffects[counter - 1].stacks);
                        children.gameObject.SetActive(true);
                    }
                    else
                    {
                        children.gameObject.SetActive(false);
                    }
                }
            }
        }

        if (abilityController != null)
        {
            TryUpdateRenderAbility(abilityController, AbilityType.Primary, windowInstance.TryGetElement<AbilityDisplaySlot>("Primary Ability"));
            TryUpdateRenderAbility(abilityController, AbilityType.Secondary, windowInstance.TryGetElement<AbilityDisplaySlot>("Secondary Ability"));
            TryUpdateRenderAbility(abilityController, AbilityType.Utility, windowInstance.TryGetElement<AbilityDisplaySlot>("Utility Ability"));
            TryUpdateRenderAbility(abilityController, AbilityType.Ultimate, windowInstance.TryGetElement<AbilityDisplaySlot>("Ultimate Ability"));
        }
    }

    private void TryUpdateRenderAbility(AbilityController abilityController, AbilityType abilityType, AbilityDisplaySlot abilityDisplaySlot)
    {
        if (abilityController.TryGetAbility(abilityType, out IAbility ability))
        {
            abilityDisplaySlot.gameObject.SetActive(true);
            UpdateAbilitySlot(abilityDisplaySlot, ability);
            SetAbilitySlotIcons(abilityDisplaySlot, ability.Data);
        }
        else
        {
            abilityDisplaySlot.gameObject.SetActive(false);
        }
    }

    private void UpdateAbilitySlot(AbilityDisplaySlot abilityDisplaySlot, IAbility ability)
    {
        if (ability.CooldownTime > 0 || ability.Charges > 0)
        {
            abilityDisplaySlot.UpdateUseState(ability.Charges, ability.DurationActive);
        }

        if (ability.DurationActive)
        {
            abilityDisplaySlot.UpdateDurationBar(ability.DurationPercentage);
            abilityDisplaySlot.UpdateCooldownBar(1f);
        }
        else
        {
            abilityDisplaySlot.UpdateDurationBar(0f);
            abilityDisplaySlot.UpdateCooldownBar(ability.ChargePercentage);
        }

        if (ability.MaxCharges > 1)
        {
            abilityDisplaySlot.UpdateChargesText(ability.Charges);
        }
        else
        {
            abilityDisplaySlot.ClearChargesText();
        }
    }

    private void SetAbilitySlotIcons(AbilityDisplaySlot abilityDisplaySlot, AbilityData abilityData)
    {
        abilityDisplaySlot.CooldowSprite = abilityData.Icon;
        abilityDisplaySlot.UsableSprite = abilityData.Icon;
        abilityDisplaySlot.Background = abilityData.Icon;
    }
}