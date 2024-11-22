using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ship Configuration", menuName = "Java Asunder/Config/Ships", order = 1)]
public class ShipConfiguration : ScriptableObject
{

    #region Engines:
    [Header("Engines:")]
    [SerializeField] private float _minShipSpeedFromStrength;
    public float minEngineForceFromStrength
    {
        get
        {
            return _minShipSpeedFromStrength;
        }	
    }

    [SerializeField] private float _maxShipSpeedFromStrength;
    public float maxEngineForceFromStrength
    {
        get
        {
            return _maxShipSpeedFromStrength;
        }	
    }
    
    [SerializeField] private float _minTurnSpeedFromAgility;
    public float minTurnSpeedFromAgility
    {
        get
        {
            return _minTurnSpeedFromAgility;
        }	
    }

    [SerializeField] private float _maxTurnSpeedFromAgility;
    public float maxTurnSpeedFromAgility
    {
        get
        {
            return _maxTurnSpeedFromAgility;
        }	
    }

    [Header("Engines / Spool Speed:")]
    [SerializeField] private float _minEngineSpoolSpeedFromAgility;
    public float minEngineSpoolSpeedFromAgility
    {
        get
        {
            return _minEngineSpoolSpeedFromAgility;
        }	
    }

    [SerializeField] private float _maxEngineSpoolSpeedFromAgility;
    public float maxEngineSpoolSpeedFromAgility
    {
        get
        {
            return _maxEngineSpoolSpeedFromAgility;
        }	
    }

    [SerializeField] private float _engineSpoolDownSpeedMultilier = 1.5f;
    public float engineSpoolDownSpeedMultpilier
    {
        get
        {
            return _engineSpoolDownSpeedMultilier;
        }	
    }

    [Header("Engines / Rigidbody:")]
    [SerializeField] private float _shipDrag;
    public float shipDrag
    {
        get
        {
            return _shipDrag;
        }	
    }
    
    [SerializeField] private float _shipDragAngular;
    public float shipDragAngular
    {
        get
        {
            return _shipDragAngular;
        }	
    }
    #endregion

    #region Fire Control
    [Header("Fire Control / Computation Speed")]
    [SerializeField] private float _minFireControlSpeedFromSpeed;
    public float minFireControlSpeedFromSpeed
    {
        get
        {
            return _minFireControlSpeedFromSpeed;
        }	
    }

    [SerializeField] private float _maxFireControlSpeedFromSpeed;
    public float maxFireControlSpeedFromSpeed
    {
        get
        {
            return _maxFireControlSpeedFromSpeed;
        }	
    }

    [Header("Fire Control / Effective Distance")]
    [SerializeField] private float _fireControlMinEffectiveDistance;
    public float fireControlMinEffectiveDistance
    {
        get
        {
            return _fireControlMinEffectiveDistance;
        }	
    }

    [SerializeField] private float _fireControlMaxEffectiveDistance;
    public float fireControlMaxEffectiveDistance
    {
        get
        {
            return _fireControlMaxEffectiveDistance;
        }	
    }

    [Header("Fire Control / Additional:")]
    [SerializeField] private float _fireControlConfidenceLossFromEnemyVelocityFactor;
    public float fireControlConfidenceLossFromEnemyVelocity
    {
        get
        {
            return _fireControlConfidenceLossFromEnemyVelocityFactor;
        }	
    }

    [SerializeField] private float _fireControlRandomnessMultiplierVariance;
    public float fireControlRandomnessMultiplierVariance
    {
        get
        {
            return _fireControlRandomnessMultiplierVariance;
        }	
    }

    #endregion

    #region Guns

    [Header("Guns / Reload Times")]
    [SerializeField] private float _gunMinReloadTime;
    public float gunMinReloadTime
    {
        get
        {
            return _gunMinReloadTime;
        }	
    }
    
    [SerializeField] private float _gunMaxReloadTime;
    public float gunMaxReloadTime
    {
        get
        {
            return _gunMaxReloadTime;
        }	
    }

    [Header("Guns / Turn Speeds")]
    [SerializeField] private float _gunMinTurnSpeed;
    public float gunMinTurnSpeed
    {
        get
        {
            return _gunMinTurnSpeed;
        }	
    }

    [SerializeField] private float _gunMaxTurnSpeed;
    public float gunMaxTurnSpeed
    {
        get
        {
            return _gunMaxTurnSpeed;
        }	
    }

    [Header("Guns / Inaccuracy")]
    [SerializeField] private float _gunMinInaccuracy;
    public float gunMinInaccuracy
    {
        get
        {
            return _gunMinInaccuracy;
        }	
    }

    [SerializeField] private float _gunMaxInaccuracy;
    public float gunMaxInaccuracy
    {
        get
        {
            return _gunMaxInaccuracy;
        }	
    }

    #endregion

    #region Shells
    [Header("Shells")]
    [SerializeField] private float _shellMinVelocity;
    public float shellMinVelocity
    {
        get
        {
            return _shellMinVelocity;
        }	
    }

    [SerializeField] private float _shellMaxVelocity;
    public float shellMaxVelocity
    {
        get
        {
            return _shellMaxVelocity;
        }	
    }

    [SerializeField] private float _shellMinDudChance;
    public float shellMinDudChance
    {
        get
        {
            return _shellMinDudChance;
        }	
    }

    [SerializeField] private float _shellMaxDudChance;
    public float shellMaxDudChance
    {
        get
        {
            return _shellMaxDudChance;
        }	
    }
    
    #endregion

    #region Damage Control

    [Header("Damage Control / Fires:")]
    [SerializeField] private float _damageControlFireMinEffectiveness;
    public float damageControlFireMinEffectiveness
    {
        get
        {
            return _damageControlFireMinEffectiveness;
        }	
    }

    [SerializeField] private float _damageControlFireMaxEffectiveness;
    public float damageControlFireMaxEffectiveness
    {
        get
        {
            return _damageControlFireMaxEffectiveness;
        }	
    }
    [Header("Damage Control / Flooding:")]
    [SerializeField] private float _damageControlFloodingMinEffectiveness;
    public float damageControlFloodingMinEffectiveness
    {
        get
        {
            return _damageControlFloodingMinEffectiveness;
        }	
    }

    [SerializeField] private float _damageControlFloodingMaxEffectiveness;
    public float damageControlFloodingMaxEffectiveness
    {
        get
        {
            return _damageControlFloodingMaxEffectiveness;
        }	
    }

    [Header("Damage Control / Players:")]
    [SerializeField] private int _damageControlMedianPlayerRoll = 10;
    public int damageControlMedianPlayerRoll
    {
        get
        {
            return _damageControlMedianPlayerRoll;
        }	
    }

    [SerializeField] private float _damageControlPlayerIntensityReductionPerPositiveRoll;
    public float damageControlPlayerIntensityReductionPerPositiveRoll
    {
        get
        {
            return _damageControlPlayerIntensityReductionPerPositiveRoll;
        }	
    }
    #endregion

    #region Armour

    [SerializeField] private float _armourStrengthMultiplier;
    public float armourStrengthMultiplier
    {
        get
        {
            return _armourStrengthMultiplier;
        }	
    }

    #endregion
}

