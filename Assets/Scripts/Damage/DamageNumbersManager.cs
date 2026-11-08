using System.Collections.Generic;
using UnityEngine;

public class DamageNumbersManager : GameService<DamageNumbersManager>
{
    [Header("Settings")]
    [SerializeField] private DamageNumber damageNumberPrefab;
    [SerializeField] private int preWarmAmount = 10;
    [SerializeField] private float duration = 1.5f;

    [Header("Optimization")]
    [SerializeField, Min(1)] private int spreadFrames = 5;

    [Header("Config")]
    [SerializeField] private DamageNumbersConfig config;

    public DamageNumbersConfig Config => Instance.config;

    public float Duration => Duration;

    private readonly Stack<DamageNumberData> inactiveDamageNumbers = new Stack<DamageNumberData>();
    private readonly List<DamageNumberData> activeDamageNumbers = new List<DamageNumberData>();

    private Queue<InitializeDamageNumberData> pendingNumbers = new Queue<InitializeDamageNumberData>();

    private float currentTime = 0f;

    public void DisplayDamageNumber(Vector3 position, GameObject receiver, float value, float size)
    {
        DisplayDamageNumber(position, receiver, value, size, Vector2.zero);
    }

    public void DisplayHealNumber(Vector3 position, GameObject receiver, float value, float size)
    {
        DisplayHealNumber(position, receiver, value, size, Vector2.zero);
    }

    public void DisplayDamageNumber(Vector3 position, GameObject receiver, float value, float size, Vector2 offset)
    {
        DisplayNumber(position, receiver, value, size, offset, Instance.config.DefaultDamageColor);
    }

    public void DisplayHealNumber(Vector3 position, GameObject receiver, float value, float size, Vector2 offset)
    {
        DisplayNumber(position, receiver, value, size, offset, Instance.config.DefaultHealColor);
    }

    private void DisplayNumber(Vector3 position, GameObject receiver, float value, float size, Vector2 offset, Color color)
    {
        DamageNumberData damageNumberData;

        if (Instance.inactiveDamageNumbers.Count <= 0)
        {
            damageNumberData = new DamageNumberData(CreateDamageNumber());

        }
        else
        {
            damageNumberData = Instance.inactiveDamageNumbers.Pop();
        }

        pendingNumbers.Enqueue(new InitializeDamageNumberData(damageNumberData, receiver, value, size, offset, color, receiver.transform.position));
    }

    private void Start()
    {
        for (int i = 0; i < preWarmAmount; i++)
        {
            DamageNumber damageNumber = CreateDamageNumber();
            damageNumber.MeshRenderer.enabled = false;

            inactiveDamageNumbers.Push(new DamageNumberData(damageNumber));
        }
    }

    private void Update()
    {
        if (pendingNumbers.Count > 0)
        {
            int itemsToProcess = Mathf.CeilToInt((float)pendingNumbers.Count / spreadFrames);

            for (int i = 0; i < itemsToProcess && pendingNumbers.Count > 0; i++)
            {
                InitializeDamageNumberData data = pendingNumbers.Dequeue();

                Vector3 currentPosition = data.lastPosition;

                if (data.receiver != null)
                {
                    currentPosition = data.receiver.transform.position;
                }

                InitializeDamageNumber(data.damageNumberData.number, currentPosition, data.value, data.size, data.color);
                ActivateDamageNumber(data.damageNumberData, data.receiver, data.offset, currentPosition);
            }
        }

        currentTime += Time.deltaTime;
        UpdateAndRemoveActiveNumbers();
    }

    private DamageNumber CreateDamageNumber()
    {
        DamageNumber damageNumber = Instantiate(Instance.damageNumberPrefab, transform);
        damageNumber.MeshRenderer.enabled = false;

        return damageNumber;
    }

    private void ActivateDamageNumber(DamageNumberData damageNumberData, GameObject receiver, Vector2 offset, Vector3 lastPosition)
    {
        damageNumberData.Initialize(receiver, Instance.currentTime, Instance.duration, offset, lastPosition);

        damageNumberData.number.gameObject.transform.SetAsLastSibling();
        damageNumberData.number.MeshRenderer.enabled = true;

        Instance.activeDamageNumbers.Add(damageNumberData);
    }

    private void InitializeDamageNumber(DamageNumber damageNumber, Vector3 position, float damage, float size, Color color)
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
                damageNumberData.number.MeshRenderer.enabled = false;
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

    public readonly struct InitializeDamageNumberData
    {
        public readonly DamageNumberData damageNumberData;
        public readonly GameObject receiver;
        public readonly float value;
        public readonly float size;
        public readonly Vector2 offset;
        public readonly Color color;

        public readonly Vector3 lastPosition;

        public InitializeDamageNumberData(DamageNumberData damageNumberData, GameObject receiver, float value, float size, Vector2 offset, Color color, Vector3 lastPosition)
        {
            this.damageNumberData = damageNumberData;
            this.receiver = receiver;

            this.value = value;
            this.size = size;
            this.offset = offset;
            this.color = color;

            this.lastPosition = lastPosition;
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

        public void Initialize(GameObject receiver, float spawnTime, float duration, Vector3 offset, Vector3 lastPosition)
        {
            this.receiver = receiver;
            this.spawnTime = spawnTime;
            this.offset = offset;

            despawnTime = spawnTime + duration;
            this.lastPosition = lastPosition;
        }

        public void UpdateLastPosition()
        {
            lastPosition = receiver.transform.position;
        }
    }
}