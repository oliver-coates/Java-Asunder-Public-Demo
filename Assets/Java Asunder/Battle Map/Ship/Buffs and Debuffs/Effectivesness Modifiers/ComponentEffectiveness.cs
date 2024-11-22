using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[Serializable]
public class ComponentEffectiveness : ICharacterInteractable
{
    private const float EFFECTIVENESS_AMOUNT = 0.5f;

    public float value
    {
        get
        {
            return GetEffectiveness();
        }	
    }

    [SerializeField] private bool _isCurrentlyBeingBuffedByPlayer;
    public bool isCurrentlyBeingBuffedByPlayer
    {
        get
        {
            return _isCurrentlyBeingBuffedByPlayer;
        }	
    }

    [SerializeField] private List<EffectivenessModifier> _modifiers;


    [Header("Decorative:")]
    public string name;
    public string description;


    public ComponentEffectiveness(string name, string taskDescription)
    {
        this.name = name;
        this.description = taskDescription;

        _modifiers = new List<EffectivenessModifier>();
    }


    public float GetEffectiveness()
    {
        float effectiveness = 1f;

        foreach (EffectivenessModifier modifier in _modifiers)
        {
            effectiveness += modifier.effect;
        }

        effectiveness = Mathf.Clamp(effectiveness, 0, 2f);

        return effectiveness;
    }

    public void Tick()
    {
        List<EffectivenessModifier> toRemove = new List<EffectivenessModifier>();

        foreach (EffectivenessModifier modifer in _modifiers)
        {
            modifer.Tick(Time.deltaTime);

            if (modifer.timeRemaining < 0)
            {
                toRemove.Add(modifer);
            }   
        }

        // Rmeove modifers who's time remaining has fallen below 0
        foreach (EffectivenessModifier modifierToRemove in toRemove)
        {
            _modifiers.Remove(modifierToRemove);

            // If this modifier was applied by a player character,
            // flip this boolean back to false.
            // TODO: This is going to introduce a problem if multiple players try to
            // buff the same component (which they shouldn't be able to anyway.)
            if (modifierToRemove.characterApplyingBuff != null)
            {
                _isCurrentlyBeingBuffedByPlayer = false;
            }
        }
    }

    public void ApplyRoll(int roll, PlayerCharacter playerCharacter)
    {
         // Get the roll as a value ranging from
        // 0 at a roll of 3 to 1 at a roll of 18
        // High rolls will exceed this (if players roll exceptionally well)
        float rollAsPercent = (roll - 3f) / 15f;

        float effectMultipier = Mathf.Lerp(-EFFECTIVENESS_AMOUNT, EFFECTIVENESS_AMOUNT, rollAsPercent);

        _modifiers.Add(new EffectivenessModifier(effectMultipier, playerCharacter));
        _isCurrentlyBeingBuffedByPlayer = true;
    }

    public string GetTaskName()
    {
        return name;
    }

    public string GetTaskDescription()
    {
        return description;
    }
}
