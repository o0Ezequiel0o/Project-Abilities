using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Zeke.UI;
using System;

public class CharacterSelector : MonoBehaviour
{
    [Tooltip("Where the selected spawnable will be saved (for persistent data)")]
    [SerializeField] private Spawnable selectedSave;

    [Space]

    [SerializeField] private UIWindow optionPrefab;
    [SerializeField] private RectTransform root;

    [Space]

    [SerializeField] private List<Spawnable> spawnables;

    public Spawnable SelectedSpawnable { get; private set; }

    public Action<Spawnable> onSpawnableSelected;

    private void Awake()
    {
        for (int i = 0; i < spawnables.Count; i++)
        {
            UIWindow window = Instantiate(optionPrefab, root);

            Spawnable spawnable = spawnables[i];
            window.TryGetElement<Button>("Button").onClick.AddListener(() => OnSpawnableSelected(spawnable));
        }
    }

    private void Start()
    {
        if (SelectedSpawnable == null && spawnables.Count > 0)
        {
            OnSpawnableSelected(spawnables[0]);
        }
    }

    private void OnSpawnableSelected(Spawnable spawnable)
    {
        SelectedSpawnable = spawnable;
        selectedSave.LoadSpawnable(spawnable);
        onSpawnableSelected?.Invoke(spawnable);
    }
}