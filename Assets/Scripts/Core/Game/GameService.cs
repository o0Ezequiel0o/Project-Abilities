using UnityEngine;

public abstract class GameService<T> : MonoBehaviour where T : GameService<T>
{
    public static bool IsNull => instance == null || isQuitting;

    private static bool isQuitting = false;

    public static T Instance
    {
        get
        {
            if (isQuitting) return null;
            if (instance != null) return instance;

            instance = FindAnyObjectByType<T>();
            instance?.InitializeService();

            return instance;
        }
    }

    private static T instance;

    protected virtual void InitializeService() { }

    protected virtual void OnApplicationQuit()
    {
        isQuitting = true;
    }
}