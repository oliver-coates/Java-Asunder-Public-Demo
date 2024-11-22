using System;
using System.Collections;
using System.Collections.Generic;
using Ships;
using UnityEngine;

public class DamageControlInstance : BoardPiece, IShipComponentInstance
{
    private const float TIME_TO_REPAIR = 10;
    private const float CHECK_FOR_DAMAGED_SECTION_TIME_INTERVAL = 2.5f;
    private const float REPAIR_STRENGTH_RANDOM_VARIANCE = 0.4f;

    private ShipInstance _ship;
    private ComponentSlot _componentSlot;
    private DamageControlType _type;
    private ShipSection[] _sections;

    private float _internalTimer;

    private float _repairEffectivenessRandomMultiplier; 


    private ShipSection _sectionToTarget;
    public ShipSection sectionToTarget
    {
        get
        {
            return _sectionToTarget;
        }
    }
    private DamageInstance _damageToTarget;
    public DamageInstance damageToTarget
    {
        get
        {
            return _damageToTarget;
        }
    }

    [Header("Stats:")]
    [SerializeField] private float _baseFireEffectivness;
    [SerializeField] private float _baseFloodingEffectivness;

    public void Setup(ShipInstance ship, ComponentSlot componentSlot)
    {
        _ship = ship;
        _sections = ship.sections;
        _componentSlot = componentSlot;

        name = componentSlot.slotName;

        _ship.damageControls.Add(this);

        // Setup stats from this damage control type
        _type = (DamageControlType) componentSlot.component;

        _baseFireEffectivness = Mathf.Lerp(Config.Ship.damageControlFireMinEffectiveness,
                                           Config.Ship.damageControlFireMaxEffectiveness,
                                           _type.fireFighting / 100f);

        _baseFloodingEffectivness = Mathf.Lerp(Config.Ship.damageControlFloodingMinEffectiveness,
                                               Config.Ship.damageControlFloodingMaxEffectiveness,
                                               _type.counterFlooding / 100f);
    }

    protected override void Initialise() { }
    protected override void UpdateTick() { }


    protected override void GameTick()
    {
        if (_ship.isSinking)
        {
            return;
        }

        _internalTimer += Time.deltaTime;
    
        if (_damageToTarget == null)
        {
            if (_internalTimer > CHECK_FOR_DAMAGED_SECTION_TIME_INTERVAL)
            {
                LookForSomethingToRepair();
                _internalTimer = 0f;
            }
        }
        else
        {
            RepairTick();

            if (_internalTimer > TIME_TO_REPAIR)
            {
                FinishRepairing();
                _internalTimer = 0f;

                // Immediately check for somethign to repair
                LookForSomethingToRepair();
            }
        }
    }

    private void LookForSomethingToRepair()
    {
        if (FindDamageWithHighestIntensity(_sections, out _sectionToTarget, out _damageToTarget))
        {
            // We found a section to repair
            StartRepairing();
        }
        else
        {
            // We could not find a section to repair
        }
    }

    private bool FindDamageWithHighestIntensity(ShipSection[] sectionsToRepair, out ShipSection sectionToTarget, out DamageInstance damageToTarget)
    {
        sectionToTarget = null;
        damageToTarget = null;

        // Go through each ship section
        foreach (ShipSection shipSection in sectionsToRepair)
        {
            // Go through the damage instances on each section (Flooding, Fires, etc)
            foreach (DamageInstance damageInstance in shipSection.state.damages)
            {
                // Auto pick if its null
                if (damageToTarget == null)
                {
                    damageToTarget = damageInstance;
                    sectionToTarget = shipSection;
                }
                // Else compare to pick the highest intensity damage to target.
                else if (damageInstance.intensity > damageToTarget.intensity)
                {
                    damageToTarget = damageInstance;
                    sectionToTarget = shipSection;
                }
            }             
        }

        // return TRUE if we found a section to target.
        return damageToTarget != null;
    }


    private void StartRepairing()
    {
        _repairEffectivenessRandomMultiplier = 1f - UnityEngine.Random.Range(-REPAIR_STRENGTH_RANDOM_VARIANCE, REPAIR_STRENGTH_RANDOM_VARIANCE);
    }

    private void RepairTick()
    {
        float repairStrength = 0;
        if (damageToTarget.damageEffect.usesFirefightingSkill)
        {
            repairStrength = _baseFireEffectivness;
        }
        else if (damageToTarget.damageEffect.usesFloodingSkill)
        {
            repairStrength = _baseFloodingEffectivness;
        }

        float reduction = Time.deltaTime 
                          * _repairEffectivenessRandomMultiplier 
                          * repairStrength;
         
        _damageToTarget.ReduceIntensity(reduction); 
    }

    private void FinishRepairing()
    {
        _damageToTarget = null;
        _sectionToTarget = null;
    }


    public string GetName()
    {
        return _componentSlot.slotName;
    }



    public ComponentEffectiveness[] GetComponentEffectivenesses()
    {
        return null;
    }
}
