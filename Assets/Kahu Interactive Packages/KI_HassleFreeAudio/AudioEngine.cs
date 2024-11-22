using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace KahuInteractive
{

namespace HassleFreeAudio
{
    
public static class AudioEngine
{
    private static AudioPlayer _AudioPlayer; 
    private static bool _Initialised;

    public static void Initialise()
    {
        if (_Initialised)
        {
            Debug.LogError($"Do not initiliase the audio engine twice.");
            return;
        }

        _Initialised = true;
    
        _AudioPlayer = new GameObject("Hassle Free Audio Engine").AddComponent<AudioPlayer>();
        
        Object.DontDestroyOnLoad(_AudioPlayer.gameObject);
    }

    public static void PlaySound(AudioClip clip, AudioMixerGroup mixer, Vector3 worldPosition, float volume, float pitch, bool playInWorldSpace, ClipSet clipSet)
    {
        // Ensure we are initialised
        if (!_Initialised)
        {
            Initialise();
        }

        // Play sound:
        _AudioPlayer.PlaySound(clip, mixer, worldPosition, volume, pitch, playInWorldSpace, clipSet);
    }

    public static void PlaySound(ClipSet clipSet)
    {
        if (clipSet == null)
        {
            Debug.LogError("Recieved null clip set");
            return;
        }

        float pitch = 1f;
        float volume = 1f;
        AudioMixerGroup mixer = null;
        AudioClip clip = clipSet.GetRandomClip(out mixer, out pitch, out volume);

        PlaySound(clip, mixer, Vector3.zero, volume, pitch, false, clipSet);
    }

    public static void PlaySound(ClipSet clipSet, float volume)
    {
        if (clipSet == null)
        {
            Debug.LogError("Recieved null clip set");
            return;
        }

        float pitch = 1f;
        AudioMixerGroup mixer = null;
        AudioClip clip = clipSet.GetRandomClip(out mixer, out pitch, out float volumeUnused);

        PlaySound(clip, mixer, Vector3.zero, volume, pitch, false, clipSet);
    }

    public static void PlaySound(ClipSet clipSet, Vector3 worldPosition)
    {
        if (clipSet == null)
        {
            Debug.LogError("Recieved null clip set");
            return;
        }

        float pitch = 1f;
        float volume = 1f;
        AudioMixerGroup mixer = null;
        AudioClip clip = clipSet.GetRandomClip(out mixer, out pitch, out volume);

        PlaySound(clip, mixer, worldPosition, volume, pitch, true, clipSet);
    }

    public static void PlaySound(ClipSet clipSet, Vector3 worldPosition, float volume)
    {
        if (clipSet == null)
        {
            Debug.LogError("Recieved null clip set");
            return;
        }

        float pitch = 1f;
        AudioMixerGroup mixer = null;
        AudioClip clip = clipSet.GetRandomClip(out mixer, out pitch, out float volumeUnused);

        PlaySound(clip, mixer, worldPosition, volume, pitch, true, clipSet);
    }

    public static void PlaySound(AudioClip clip)
    {
        PlaySound(clip, null, Vector3.zero, 1f, 1f, false, null);
    }

    public static void PlaySound(AudioClip clip, float volume)
    {
        PlaySound(clip, null, Vector3.zero, volume, 1f, false, null);
    }
}

}

}