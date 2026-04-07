using UnityEngine;

public class LevelInitiator : MonoBehaviour
{
    [SerializeField] private GameObject level;

    private GameObject currentLevel;

    private void Start()
    {
        LoadLevel();
    }

    private void LoadLevel()
    {
        currentLevel = Instantiate(level);
    }
}