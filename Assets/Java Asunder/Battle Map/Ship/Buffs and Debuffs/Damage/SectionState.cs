using System;
using System.Collections;
using System.Collections.Generic;
using KahuInteractive.HassleFreeAudio;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.Timeline;

namespace Ships
{


[System.Serializable]
public class SectionDamageState
{
    #region Magic Numbers

    public const float SEVERITY_RANDOM_MULTIPLIER = 0.33f;
    public const float SMOKE_EMISSION_AMOUNT = 125f;

    #endregion

    [NonSerialized] private ShipInstance _ship;
    private ShipSection _section;

    private bool _isPlayerShip;

    [SerializeField] private float _integrity;
    public float integrity
    {
        get
        {
            return _integrity;
        }	
    }

    [SerializeField] private float _effectivenessMultiplier;
    public float effectivenessMultiplier
    {
        get
        {
            return _effectivenessMultiplier;
        }	
    }

    [NonSerialized] public List<DamageInstance> damages;
    public float permanentIntegrityDamage;

    private float _smokeIntensity;


    public SectionDamageState(ShipInstance ship, ShipSection section)
    {
        _ship = ship;
        _isPlayerShip = (ship.shipData == SessionMaster.PlayerShip);
        _section = section;

        damages = new List<DamageInstance>();
        permanentIntegrityDamage = 0f;
        _smokeIntensity = 0f;
        _integrity = 100f;
        UpdateSmokeEmission();
    }

    public void Tick(float deltaTime)
    {
        // Recalculate integrity & tick damage instances
        _integrity = 100f - permanentIntegrityDamage;
        _smokeIntensity = 0f;

        foreach (DamageInstance damageInstance in damages)
        {
            damageInstance.Tick(deltaTime);
            _integrity -= damageInstance.integrityDamage;

            if (damageInstance.damageEffect.emitSmoke)
            {
                _smokeIntensity += damageInstance.intensity;
            } 
        }
    
        float integrityAsPercent = _integrity / 100f;

        _effectivenessMultiplier = Mathf.Lerp(0.25f, 1f, integrityAsPercent);
    
        if (_integrity <= 0 && !_ship.isSinking)
        {
            _ship.StartSinking();
        }
    
        UpdateSmokeEmission();
    }

    public void RecieveHit(AmmunitionType ammunitionType)
    {
        DamageType[] possibleDamageTypes = GameMaster.damageTypes;

        float hitSeverity = ammunitionType.damage;

        // Apply a random boost/reduction to the severity:
        hitSeverity = hitSeverity * UnityEngine.Random.Range(1f - SEVERITY_RANDOM_MULTIPLIER,
                                                    1f + SEVERITY_RANDOM_MULTIPLIER);


        // ARMOUR CALCULATION:
        float armourStrength = _ship.armourType.strength * Config.Ship.armourStrengthMultiplier;
        // Reduce the armour strength by this shells AP effect
        armourStrength -= ammunitionType.pierce;
        // Clamp the strength of the armour to prevent overpiercing
        armourStrength = Mathf.Clamp(armourStrength, 0, 100);
        // Reduce the incoming severity by our ships armour
        hitSeverity -= armourStrength;

        // Debug.Log($"Ship {_section.ship.shipData.GetFullName()} recieves a hit with severity {hitSeverity}");

        // Roll for damage types from the severity of the hit
        List<DamageType> damageTypesDealt = RollForDamageTypes(possibleDamageTypes, hitSeverity);        

        // Apply these damage types:
        foreach (DamageType damageType in damageTypesDealt)
        {
            ApplyDamageType(damageType, hitSeverity);
        }
    }

    private List<DamageType> RollForDamageTypes(DamageType[] possibleDamageTypes, float hitSeverity)
    {
        List<DamageType> damageTypesDealt = new List<DamageType>();

        foreach (DamageType damageType in possibleDamageTypes)
        {
            if (hitSeverity < damageType.minimumHitSeverity)
            {
                // Ignore if the hit severity is less than 
                continue;
            }

            // Find the chance for this hit to create this damage type (range 0-100)
            float chance = (hitSeverity - damageType.minimumHitSeverity) * damageType.chanceToOccurPerHitSeverity;

            // roll for this:
            float roll = UnityEngine.Random.Range(0f, 100f);

            if (roll < chance)
            {
                // Debug.Log($"{damageType.name} occurs with a chance of {chance}");
                // Damage type occurs!
                damageTypesDealt.Add(damageType);
            }
        }

        return damageTypesDealt;
    }

    private void ApplyDamageType(DamageType damageType, float hitSeverity)
    {
        // Go through each damage effect that this damage type should apply:
        foreach (DamageEffect damageEffect in damageType.effectsCaused)
        {
            ApplyDamageEffect(damageEffect, hitSeverity);
        }

        // Apply permenant damage to this hulls integrity:
        permanentIntegrityDamage += damageType.GetDamageToIntegrity();

        if (_isPlayerShip)
        {
            NotifyPlayerOfDamage(damageType);
        }
    }

    private void ApplyDamageEffect(DamageEffect damageEffect, float hitSeverity)
    {
        // Check that a damage instance of this type does not exist:
        foreach (DamageInstance damageInstance in damages)
        {
            if (damageInstance.damageEffect == damageEffect)
            {
                // A damage instance already exists of this type,
                // Boost it:
                damageInstance.BoostDamage(hitSeverity);
                return;
            }
        }

        // No damage instance matches this effect, create a new one:
        DamageInstance newDamageInstance = new(damageEffect, hitSeverity, _section);
        damages.Add(newDamageInstance);
    }

    public void RemoveDamageEffect(DamageInstance damageInstance)
    {
        if (damages.Contains(damageInstance))
        {
            damages.Remove(damageInstance);
        }
    }

    public void UpdateSmokeEmission()
    {
        if (_section.smokeEmitter == null)
        {
            return;
        }
        
        float smokeIntensityMultiplier = Mathf.Clamp((_smokeIntensity / 100f), 0f, 1f);
        if (_ship.isSinking)
        {
            smokeIntensityMultiplier = 0f;
        }
        
        ParticleSystem.EmissionModule emissionModule = _section.smokeEmitter.emission;
        emissionModule.rateOverTime = SMOKE_EMISSION_AMOUNT * smokeIntensityMultiplier;
    }

    private void NotifyPlayerOfDamage(DamageType damageType)
    {
        string header = damageType.name;
        Sprite sprite = damageType.displaySprite;
        string unformattedDescription = damageType.description;
        ClipSet clipSet = damageType.clipSet;

        // bruteforce formatting... not very good
        string description = unformattedDescription.Replace("{section}", _section.sectionName);

        DisasterUI.Event outEvent = new DisasterUI.Event(header, sprite, description, clipSet);

        DisasterUI.QueueDisasterEvent(outEvent);
    } 

}

}