using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerBuff
{
    public PlayerCharacter playerCharacter;
    public float timeRemaining;
    public int roll;

    public PlayerBuff(PlayerCharacter playerCharacter, float timeRemaining, int roll)
    {
        this.playerCharacter = playerCharacter;
        this.timeRemaining = timeRemaining;
        this.roll = roll;
    }
}
