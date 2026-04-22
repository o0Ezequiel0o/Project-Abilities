using UnityEngine;

[DefaultExecutionOrder(-1)]
public abstract class Singleton<T> : MonoBehaviour where T : Component
{
    public static bool IsNull => instance == null;

    private static T instance;
    protected static T Instance
    {
        get
        {
            if (instance != null) return instance;

            instance = FindAnyObjectByType<T>();
            InitializeSingleton(instance);

            if (instance != null) return instance;

            return CreateSingletonGameObject();
        }
    }

    private static T CreateSingletonGameObject()
    {
        GameObject newGameObject = new GameObject(typeof(T).Name + " [Auto-Generated]");
        instance = newGameObject.AddComponent<T>();
        InitializeSingleton(instance);

        return instance;
    }

    protected virtual void OnInitialization() { }

    private static void InitializeSingleton(T instance)
    {
        if (instance is Singleton<T> singleton)
        {
            singleton.InitializeSingleton();
        }
    }

    private void InitializeSingleton()
    {
        if (!Application.isPlaying) return;

        Debug.Log("shit worked");

        instance = this as T;
        OnInitialization();
    }
}