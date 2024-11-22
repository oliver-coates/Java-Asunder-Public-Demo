using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace KahuInteractive
{

namespace HassleFreeAudio
{

public class AudioPlayer : MonoBehaviour
{
    private List<AudioSource> _audioSources;

    private void Awake()
    {
        _audioSources = new List<AudioSource>();
    }

    public void PlaySound(AudioClip clip, AudioMixerGroup mixer, Vector3 worldPosition, float volume, float pitch, bool playInWorldSpace, ClipSet clipSet)
    {
        AudioSource source = GetNextAvailableAudioSource();

        source.dopplerLevel = 0;
        source.volume = volume;
        source.pitch = pitch;
        source.transform.position = worldPosition;
        
        // Setup mixer group
        if (mixer != null)
        {
            source.outputAudioMixerGroup = mixer;
        }
        else
        {
            // No mixer group set
            source.outputAudioMixerGroup = null;
        }

        // Setup spatialisation
        if (playInWorldSpace)
        {
            source.spatialBlend = 1;
            source.spatialize = true;

            source.rolloffMode = AudioRolloffMode.Custom;
            source.maxDistance = clipSet.maxDistance;
            source.SetCustomCurve(AudioSourceCurveType.CustomRolloff, clipSet.spatialisationCurve);
        }
        else
        {
            source.spatialize = false;
            source.spatialBlend = 0;
        }


        source.PlayOneShot(clip);

    }

    private AudioSource GetNextAvailableAudioSource()
    {
        foreach (AudioSource source in _audioSources)
        {
            if (source.isPlaying)
            {
                continue;
            }
            else
            {
                return source;
            }
        }

        // We haven't found a proper source
        // Create a new one

        GameObject newPlayer = new GameObject("Audio Source");
        newPlayer.transform.parent = transform; 
        AudioSource newSource = newPlayer.AddComponent<AudioSource>();
        _audioSources.Add(newSource);

        return newSource;
    }
}

}

}