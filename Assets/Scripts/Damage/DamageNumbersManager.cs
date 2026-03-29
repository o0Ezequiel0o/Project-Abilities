using System.Collections.Generic;
using UnityEngine;
using Zeke.PoolableGameObjects;

public class DamageNumbersManager : Singleton<DamageNumbersManager>
{
    [Header("Settings")]
    [SerializeField] private GameObject damageNumberPrefab;
    [SerializeField] private int preWarmAmount = 10;
    [SerializeField] private float duration = 1.5f;

    [Header("Config")]
    [SerializeField] private DamageNumbersConfig config;

    public static DamageNumbersConfig Config => Instance.config;

    public static float Duration => Duration;

    private readonly GameObjectPool<DamageNumber> damageNumbersPool = new GameObjectPool<DamageNumber>();
    private readonly List<ActiveDamageNumberData> activeDamageNumbers = new List<ActiveDamageNumberData>();

    private Canvas worldCanvas;

    private float currentTime = 0f;

    public static void DisplayDamageNumber(Vector3 position, GameObject receiver, float value, float size)
    {
        DisplayDamageNumber(position, receiver, value, size, Vector2.zero);
    }

    public static void DisplayDamageNumber(Vector3 position, GameObject receiver, float value, float size, Vector2 offset)
    {
        DamageNumber damageNumber = GetDamageNumber();
        InitializeDamageNumber(position, damageNumber, value, size);
        ActivateDamageNumber(damageNumber, receiver, offset);
    }

    void Start()
    {
        worldCanvas = GameInstance.WorldCanvas;

        for (int i = 0; i < preWarmAmount; i++)
        {
            GetDamageNumber().gameObject.SetActive(false);
        }
    }

    void Update()
    {
        currentTime += Time.deltaTime;
        UpdateAndRemoveActiveNumbers();
    }

    static DamageNumber GetDamageNumber()
    {
        return Instance.damageNumbersPool.Get(Instance.damageNumberPrefab, Instance.worldCanvas.transform);
    }

    static void ActivateDamageNumber(DamageNumber damageNumber, GameObject receiver, Vector2 offset)
    {
        Instance.activeDamageNumbers.Add(new ActiveDamageNumberData(damageNumber, receiver, Instance.currentTime, Instance.duration, offset));
        damageNumber.gameObject.transform.SetAsLastSibling();
        damageNumber.gameObject.SetActive(true);
    }

    static void InitializeDamageNumber(Vector3 position, DamageNumber damageNumber, float damage, float size)
    {
        damageNumber.Initialize(damage, size, Instance.config.DefaultColor);
        damageNumber.UpdateAlpha(Instance.config.StartAlpha);
        damageNumber.transform.position = position;
    }

    void UpdateAndRemoveActiveNumbers()
    {
        for (int i = activeDamageNumbers.Count - 1; i >= 0; i--)
        {
            UpdateActiveDamageNumberAlpha(activeDamageNumbers[i]);

            if (activeDamageNumbers[i].receiver != null)
            {
                Vector3 position = GetTargetPosition(activeDamageNumbers[i]);
                position.y += FloatUpwards(activeDamageNumbers[i].spawnTime);

                activeDamageNumbers[i].number.transform.position = position + activeDamageNumbers[i].offset;
            }

            if (currentTime > activeDamageNumbers[i].despawnTime)
            {
                activeDamageNumbers[i].number.gameObject.SetActive(false);
                activeDamageNumbers.RemoveAt(i);
            }
        }
    }

    private float FloatUpwards(float spawnTime)
    {
        float timeSinceStarted = (currentTime - spawnTime);
        float maxHeight = Instance.config.FloatSpeed * Instance.duration;

        float currentFloatHeight = Mathf.Lerp(0f, maxHeight, timeSinceStarted / Instance.duration);

        return currentFloatHeight;
    }

    private Vector3 GetTargetPosition(ActiveDamageNumberData data)
    {
        return data.receiver.transform.position;
    }

    private void UpdateActiveDamageNumberAlpha(ActiveDamageNumberData data)
    {
        float alphaStartTime = Mathf.Lerp(data.spawnTime, data.despawnTime, config.AlphaStartTime);

        if (currentTime > alphaStartTime)
        {
            float alphaPercent = Mathf.InverseLerp(alphaStartTime, data.despawnTime, currentTime);
            float alpha = Mathf.Lerp(config.StartAlpha, 0, alphaPercent);

            data.number.UpdateAlpha(alpha);
        }
    }

    public readonly struct ActiveDamageNumberData
    {
        public readonly DamageNumber number;
        public readonly GameObject receiver;


        public readonly float spawnTime;
        public readonly float despawnTime;

        public readonly Vector3 offset;

        public ActiveDamageNumberData(DamageNumber number, GameObject receiver, float spawnTime, float duration, Vector3 offset)
        {
            this.number = number;
            this.receiver = receiver;
            this.spawnTime = spawnTime;

            this.offset = offset;

            despawnTime = spawnTime + duration;
        }
    }
}