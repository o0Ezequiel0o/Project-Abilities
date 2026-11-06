using UnityEngine;

public class SpriteFlashController : MonoBehaviour
{
    [SerializeField, HideInInspector] private SpriteRenderer[] spriteRenderers;

    private static readonly int flashAmountID = Shader.PropertyToID("_FlashAmount");

    private MaterialPropertyBlock propertyBlock;
    private float flashTimer = 0f;

    public void StartFlash(float flashAmount, float flashDuration)
    {
        flashTimer = flashDuration;
        SetFlashAmount(flashAmount);
    }

    private void Awake()
    {
        propertyBlock = new MaterialPropertyBlock();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (spriteRenderers == null || spriteRenderers.Length == 0)
        {
            spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
        }
    }
#endif

    private void Update()
    {
        if (flashTimer < 0f) return;
        UpdateFlashTimer();
    }

    private void UpdateFlashTimer()
    {
        flashTimer -= Time.deltaTime;

        if (flashTimer <= 0f)
        {
            StopFlash();
        }
    }

    private void StopFlash()
    {
        SetFlashAmount(0f);
    }

    private void SetFlashAmount(float amount)
    {
        propertyBlock.SetFloat(flashAmountID, amount);

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].SetPropertyBlock(propertyBlock);
        }
    }
}