using System.Collections;
using System.Collections.Generic;
using Ships;
using UnityEngine;

public class FireControlInstance : BoardPiece, IShipComponentInstance
{
    private const float CONFIDENCE_GAIN_MULTIPLIER_RANDOMNESS_TIME_LENGTH = 10f;
    private const float CONDIFENCE_GAIN_MULTIPLIER = 1.45f;


    private ShipInstance _ship;
    private ElectronicsType _fireControl;
    private SectionDamageState _sectionState;

    [Header("Stats:")]
    [SerializeField] private float _baseComputationSpeed;
    [SerializeField] private float _baseEffectiveRange;


    [Header("State:")]
    [SerializeField] private float _distanceToTarget;
    public float distanceToTarget
    {
        get
        {
            return _distanceToTarget;
        }	
    }

    [SerializeField] private float _confidence;
    public float confidence
    {
        get
        {
            return _confidence;
        }	
    }

    private float _randomNoise;
    private float _randomTimer;

    public ComponentEffectiveness calculationEffectiveness;



    protected override void Initialise() { }


    public void Setup(ShipInstance ship, ComponentSlot componentSlot)
    {
        _ship = ship;
        _sectionState = componentSlot.shipSection.state;
        calculationEffectiveness = new ComponentEffectiveness("Fire Control", "Using the Ballistic Computer");
    
        _ship.OnTargetSet += ResetConfidence;

        // Calculate the base stats from the fire control stats:
        _fireControl = (ElectronicsType) componentSlot.component;

        _baseComputationSpeed = Mathf.Lerp(Config.Ship.minFireControlSpeedFromSpeed,
                                           Config.Ship.maxFireControlSpeedFromSpeed,
                                           _fireControl.speed / 100f);

        _baseEffectiveRange = Mathf.Lerp(Config.Ship.fireControlMinEffectiveDistance,
                                         Config.Ship.fireControlMinEffectiveDistance,
                                         _fireControl.range / 100f);

        if (_ship.fireControl != null)
        {
            Debug.LogError("This ship contains multiple fire control systems. Only one will be active.");
            return;
        }
        else
        {
            _ship.fireControl = this;
        }
    }

    private void OnDestroy()
    {
        _ship.OnTargetSet -= ResetConfidence;
    }



    protected override void GameTick()
    {
        calculationEffectiveness.Tick();

        if (_ship.isSinking)
        {
            return;
        }

        if (_ship.target != null)
        {
            ComputeFiringSolution();
        }
    }

    private void ComputeFiringSolution()
    {
        _distanceToTarget = Vector3.Distance(transform.position, _ship.target.transform.position);

        float distanceModifier = 1f-(Mathf.Clamp(_distanceToTarget/_baseEffectiveRange, 1f, 2f)-1f);

        // Add confidence linearly
        _confidence += Time.deltaTime 
                       * _baseComputationSpeed
                       * _randomNoise 
                       * distanceModifier 
                       * calculationEffectiveness.value
                       * _sectionState.effectivenessMultiplier;

        // Lose confidence due to enemy velocity
        _confidence -= Time.deltaTime * _ship.targetSpeed * Config.Ship.fireControlConfidenceLossFromEnemyVelocity;

        // Clamp confidence
        _confidence = Mathf.Clamp(_confidence, 0, 100f);

        // Random noise:
        _randomTimer += Time.deltaTime;
        if (_randomTimer > CONFIDENCE_GAIN_MULTIPLIER_RANDOMNESS_TIME_LENGTH)
        {
            _randomTimer = 0;
            _randomNoise = 1f - Random.Range(-Config.Ship.fireControlRandomnessMultiplierVariance,
                                              Config.Ship.fireControlRandomnessMultiplierVariance);
        }

    }


    protected override void UpdateTick() { }

    private void ResetConfidence(ShipInstance instance)
    {
        _confidence = 0;
        _randomTimer = CONFIDENCE_GAIN_MULTIPLIER_RANDOMNESS_TIME_LENGTH;
    }

    public ComponentEffectiveness[] GetComponentEffectivenesses()
    {
        ComponentEffectiveness[] output = new ComponentEffectiveness[1];

        output[0] = calculationEffectiveness;

        return output;
    }
}
