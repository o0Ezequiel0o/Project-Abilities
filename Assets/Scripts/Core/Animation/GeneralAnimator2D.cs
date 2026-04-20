using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

public static class GeneralAnimator2D
{
    public static Coroutine FadeOut(MonoBehaviour monoBehaviour, SpriteRenderer[] renderers, float duration)
    {
        return FadeOut(monoBehaviour, renderers, duration, null);
    }

    public static Coroutine FadeOut(MonoBehaviour monoBehaviour, List<SpriteRenderer> renderers, float duration)
    {
        return FadeOut(monoBehaviour, renderers, duration, null);
    }

    public static Coroutine FadeOut(MonoBehaviour monoBehaviour, SpriteRenderer[] renderers, float duration, Action finishCallback)
    {
        List<SpriteData> spritesData = GenerateSpriteData(renderers);
        return monoBehaviour.StartCoroutine(FadeOut(spritesData, duration, finishCallback));
    }

    public static Coroutine FadeOut(MonoBehaviour monoBehaviour, List<SpriteRenderer> renderers, float duration, Action finishCallback)
    {
        List<SpriteData> spritesData = GenerateSpriteData(renderers);
        return monoBehaviour.StartCoroutine(FadeOut(spritesData, duration, finishCallback));
    }

    private static IEnumerator FadeOut(List<SpriteData> spritesData, float duration, Action finishCallback)
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            for (int i = 0; i < spritesData.Count; i++)
            {
                SetSpriteDataAlpha(spritesData[i], Mathf.Lerp(spritesData[i].startColor.a, 0f, timer / duration));
            }

            yield return null;
        }

        finishCallback?.Invoke();
    }

    public static Coroutine FadeIn(MonoBehaviour monoBehaviour, SpriteRenderer[] renderers, float duration)
    {
        return FadeIn(monoBehaviour, renderers, duration, null);
    }

    public static Coroutine FadeIn(MonoBehaviour monoBehaviour, List<SpriteRenderer> renderers, float duration)
    {
        return FadeIn(monoBehaviour, renderers, duration, null);
    }

    public static Coroutine FadeIn(MonoBehaviour monoBehaviour, SpriteRenderer[] renderers, float duration, Action finishCallback)
    {
        List<SpriteData> spritesData = GenerateSpriteData(renderers);
        return monoBehaviour.StartCoroutine(FadeIn(spritesData, duration, finishCallback));
    }

    public static Coroutine FadeIn(MonoBehaviour monoBehaviour, List<SpriteRenderer> renderers, float duration, Action finishCallback)
    {
        List<SpriteData> spritesData = GenerateSpriteData(renderers);
        return monoBehaviour.StartCoroutine(FadeIn(spritesData, duration, finishCallback));
    }

    private static IEnumerator FadeIn(List<SpriteData> spritesData, float duration, Action finishCallback)
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            for (int i = 0; i < spritesData.Count; i++)
            {
                SetSpriteDataAlpha(spritesData[i], Mathf.Lerp(0f, spritesData[i].startColor.a, timer / duration));
            }

            yield return null;
        }

        finishCallback?.Invoke();
    }

    public static Coroutine Shrink(MonoBehaviour monoBehaviour, Transform root, Vector3 originalSize, float desiredRelativeSize, float duration)
    {
        return Shrink(monoBehaviour, root, originalSize, desiredRelativeSize, duration, null);
    }

    public static Coroutine Shrink(MonoBehaviour monoBehaviour, Transform root, Vector3 originalSize, float desiredRelativeSize, float duration, Action finishCallback)
    {
        return monoBehaviour.StartCoroutine(Shrink(root, originalSize, desiredRelativeSize, duration, finishCallback));
    }

    private static IEnumerator Shrink(Transform root, Vector3 originalSize, float desiredRelativeSize, float duration, Action finishCallback)
    {
        float timer = 0f;

        float xScale = 0f;
        float yScale = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            xScale = Mathf.Lerp(originalSize.x, originalSize.x * desiredRelativeSize, timer / duration);
            yScale = Mathf.Lerp(originalSize.y, originalSize.y * desiredRelativeSize, timer / duration);

            root.localScale = new Vector3(xScale, yScale, originalSize.z);

            yield return null;
        }

        finishCallback?.Invoke();
    }

    private static void SetSpriteDataAlpha(SpriteData spriteData, float alpha)
    {
        Color newColor = spriteData.spriteRenderer.color;
        newColor.a = alpha;

        spriteData.spriteRenderer.color = newColor;
    }

    private static List<SpriteData> GenerateSpriteData(List<SpriteRenderer> renderers)
    {
        List<SpriteData> spritesData = new List<SpriteData>();

        for (int i = 0; i < renderers.Count; i++)
        {
            spritesData.Add(new SpriteData(renderers[i], renderers[i].color));
        }

        return spritesData;
    }

    private static List<SpriteData> GenerateSpriteData(SpriteRenderer[] renderers)
    {
        List<SpriteData> spritesData = new List<SpriteData>();

        for (int i = 0; i < renderers.Length; i++)
        {
            spritesData.Add(new SpriteData(renderers[i], renderers[i].color));
        }

        return spritesData;
    }

    private struct SpriteData
    {
        public SpriteRenderer spriteRenderer;
        public Color startColor;

        public SpriteData(SpriteRenderer spriteRenderer, Color startColor)
        {
            this.spriteRenderer = spriteRenderer;
            this.startColor = startColor;
        }
    }
}