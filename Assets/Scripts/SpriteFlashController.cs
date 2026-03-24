using System.Collections.Generic;
using UnityEngine;

public class SpriteFlashController : MonoBehaviour
{
    private static readonly int flashAmountID = Shader.PropertyToID("_FlashAmount");

    private Material[] materials;
    private float flashTimer = 0f;

    public void StartFlash(float flashAmount, float flashDuration)
    {
        flashTimer = flashDuration;
        SetFlashAmount(flashAmount);
    }

    void Awake()
    {
        SpriteRenderer[] spriteRenderers = GetSpriteRenderers();
        GetMaterialsFromSpriteRenderers(spriteRenderers);
    }

    void Update()
    {
        if (flashTimer < 0f) return;
        UpdateFlashTimer();
    }

    SpriteRenderer[] GetSpriteRenderers()
    {
        return GetComponentsInChildren<SpriteRenderer>();
    }

    void GetMaterialsFromSpriteRenderers(SpriteRenderer[] spriteRenderers)
    {
        List<Material> materials = new List<Material>();

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i].material.HasProperty(flashAmountID))
            {
                materials.Add(spriteRenderers[i].material);
            }
        }

        this.materials = materials.ToArray();
    }

    void UpdateFlashTimer()
    {
        flashTimer -= Time.deltaTime;

        if (flashTimer <= 0)
        {
            StopFlash();
        }
    }

    void StopFlash()
    {
        SetFlashAmount(0f);
    }

    void SetFlashAmount(float amount)
    {
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].SetFloat(flashAmountID, amount);
        }
    }
}