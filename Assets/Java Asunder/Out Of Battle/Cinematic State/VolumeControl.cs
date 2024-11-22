using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class VolumeControl : MonoBehaviour
{

    public static event Action<float> OnVolumeChange;


    [SerializeField] private Slider _slider;

    private void Awake()
    {
        OnVolumeChange += SetVolumeSlider;
    }

    private void OnDestroy()
    {
        OnVolumeChange -= SetVolumeSlider;
    }

    private void SetVolumeSlider(float toValue)
    {
        _slider.SetValueWithoutNotify(toValue);
    }

    public void OnValueChanged(float newValue)
    {
        OnVolumeChange?.Invoke(newValue);        
    }
}
