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
    private readonly Stack<DamageNumberData> unactiveDamageNumbers = new Stack<DamageNumberData>();
    private readonly List<DamageNumberData> activeDamageNumbers = new List<DamageNumberData>();

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

    private void Start()
    {
        worldCanvas = GameInstance.WorldCanvas;

        for (int i = 0; i < preWarmAmount; i++)
        {
            GetDamageNumber().gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        currentTime += Time.deltaTime;
        UpdateAndRemoveActiveNumbers();
    }

    private static DamageNumber GetDamageNumber()
    {
        return Instance.damageNumbersPool.Get(Instance.damageNumberPrefab, Instance.worldCanvas.transform);
    }

    private static void ActivateDamageNumber(DamageNumber damageNumber, GameObject receiver, Vector2 offset)
    {
        DamageNumberData damageNumberData;

        if (Instance.unactiveDamageNumbers.Count <= 0)
        {
            damageNumberData = new DamageNumberData(damageNumber, receiver, Instance.currentTime, Instance.duration, offset);
        }
        else
        {
            damageNumberData = Instance.unactiveDamageNumbers.Pop();
            damageNumberData.Initialize(damageNumber, receiver, Instance.currentTime, Instance.duration, offset);
        }

        Instance.activeDamageNumbers.Add(damageNumberData);

        damageNumber.gameObject.transform.SetAsLastSibling();
        damageNumber.gameObject.SetActive(true);
    }

    private static void InitializeDamageNumber(Vector3 position, DamageNumber damageNumber, float damage, float size)
    {
        damageNumber.Initialize(damage, size, Instance.config.DefaultColor);
        damageNumber.UpdateAlpha(Instance.config.StartAlpha);
        damageNumber.transform.position = position;
    }

    private void UpdateAndRemoveActiveNumbers()
    {
        for (int i = activeDamageNumbers.Count - 1; i >= 0; i--)
        {
            DamageNumberData damageNumberData = activeDamageNumbers[i];

            UpdateActiveDamageNumberAlpha(damageNumberData);

            if (damageNumberData.receiver != null)
            {
                damageNumberData.UpdateLastPosition();
            }

            Vector3 newPosition = damageNumberData.lastPosition;
            Vector3 newOffset = damageNumberData.offset;
            newOffset.y += Instance.config.FloatSpeed * Time.deltaTime;

            damageNumberData.offset = newOffset;
            newPosition += damageNumberData.offset;

            damageNumberData.number.transform.position = newPosition;

            if (currentTime > damageNumberData.despawnTime)
            {
                damageNumberData.number.gameObject.SetActive(false);
                unactiveDamageNumbers.Push(damageNumberData);
                activeDamageNumbers.RemoveAt(i);
            }
        }
    }

    private void UpdateActiveDamageNumberAlpha(DamageNumberData data)
    {
        float alphaStartTime = Mathf.Lerp(data.spawnTime, data.despawnTime, config.AlphaStartTime);

        if (currentTime > alphaStartTime)
        {
            float alphaPercent = Mathf.InverseLerp(alphaStartTime, data.despawnTime, currentTime);
            float alpha = Mathf.Lerp(config.StartAlpha, 0, alphaPercent);

            data.number.UpdateAlpha(alpha);
        }
    }

    public class DamageNumberData
    {
        public DamageNumber number;
        public GameObject receiver;

        public Vector3 lastPosition;

        public float spawnTime;
        public float despawnTime;

        public Vector3 offset;

        public DamageNumberData(DamageNumber number, GameObject receiver, float spawnTime, float duration, Vector3 offset)
        {
            Initialize(number, receiver, spawnTime, duration, offset);
        }

        public void Initialize(DamageNumber number, GameObject receiver, float spawnTime, float duration, Vector3 offset)
        {
            this.number = number;
            this.receiver = receiver;
            this.spawnTime = spawnTime;
            this.offset = offset;

            despawnTime = spawnTime + duration;
            lastPosition = receiver.transform.position;
        }

        public void UpdateLastPosition()
        {
            lastPosition = receiver.transform.position;
        }
    }
}