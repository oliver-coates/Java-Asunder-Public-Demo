using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EffectivenessModifier
{

    public float effect;
    [SerializeField] private float _timeRemaining;
    public float timeRemaining
    {
        get
        {
            return _timeRemaining;
        }
    }

    public PlayerCharacter characterApplyingBuff;

    public EffectivenessModifier(float effect)
    {
        this.effect = effect;
        _timeRemaining = PlayerCharacter.TASK_TIME;
        characterApplyingBuff = null;
    }

    public EffectivenessModifier(float effect, PlayerCharacter playerCharacter)
    {
        this.effect = effect;
        this.characterApplyingBuff = playerCharacter;
        _timeRemaining = PlayerCharacter.TASK_TIME;
    }

    public void Tick(float time)
    {
        _timeRemaining -= time;
    }

}
