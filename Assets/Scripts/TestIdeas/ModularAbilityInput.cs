using UnityEngine;

public class ModularAbilityInput : MonoBehaviour
{
    [SerializeField] private ModularAbilityController controller;
    [SerializeField] private AbilityType abilityType;
    [SerializeField] private KeyCode keyCode;

    private void Update()
    {
        if (Input.GetKeyDown(keyCode))
        {
            controller.TryUseAbility(abilityType, false);
        }
        else if (Input.GetKey(keyCode))
        {
            controller.TryUseAbility(abilityType, true);
        }
    }
}