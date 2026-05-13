using System.Collections.Generic;
using UnityEngine;
using Zeke.Items;

public class HackItemGeneratorsSkill : PassiveBase
{
    public override PassiveData Data => data;
    private readonly HackItemGeneratorsSkillData data;

    private readonly GameObject source;
    private readonly Stat chargeSpeed;

    private Collider2D currentItemGeneratorCollider;
    private ItemGenerator currentItemGenerator;
    private StatusBar progressBarInstance;

    private float hackTimer = 0f;

    private readonly List<RaycastHit2D> hits = new List<RaycastHit2D>(8);

    public HackItemGeneratorsSkill(GameObject source, PassiveController passiveController, HackItemGeneratorsSkillData data, Stat chargeSpeed) : base(passiveController)
    {
        this.source = source;
        this.data = data;

        this.chargeSpeed = chargeSpeed;
    }

    public override void Awake()
    {
        progressBarInstance = GameObject.Instantiate(data.ProgressBarPrefab, GameInstance.WorldCanvas.transform);
        progressBarInstance.gameObject.SetActive(false);
    }

    public override void Update()
    {
        if (currentItemGenerator != null && currentItemGenerator.CanHack(source))
        {
            UpdateHacking();
            UpdateProgressBar();
        }
        else
        {
            if (progressBarInstance.gameObject.activeSelf)
            {
                progressBarInstance.gameObject.SetActive(false);
            }

            FindItemGenerator();
        }
    }

    private void FindItemGenerator()
    {
        ContactFilter2D contactFilter = new ContactFilter2D() { layerMask = data.CheckLayers, useLayerMask = true };
        for (int i = 0; i < Physics2D.CircleCast(source.transform.position, data.Radius, Vector2.zero, contactFilter, hits, 0f); i++)
        {
            if (hits[i].transform.TryGetComponent(out ItemGenerator itemGenerator) && itemGenerator.CanHack(source))
            {
                currentItemGeneratorCollider = itemGenerator.GetComponent<Collider2D>();
                progressBarInstance.transform.position = itemGenerator.transform.position;
                progressBarInstance.gameObject.SetActive(true);
                currentItemGenerator = itemGenerator;
                break;
            }
        }
    }

    private void UpdateHacking()
    {
        bool canStillHackItemGenerator = true;

        if (CurrentItemGeneratorInRange())
        {
            hackTimer += Time.deltaTime * chargeSpeed.Value;

            if (hackTimer > data.TimeRequired)
            {
                currentItemGenerator.Hack(source);
                hackTimer = 0f;
            }

            if (!currentItemGenerator.CanHack(source))
            {
                canStillHackItemGenerator = false;
            }
        }
        else
        {
            canStillHackItemGenerator = false;
        }

        if (!canStillHackItemGenerator)
        {
            hackTimer = 0f;
            currentItemGenerator = null;
            currentItemGeneratorCollider = null;
            progressBarInstance.gameObject.SetActive(false);
        }
    }

    private void UpdateProgressBar()
    {
        progressBarInstance.UpdateBar(hackTimer, data.TimeRequired);
    }

    private bool CurrentItemGeneratorInRange()
    {
        Vector2 closestPoint;

        if (currentItemGeneratorCollider != null)
        {
            closestPoint = currentItemGeneratorCollider.ClosestPoint(source.transform.position);
        }
        else
        {
            closestPoint = currentItemGenerator.transform.position;
        }

        return Vector2.Distance(closestPoint, source.transform.position) <= data.Radius;
    }

    public override void OnRemove()
    {
        if (progressBarInstance != null)
        {
            GameObject.Destroy(progressBarInstance.gameObject);
        }
    }

    protected override void UpgradeInternal()
    {
        chargeSpeed.Upgrade();
    }
}