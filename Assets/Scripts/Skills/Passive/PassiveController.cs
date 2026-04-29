using System.Collections.Generic;
using UnityEngine;

public class PassiveController : MonoBehaviour, IUpgradable
{
    [SerializeField] private List<PassiveData> setPassives;

    public List<PassiveData> DefaultPassives => setPassives;

    private readonly List<IPassive> passives = new List<IPassive>();

    public void AddPassive(PassiveData passiveData)
    {
        if (HasPassive(passiveData)) return;

        IPassive passive = passiveData.CreatePassive(gameObject, this);

        passive.Initialize();
        passives.Add(passive);
    }

    public bool TryGetPassive(PassiveData passiveData, out IPassive passive)
    {
        passive = null;

        for (int i = 0; i < passives.Count; i++)
        {
            if (passives[i].Data == passiveData)
            {
                passive = passives[i];
                return true;
            }
        }

        return false;
    }

    public void Upgrade()
    {
        for (int i = 0; i < passives.Count; i++)
        {
            passives[i].Upgrade();
        }
    }

    public void UpgradePassive(PassiveData passiveData)
    {
        for (int i = 0; i < passives.Count; i++)
        {
            if (passives[i].Data == passiveData)
            {
                passives[i].Upgrade();
                return;
            }
        }
    }

    private void Awake()
    {
        for (int i = 0; i < setPassives.Count; i++)
        {
            AddPassive(setPassives[i]);
        }
    }

    private void Update()
    {
        UpdatePassives();
    }

    private void OnDestroy()
    {
        RemovePassives();
    }

    private void UpdatePassives()
    {
        for (int i = 0; i < passives.Count; i++)
        {
            passives[i].Update();
        }
    }

    private void RemovePassives()
    {
        for (int i = passives.Count - 1; i >= 0; i--)
        {
            passives[i].OnRemove();
            passives.RemoveAt(i);
        }
    }

    private bool HasPassive(PassiveData passiveData)
    {
        for (int i = 0; i < passives.Count; i++)
        {
            if (passives[i].Data == passiveData)
            {
                return true;
            }
        }

        return false;
    }
}