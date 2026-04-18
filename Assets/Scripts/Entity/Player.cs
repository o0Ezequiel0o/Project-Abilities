using UnityEngine;

public class Player : MonoBehaviour
{
    private void OnEnable()
    {
        GameInstance.AddPlayer(this);
    }

    private void OnDisable()
    {
        GameInstance.RemovePlayer(this);
    }
}