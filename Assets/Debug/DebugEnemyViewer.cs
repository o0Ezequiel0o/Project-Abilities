using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine;
using Zeke.UI;
using TMPro;

public class DebugEnemyViewer : MonoBehaviour
{
    [SerializeField] private UIWindow window;
    [SerializeField] private LayerMask interactionLayers;
    [SerializeReference] private InputActionReference selectInput;

    private Camera mainCamera;

    private GameObject currentObj;
    private UIWindow windowInstance;

    private void Awake()
    {
        selectInput.action.started += OnClick;
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (windowInstance != null)
        {
            UpdateValues();
        }
    }

    private void OnClick(InputAction.CallbackContext context)
    {
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorldPosition.z = 0f;

        Collider2D hit = Physics2D.OverlapPoint(mouseWorldPosition, interactionLayers);

        if (hit != null)
        {
            currentObj = hit.gameObject;
        }

        if (windowInstance != null)
        {
            Destroy(windowInstance.gameObject);
        }

        windowInstance = Instantiate(window, GameInstance.ScreenCanvas.transform);

        UpdateValues();
    }

    private void UpdateValues()
    {
        if (currentObj == null) return;

        Damageable damageable = currentObj.GetComponent<Damageable>();
        EntityMove entityMove = currentObj.GetComponent<EntityMove>();
        ItemHandler itemHandler = currentObj.GetComponent<ItemHandler>();
        SpriteRenderer spriteRenderer = currentObj.GetComponent<SpriteRenderer>();
        StatusEffectHandler statusEffectHandler = currentObj.GetComponent<StatusEffectHandler>();

        windowInstance.TryGetElement<TextMeshProUGUI>("Name").text = currentObj.name;

        if (spriteRenderer != null)
        {
            windowInstance.TryGetElement<Image>("Image").sprite = spriteRenderer.sprite;
        }
        
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
    }
}