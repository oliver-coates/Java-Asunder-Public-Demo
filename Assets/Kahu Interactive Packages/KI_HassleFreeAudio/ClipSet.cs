using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace KahuInteractive
{

namespace HassleFreeAudio
{

[CreateAssetMenu(fileName = "New Clip Set", menuName = "KahuInteractive/HassleFreeAudio/ClipSet", order = 1)]
public class ClipSet : ScriptableObject
{
    [SerializeField] private AudioClip[] _clips;
    [SerializeField] private AudioMixerGroup _mixer;

    [Range(0,1)]
    [SerializeField] private float _pitchVariance;
    [Range(0,1)]
    [SerializeField] private float _volumeVariance; 

    [Header("Sound Spacialisation Settings: (Ignored if not played in world space)")]
    public float maxDistance;    
    public AnimationCurve spatialisationCurve;
    

    public AudioClip GetRandomClip()
    {
        return _clips[Random.Range(0,_clips.Length)];
    }

    public AudioClip GetRandomClip(out AudioMixerGroup mixer, out float pitch, out float volume)
    {
        pitch = 1f + Random.Range(-_pitchVariance, _pitchVariance);
        volume = 1f + Random.Range(-_volumeVariance, _volumeVariance);
        mixer = _mixer;

        return _clips[Random.Range(0,_clips.Length)];
    }
}

}

}