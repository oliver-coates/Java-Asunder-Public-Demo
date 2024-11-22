using System.Collections;
using System.Collections.Generic;
using Ships;
using UnityEngine;

public class GunAimer : MonoBehaviour
{  
    private const float SHOT_BIAS_DISTANCE_MODIFIER = 0.2f;


    private ShipInstance _ship;

    [Header("Settings:")]
    [SerializeField] private bool _useFireControl;
    public bool usesFireControl
    {
        get
        {
            return _useFireControl;
        }
    }
    
    [Header("State:")]
    public ComponentEffectiveness aimEffectiveness;
    [SerializeField] private Vector2 _shotBias;

    [SerializeField] private float _crewAimingEffectiveness;
    [SerializeField] private float _crewAimingEffectivenssTimer;

    public void Setup(ShipInstance ship, ComponentSlot componentSlot)
    {
        _ship = ship;

        aimEffectiveness = new ComponentEffectiveness("Aiming", $"({componentSlot.slotName}) Aiming");
    }

    public void GenerateNewShotBias()
    {
        _shotBias = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }

    /// <summary>
    /// Gets the location, as a vector3, that this turret thinks is the enemy ship position
    /// </summary>
    public Vector3 GetAimLocation(ShipInstance target, float distanceToTarget, float shotVelocity)
    {
        // Deterime the actual position of where the target is:
        float timeToReachTarget = distanceToTarget / shotVelocity;
        Vector3 velocityOffset = target.rb.velocity * timeToReachTarget;
        Vector3 velocityAdjustedTargetPosition = target.transform.position + velocityOffset;

        // Add in the shot bias
        Vector2 shotBiasAmount = DetermineShotBias(distanceToTarget);

        Vector3 finalLocation = velocityAdjustedTargetPosition;
        finalLocation.x += shotBiasAmount.x;
        finalLocation.y += shotBiasAmount.y;

        return finalLocation;
    }

    private Vector2 DetermineShotBias(float distanceToTarget)
    {
        Vector2 shotBiasAmount;
        if (_useFireControl)
        {
            shotBiasAmount = DetermineShotBiasWithBallisticComputer(distanceToTarget);
        }
        else
        {
            shotBiasAmount = DetermineShotBiasWithManualAim(distanceToTarget);
        }
         

        return shotBiasAmount;
    }

    private Vector2 DetermineShotBiasWithBallisticComputer(float distanceToTarget)
    {
        Vector2 shotBiasAmount = _shotBias * (distanceToTarget * SHOT_BIAS_DISTANCE_MODIFIER);
        if (_ship.fireControl != null)
        {
            // if we have a fire control system, reduce the shot bias
            float accuracyMultiplier = 1f - (_ship.fireControl.confidence / 100f);
            shotBiasAmount = shotBiasAmount * accuracyMultiplier;
        }

        return shotBiasAmount;
    }

    private Vector2 DetermineShotBiasWithManualAim(float distanceToTarget)
    {
        Vector2 shotBiasAmount = _shotBias * (distanceToTarget * SHOT_BIAS_DISTANCE_MODIFIER);

        float skill;
        if (aimEffectiveness.isCurrentlyBeingBuffedByPlayer)
        {
            // Use player skill
            Debug.Log($"USING PLAYER SKILL!");

            skill = aimEffectiveness.value;
        }
        else
        {
            // Use crew skill
            skill = _crewAimingEffectiveness;
        }

        shotBiasAmount *= (1f - skill);

        return shotBiasAmount;
    }

    private void Update()
    {
        if (_useFireControl == false)
        {
            if (_crewAimingEffectivenssTimer > 0)
            {
                // Crew is aiming
                _crewAimingEffectivenssTimer -= Time.deltaTime;
            }
            else
            {
                _crewAimingEffectivenssTimer = 6f;

                _crewAimingEffectiveness = Random.Range(0.4f, 1.5f);
            }
        }
     
    }
}
