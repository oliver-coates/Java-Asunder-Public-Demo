using System;
using System.Collections;
using System.Collections.Generic;
using Effects;
using Ships;
using UnityEngine;

public class TorpedoInstance : BoardPiece
{
    [Serializable]
    private enum SubmergedState
    {
        AboveWater,
        EnteringWater,
        BelowWater
    }


    [Header("Settings:")]
    [SerializeField] private AmmunitionType _ammunitionType;
    [SerializeField] private float _armTime = 5f;
    [SerializeField] private float _velocity = 20f;

    [Header("State:")]
    [SerializeField] private float _timer;

    [Header("References:")]
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Transform _tipPoint;
    

    [Header("Decorative")]
    [SerializeField] private SubmergedState _submergedState;
    [SerializeField] private float _timeSpentInAir = 1.2f;
    [SerializeField] private float _timeToEnterWater = 0.25f;
    [SerializeField] private Color _aboveWaterColor = Color.white;
    [SerializeField] private Color _belowWaterColor = Color.black;



    protected override void Initialise()
    {
        _timer = 0f;

        _submergedState = SubmergedState.AboveWater;
        _spriteRenderer.color = _aboveWaterColor;
    }

    protected override void GameTick()
    {
        _timer += Time.deltaTime;

        if (_timer > 300)
        {
            Destroy(gameObject);
        }

        switch (_submergedState)
        {
            case (SubmergedState.AboveWater):
                if (_timer > _timeSpentInAir)
                {
                    StartEnterWater();
                }
                break;
            
            case (SubmergedState.EnteringWater):
                float colorLerp = (_timer - _timeSpentInAir) / _timeToEnterWater;
                _spriteRenderer.color = Color.Lerp(_aboveWaterColor, _belowWaterColor, colorLerp);

                if (_timer > (_timeSpentInAir + _timeToEnterWater))
                {
                    FinishEnterWater();
                }
                break;
            
            case (SubmergedState.BelowWater):
                if (_timer > _armTime)
                {
                    CheckForCollision();
                }

                break;
            
            default:
                break;
        }
    }

    protected override void UpdateTick()
    {
    
    }

    private void StartEnterWater()
    {
        _submergedState = SubmergedState.EnteringWater;
    }

    private void FinishEnterWater()
    {
        _submergedState = SubmergedState.BelowWater;
    }

    private void CheckForCollision()
    {
        Vector3 positionNextFrame = _tipPoint.position + (_tipPoint.up * _velocity);

        RaycastHit2D hit = Physics2D.Linecast(_tipPoint.position, positionNextFrame);
        if (hit.collider != null)
        {
            if (hit.collider.tag == "ShipSection")
            {
                ShipSection hitSection = hit.collider.GetComponent<ShipSection>();
                if (hitSection != null)
                {
                    HitShipSection(hitSection);
                }
            }
        }
    }

    private void HitShipSection(ShipSection hitSection)
    {
        // Roll for dud chance:
        // float dudRoll = Random.Range(0f, 1f);
        // if (dudRoll <= _baseDudChance)
        // {
        //     // Dud!
        //     Destroy(gameObject);
        //     return;
        // }

        // Hit:
        hitSection.Hit(_ammunitionType);
        EffectManager.SpawnEffect(_ammunitionType.explosionEffect, transform.position);
        Destroy(gameObject);
    }

}
