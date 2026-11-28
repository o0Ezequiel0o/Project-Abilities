using UnityEngine;

public abstract class PassiveData : ScriptableObject
{
    [field: Header("Display")]
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public string Name { get; private set; }

    [field: SerializeField, TextArea(3, 3)] public string Description { get; private set; }

    public abstract IPassive CreatePassive(GameObject source, PassiveController passiveController);
}