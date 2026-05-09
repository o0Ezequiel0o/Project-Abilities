using System.Collections.Generic;
using UnityEngine;

public class DamageNumbersManager : Singleton<DamageNumbersManager>
{
    [Header("Settings")]
    [SerializeField] private DamageNumber damageNumberPrefab;
    [SerializeField] private int preWarmAmount = 10;
    [SerializeField] private float duration = 1.5f;

    [Header("Config")]
    [SerializeField] private DamageNumbersConfig config;

    public static DamageNumbersConfig Config => Instance.config;

    public static float Duration => Duration;

    private readonly Stack<DamageNumberData> inactiveDamageNumbers = new Stack<DamageNumberData>();
    private readonly List<DamageNumberData> activeDamageNumbers = new List<DamageNumberData>();

    private float currentTime = 0f;

    public static void DisplayDamageNumber(Vector3 position, GameObject receiver, float value, float size)
    {
        DisplayDamageNumber(position, receiver, value, size, Vector2.zero);
    }

    public static void DisplayHealNumber(Vector3 position, GameObject receiver, float value, float size)
    {
        DisplayHealNumber(position, receiver, value, size, Vector2.zero);
    }

    public static void DisplayDamageNumber(Vector3 position, GameObject receiver, float value, float size, Vector2 offset)
    {
        DisplayNumber(position, receiver, value, size, offset, Instance.config.DefaultDamageColor);
    }

    public static void DisplayHealNumber(Vector3 position, GameObject receiver, float value, float size, Vector2 offset)
    {
        DisplayNumber(position, receiver, value, size, offset, Instance.config.DefaultHealColor);
    }

    private static void DisplayNumber(Vector3 position, GameObject receiver, float value, float size, Vector2 offset, Color color)
    {
        DamageNumberData damageNumber;

        if (Instance.inactiveDamageNumbers.Count <= 0)
        {
            damageNumber = new DamageNumberData(CreateDamageNumber());

        }
        else
        {
            damageNumber = Instance.inactiveDamageNumbers.Pop();
        }

        InitializeDamageNumber(damageNumber.number, position, value, size, color);
        ActivateDamageNumber(damageNumber, receiver, offset);
    }

    private void Start()
    {
        for (int i = 0; i < preWarmAmount; i++)
        {
            DamageNumber damageNumber = CreateDamageNumber();
            damageNumber.gameObject.SetActive(false);

            inactiveDamageNumbers.Push(new DamageNumberData(damageNumber));
        }
    }

    private void Update()
    {
        currentTime += Time.deltaTime;
        UpdateAndRemoveActiveNumbers();
    }

    private static DamageNumber CreateDamageNumber()
    {
        return Instantiate(Instance.damageNumberPrefab, GameInstance.WorldCanvas.transform);
    }

    private static void ActivateDamageNumber(DamageNumberData damageNumberData, GameObject receiver, Vector2 offset)
    {
        damageNumberData.Initialize(receiver, Instance.currentTime, Instance.duration, offset);

        damageNumberData.number.gameObject.transform.SetAsLastSibling();
        damageNumberData.number.gameObject.SetActive(true);

        Instance.activeDamageNumbers.Add(damageNumberData);
    }

    private static void InitializeDamageNumber(DamageNumber damageNumber, Vector3 position, float damage, float size, Color color)
    {
        damageNumber.Initialize(damage, size, color);
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
                inactiveDamageNumbers.Push(damageNumberData);
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

        public DamageNumberData(DamageNumber number)
        {
            this.number = number;
        }

        public void Initialize(GameObject receiver, float spawnTime, float duration, Vector3 offset)
        {
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