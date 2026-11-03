using PrimeTween;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Zeke.UI;
using TMPro;

namespace Zeke.Abilities.Ascendancy
{
    [RequireComponent(typeof(AscendancyHandler))]
    public class AscendancyHandlerScreenRenderer : MonoBehaviour
    {
        [Header("Visual - UI")]
        [SerializeField] private UIWindow optionsWindow;
        [SerializeField] private UIWindow optionWindow;

        [Header("Animation")]
        [SerializeField] private float optionAppearDuration = 0.2f;

        private readonly Queue<AscendancyHandler.AscendanceElementInfo> pending = new Queue<AscendancyHandler.AscendanceElementInfo>();
        private readonly HashSet<AbilityData> pendingHash = new HashSet<AbilityData>();

        private readonly List<UIWindow> spawnedOptionWindows = new List<UIWindow>();

        private UIWindow windowInstance;

        private AscendancyHandler ascendancyHandler;

        private bool interactionEnabled = true;

        private readonly GameInstance.PauseID pauseID = new();

        private void Awake()
        {
            ascendancyHandler = GetComponent<AscendancyHandler>();
            ascendancyHandler.onAscendancyReady += OnAscendancyReady;
        }

        private void OnDestroy()
        {
            ascendancyHandler.onAscendancyReady -= OnAscendancyReady;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                gameObject.GetComponent<LevelHandler>().GiveExperience(10000);
            }
        }

        private void CreateOptionsWindow()
        {
            windowInstance = Instantiate(optionsWindow, GameInstance.ScreenCanvas.transform);
            AscendancyHandler.AscendanceElementInfo info = pending.Dequeue();
            pendingHash.Remove(info.key);

            CreateOriginalElement(info);
            PopulateOptionsWindow(info);

            GameInstance.Pause(pauseID);

            interactionEnabled = true;
        }

        private void CreateOriginalElement(AscendancyHandler.AscendanceElementInfo info)
        {
            RectTransform root = windowInstance.TryGetElement<RectTransform>("Original Root");

            UIWindow originalWindowInstance = Instantiate(optionWindow, root);

            originalWindowInstance.TryGetElement<Image>("Image").sprite = info.key.Icon;
            originalWindowInstance.TryGetElement<TextMeshProUGUI>("Name").text = info.key.Name;
            originalWindowInstance.TryGetElement<TextMeshProUGUI>("Description").text = info.key.Description;
        }

        private void CloseOptionsWindow()
        {
            Destroy(windowInstance.gameObject);
            spawnedOptionWindows.Clear();
            GameInstance.Unpause(pauseID);
        }

        private void OnCloseAnimationFinished()
        {
            CloseOptionsWindow();

            if (pending.Count > 0)
            {
                CreateOptionsWindow();
            }
        }

        private void OnAscendancyReady(AscendancyHandler.AscendanceElementInfo element)
        {
            if (pendingHash.Contains(element.key)) return;

            pending.Enqueue(element);
            pendingHash.Add(element.key);

            if (windowInstance == null)
            {
                CreateOptionsWindow();
            }
        }

        private void PopulateOptionsWindow(AscendancyHandler.AscendanceElementInfo elementsInfo)
        {
            LayoutGroup layoutGroupRoot = windowInstance.TryGetElement<LayoutGroup>("Layout Group");

            for (int i = 0; i < elementsInfo.element.Options.Count; i++)
            {
                UIWindow window = CreateOptionUIWindow(layoutGroupRoot.transform, elementsInfo.element.Options[i], elementsInfo.key);
                PlayOptionAppearAnimation(window.gameObject, optionAppearDuration * i);
                spawnedOptionWindows.Add(window);
            }
        }

        private UIWindow CreateOptionUIWindow(Transform root, AscendancyData option, AbilityData key)
        {
            UIWindow optionWindowInstance = Instantiate(optionWindow, root);

            optionWindowInstance.TryGetElement<Image>("Image").sprite = option.Icon;
            optionWindowInstance.TryGetElement<TextMeshProUGUI>("Name").text = option.Title;
            optionWindowInstance.TryGetElement<TextMeshProUGUI>("Description").text = option.Description;

            Button button = optionWindowInstance.TryGetElement<Button>("Clickbox");
            button.onClick.AddListener(() => OnOptionSelected(key, option, optionWindowInstance));

            return optionWindowInstance;
        }

        private void OnOptionSelected(AbilityData key, AscendancyData option, UIWindow selectedWindow)
        {
            if (!interactionEnabled) return;

            interactionEnabled = false;

            ascendancyHandler.ApplyAscension(key, option);

            for (int i = 0; i < spawnedOptionWindows.Count; i++)
            {
                UIWindow window = spawnedOptionWindows[i];

                if (window == null) continue;

                if (window == selectedWindow)
                {
                    PlayOptionSelectAnimation(window.gameObject, () =>
                    {
                        PlayOptionDisappearAnimation(window.gameObject);
                        PlayMenuPopOutAnimation(windowInstance.gameObject, OnCloseAnimationFinished);
                    });
                }
                else
                {
                    PlayOptionDisappearAnimation(window.gameObject);
                }
            }

            ascendancyHandler.CheckForReadyAscendancies();
        }

        private void PlayOptionAppearAnimation(GameObject window, float delay)
        {
            RectTransform rectTransform = window.GetComponent<RectTransform>();
            CanvasGroup canvasGroup = window.GetComponent<CanvasGroup>();

            rectTransform.localScale = Vector3.one * 0.1f;
            canvasGroup.alpha = 0f;

            Sequence.Create(useUnscaledTime: true)
                .Group(Tween.Scale(rectTransform, startValue: Vector3.one * 0.1f, endValue: Vector3.one, duration: optionAppearDuration, ease: Ease.OutBack, startDelay: delay))
                .Group(Tween.Alpha(canvasGroup, startValue: 0f, endValue: 1f, duration: 0.15f, ease: Ease.OutSine, startDelay: delay));
        }

        private void PlayOptionDisappearAnimation(GameObject window)
        {
            RectTransform rectTransform = window.GetComponent<RectTransform>();
            CanvasGroup canvasGroup = window.GetComponent<CanvasGroup>();

            Sequence.Create(useUnscaledTime: true)
                .Group(Tween.Scale(rectTransform, startValue: rectTransform.localScale, endValue: Vector3.zero, duration: 0.3f, ease: Ease.InBack))
                .Group(Tween.Alpha(canvasGroup, startValue: canvasGroup.alpha, endValue: 0f, duration: 0.3f, ease: Ease.InSine));
        }

        private void PlayMenuPopOutAnimation(GameObject window, Action onComplete)
        {
            RectTransform rectTransform = window.GetComponent<RectTransform>();
            CanvasGroup canvasGroup = window.GetComponent<CanvasGroup>();

            Sequence.Create(useUnscaledTime: true)
                .Group(Tween.Scale(rectTransform, startValue: rectTransform.localScale, endValue: rectTransform.localScale * 1.05f, duration: 0.15f, ease: Ease.OutQuad))

                .Chain(Tween.Scale(rectTransform, startValue: rectTransform.localScale * 1.05f, endValue: Vector3.zero, duration: 0.2f, ease: Ease.InBack))
                .Group(Tween.Alpha(canvasGroup, startValue: canvasGroup.alpha, endValue: 0f, duration: 0.15f, ease: Ease.InSine))

                .ChainCallback(() => onComplete?.Invoke());
        }

        private void PlayOptionSelectAnimation(GameObject window, Action onAnimationFinish)
        {
            RectTransform rectTransform = window.GetComponent<RectTransform>();
            CanvasGroup canvasGroup = window.GetComponent<CanvasGroup>();

            Sequence.Create(useUnscaledTime: true)
                .Group(Tween.Scale(rectTransform, startValue: rectTransform.localScale, endValue: Vector3.one * 1.15f, duration: 0.25f, ease: Ease.OutBack))
                .Group(Tween.Alpha(canvasGroup, startValue: canvasGroup.alpha, endValue: 1f, duration: 0.2f))

                .ChainDelay(0.3f)

                .ChainCallback(() => onAnimationFinish?.Invoke());
        }
    }
}