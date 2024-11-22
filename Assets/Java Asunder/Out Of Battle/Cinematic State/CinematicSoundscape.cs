using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Soundscape", menuName = "Java Asunder/Cinematics/Soundscape", order = 1)]
public class CinematicSoundscape : ScriptableObject
{
    public AudioClip loopSound;


    [Header("Random Sounds")]
    public bool hasRandomSounds;
    public AudioClip[] randomSounds;

    [SerializeField] private float randomSoundMinInterval = 1f;
    [SerializeField] private float randomSoundMaxInterval = 10f;

    public float GetRandomTimeInterval()
    {
        return Random.Range(randomSoundMinInterval, randomSoundMaxInterval);
    }

    public AudioClip GetRandomSound()
    {
        return randomSounds[Random.Range(0, randomSounds.Length)];
    }
}
