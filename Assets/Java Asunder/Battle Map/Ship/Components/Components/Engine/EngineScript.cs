using System.Collections;
using System.Collections.Generic;
using Ships;
using UnityEngine;

[System.Serializable]
public class EngineScript : BoardPiece, IShipComponentInstance
{

    #region Magic Tuners
    public const float ENGINE_SPEED_STARTING = 0.6f;
    #endregion

    [SerializeField] private ComponentSlot _engineSlot;
    private SectionDamageState _sectionState;
    [SerializeField] private Rigidbody2D _rigidBody;
    private ShipInstance _ship;
    private EngineType _engineType;

    [Header("Stats:")]
    // The speed that this engine spools up or down by
    [SerializeField] private float _baseSpoolSpeed;
    // The force that this engine can output - This comes from the strength value
    [SerializeField] private float _baseEngineForce;
    // The speed that this ship can turn - This comes from the agility value
    [SerializeField] private float _baseTurnSpeed;

    [Header("State:")]
    // The speed that the engine is currently going.
    [Range(0,1)] [SerializeField] private float _currentEngineSpeed;
    public float currentEngineSpeed
    {
        get
        {
            return _currentEngineSpeed;
        }
    }

    public float speed
    {
        get
        {
            // returns the speed of this vessel, in knots
            return _rigidBody.velocity.magnitude * 1.943844f;
        }
    }

    public ComponentEffectiveness speedEffectivness;



    protected override void Initialise() { }

    public void Setup(ShipInstance ship, ComponentSlot componentSlot)
    {
        _ship = ship;
        _engineSlot = componentSlot;
        _sectionState = componentSlot.shipSection.state;
        _ship.engine = this;
        speedEffectivness = new ComponentEffectiveness("Engines", "Overcharging the engines");

        // Set up rigid body:
        _rigidBody = ship.rb;
        _rigidBody.drag = Config.Ship.shipDrag;
        _rigidBody.angularDrag = Config.Ship.shipDragAngular;
        
        // Initial speed
        _currentEngineSpeed = ENGINE_SPEED_STARTING;
        ship.targetSpeed = ENGINE_SPEED_STARTING;

        // Calculate the base stats from the engine typpe
        _engineType = (EngineType) _engineSlot.component;
    
        _baseSpoolSpeed = Mathf.Lerp(Config.Ship.minEngineSpoolSpeedFromAgility, 
                                     Config.Ship.maxEngineSpoolSpeedFromAgility, 
                                     _engineType.agility / 100f);
        
        // Strength for ships is 1/2 the engine + 1/2 the ship
        float totalStrength =  (_engineType.strength + _ship.shipData.shipClass.speed) / 200f;

        _baseEngineForce = Mathf.Lerp(Config.Ship.minEngineForceFromStrength,
                                      Config.Ship.maxEngineForceFromStrength,
                                      totalStrength);

        // Agility for ships is 1/2 the engine + 1/2 the ship
        float totalAgility =  (_engineType.agility + _ship.shipData.shipClass.agility) / 200f;
                                
        _baseTurnSpeed = Mathf.Lerp (Config.Ship.minTurnSpeedFromAgility,
                                     Config.Ship.maxTurnSpeedFromAgility,
                                     totalAgility);
    }

    protected override void UpdateTick() { }


    protected override void GameTick()
    {
        speedEffectivness.Tick();

        if (_ship.isSinking)
        {
            return;
        }

        // Spool up/down the engine
        float engineChangeSpeed = _baseSpoolSpeed;
        // The engine spools down faster than spooling up
        if (_currentEngineSpeed > _ship.targetSpeed)
        {
            // If we are spooling down, spool down by this multiplier
            engineChangeSpeed *= Config.Ship.engineSpoolDownSpeedMultpilier;
        }
        _currentEngineSpeed = Mathf.MoveTowards(_currentEngineSpeed, _ship.targetSpeed, engineChangeSpeed * Time.deltaTime);

        // Add force:
        float speedMultiplier = _baseEngineForce * speedEffectivness.value * _sectionState.effectivenessMultiplier;
        Vector3 force = _ship.transform.up *  _currentEngineSpeed * Time.deltaTime * speedMultiplier;
        _rigidBody.AddForce(force, ForceMode2D.Force);
    
        // Rotate:
        float rotationMultiplier = _baseTurnSpeed;
        float rotateAmount = _ship.rudder 
                            * rotationMultiplier
                            * _ship.bridge.turnEffectiveness.value
                            * Time.deltaTime;
        rotateAmount *= _rigidBody.velocity.magnitude; // Scale rotation amount by the speed of the ship - faster ships turn faster.

        _rigidBody.AddTorque(rotateAmount, ForceMode2D.Force);
    }

    public ComponentEffectiveness[] GetComponentEffectivenesses()
    {
        ComponentEffectiveness[] output = new ComponentEffectiveness[1];

        output[0] = speedEffectivness;

        return output;
    }
}
