using UnityEngine;

[CreateAssetMenu(fileName = "Command Test", menuName = "ScriptableObjects/CheatMenuCommands/CommandTest", order = 1)]
public class CheatMenuCommand : ScriptableObject
{
    [field: SerializeField] public string CommandName { get; private set; }

    public void Execute()
    {
        Debug.Log("print this idk");
    }
}