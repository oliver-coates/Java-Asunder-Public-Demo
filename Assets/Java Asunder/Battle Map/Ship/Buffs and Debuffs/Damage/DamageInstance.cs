using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ships
{


[System.Serializable]
public class DamageInstance : ICharacterInteractable
{
    private const float INTENISTY_REDUCTION_PER_SECOND_PER_PLAYER_ROLL = 0.05f;

    [NonSerialized] private SectionDamageState _state;

    public float integrityDamage
    {
        get
        {
            return (_intensity * _damageEffect.integrityReductionPerIntensity);
        }
    }

    [Header("The type of damage this is (Flooding, Fire, etc)")]
    [SerializeField] private DamageEffect _damageEffect;
    public DamageEffect damageEffect
    {
        get
        {
            return _damageEffect;
        }	
    }
    

    [Header("The intensity of this damage:")]
    [Range(0f, 100f)]
    [SerializeField] private float _intensity;
    public float intensity
    {
        get
        {
            return _intensity;
        }	
    }

    // Location of the hit
    private ShipSection _location;

    [Header("Player buffs")]
    [SerializeField] private List<PlayerBuff> _playerBuffs;

    [SerializeField] private PlayerCharacter _playerAssigned;
    [SerializeField] private float _timer;
    [SerializeField] private int _roll;

    public DamageInstance(DamageEffect type, float hitSeverity, ShipSection section)
    {
        _damageEffect = type;
        _intensity = hitSeverity;
        _state = section.state;
        _location = section;

        _playerBuffs = new List<PlayerBuff>();
    }

    public void BoostDamage(float severity)
    {
        _intensity += severity;

        _intensity = Mathf.Clamp(_intensity, 0f, 100f);
    }

    public void Tick(float deltaTime)
    {
        _intensity += deltaTime * (_damageEffect.intensityGrowthBase + (_damageEffect.intensityGrowthPerIntensity * _intensity));

        // Player character damage controls:

        // Collate the total intensity reduction from all players working
        float totalIntensityReduction = 0;

        int buffAcessorIndex = 0;
        for (int buffIndex = 0; buffIndex < _playerBuffs.Count; buffIndex++)
        {
            PlayerBuff buff = _playerBuffs[buffAcessorIndex];

            // Adjust this players roll down by the median roll
            int adjustedRoll = buff.roll - Config.Ship.damageControlMedianPlayerRoll;
            adjustedRoll = Mathf.Clamp(adjustedRoll, 1, 20); // <- at the very worst, the player can only roll a 1.

            // Determine the intensity reduction from each point above the roll.
            totalIntensityReduction += adjustedRoll * Config.Ship.damageControlPlayerIntensityReductionPerPositiveRoll;
            
            if (buff.timeRemaining > 0)
            {
                buff.timeRemaining -= deltaTime;

                // Continue iteration
                buffAcessorIndex++;
            }
            else
            {
                // Remove
                buff.playerCharacter.FinishTask();
                _playerBuffs.RemoveAt(buffAcessorIndex);
            }
        }

        ReduceIntensity(totalIntensityReduction * Time.deltaTime);

        _intensity = Mathf.Clamp(_intensity, 0f, 100f);
    }
 
    public void ReduceIntensity(float amoumt)
    {
        _intensity -= amoumt;
        _intensity = Mathf.Clamp(_intensity, 0, 100f);

        if (_intensity == 0)
        {
            _state.RemoveDamageEffect(this);
        }
    }

    public string GetTaskDescription()
    {
        return _damageEffect.GetTaskDescription(_location);
    }

    public string GetDescription()
    {
        return _damageEffect.GetDescription(_intensity);
    }

    public void ApplyRoll(int roll, PlayerCharacter playerCharacter)
    {
        _playerBuffs.Add(new PlayerBuff(playerCharacter, PlayerCharacter.TASK_TIME, roll));
    }

    public string GetTaskName()
    {
        return damageEffect.damageName;
    }
}

}