using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Cinematic State", menuName = "Java Asunder/Cinematics/CinematicState", order = 1)]
public class CinematicState : ScriptableObject
{
    public Sprite[] displaySprites;

    public CinematicSoundscape[] soundscapes;

    public Sprite GetRandomSprite()
    {
        return displaySprites[Random.Range(0, displaySprites.Length)];
    }
}
