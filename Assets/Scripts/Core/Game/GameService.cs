using UnityEngine;

public abstract class GameService<T> : MonoBehaviour where T : GameService<T>
{
    public static bool IsNull => instance == null;

    public static T Instance
    {
        get
        {
            if (instance != null) return instance;

            instance = FindAnyObjectByType<T>();
            instance?.InitializeService();

            return instance;
        }
    }

    private static T instance;

    protected virtual void InitializeService() { }
}