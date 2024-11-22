using System.Collections;
using System.Collections.Generic;
using KahuInteractive.HassleFreeAudio;
using UnityEngine;

namespace Ships
{


[CreateAssetMenu(fileName = "New Damage Type", menuName = "Java Asunder/Damage/Damage Type", order = 1)]
public class DamageType : ScriptableObject
{
    [Header("Decorative:")]
    [SerializeField] private string _description;
    public string description
    {
        get
        {
            return _description;
        }	
    }

    [SerializeField] private Sprite _displaySprite;
    public Sprite displaySprite
    {
        get
        {
            return _displaySprite;
        }	
    }

    [SerializeField] private ClipSet _clipSet;
    public ClipSet clipSet
    {
        get
        {
            return _clipSet;
        }	
    }

    [Header("Chance of hit:")]
    [SerializeField] private float _minimumHitSeverity;
    public float minimumHitSeverity
    {
        get
        {
            return _minimumHitSeverity;
        }
    }
    [SerializeField] private float _chanceToOccurPerHitSeverity;   
    public float chanceToOccurPerHitSeverity
    {
        get
        {
            return _chanceToOccurPerHitSeverity;
        }
    }


    [Header("Possible damage to integrity:")]
    [SerializeField] private float _minimumDamageToIntegrity;
    [SerializeField] private float _maximumDamageToIntegrity;
    public float GetDamageToIntegrity()
    {
        return Random.Range(_minimumDamageToIntegrity, _maximumDamageToIntegrity);
    }

    [Header("Effects caused by this damage type:")]
    [SerializeField] private DamageEffect[] _effectsCaused;
    public DamageEffect[] effectsCaused
    {
        get
        {
            return _effectsCaused;
        }
    }


    [Header("Possible starting intensity of damage type:")]
    [SerializeField] private float _minimumDamageStartingIntensity;
    [SerializeField] private float _maximumDamageStartingIntensity;


}

}