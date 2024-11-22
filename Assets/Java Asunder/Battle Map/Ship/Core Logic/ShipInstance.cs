using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Ships
{


public class ShipInstance : BoardPiece
{
    public enum AttackOrder
    {
        DoNotFire = 0,
        FireAtWill = 1
    }
    public const float TIME_TO_SINK = 20f;

    #region Events
    public static event Action<ShipInstance> OnShipInstanceCreated;
    public static event Action<ShipInstance> OnShipInstanceDestroyed;
    public static event Action<ShipInstance> OnPlayerShipCreated;

    public event Action<ShipInstance> OnTargetSet;
    public static event Action<ShipInstance> OnShipStartedSinking;
    public event Action OnStartedSinking;
    public event Action OnSunk;
    #endregion


    [Header("Ship Data:")]
    public Ship shipData;

    [Header("State:")]
    [HideInInspector] public ShipAIController AI; // Will only exist on non-player ships
    [SerializeField] private bool _isSinking;
    public bool isSinking
    {
        get
        {
            return _isSinking;
        }	
    }
    
    private float _sinkTimer;
    public float sinkLerpValue;

    [SerializeField] private AttackOrder _attackOrder;
    public AttackOrder attackOrder
    {
        get
        {
            return _attackOrder;
        }	
    }


    [Header("Decoration:")]
    public float UIDisplayOffset = -50f;
    public SpriteRenderer shipSpriteRenderer; // The sprite renderer for the base ship
    public LineRenderer shipDirectionLine; // A direction line only visible to the game master
    public LineRenderer shipTargetLine; // A line visible only to the game master, shows what the ship is targetting with its guns.
    public LineRenderer torpedoLaunchLine;

    [Header("Sections:")]
    #region Sections
    [SerializeField] private ComponentSlot armourSlot;
    [HideInInspector] public ShipSection[] sections;
    [HideInInspector] public ComponentSlot[] componentSlots;
    #endregion

    [Header("Easy component access")]
    public Rigidbody2D rb;
    public ComponentSlot bridgeComponentSlot;
    

    [Header("Core Components: (Filled out at runtime)")]
    #region Core Components
    [HideInInspector] public ShipBridgeInstance bridge;
    [HideInInspector] public ArmourType armourType;
    [HideInInspector] public List<DamageControlInstance> damageControls;
    [HideInInspector] public EngineScript engine;
    [HideInInspector] public FireControlInstance fireControl;
    #endregion

    [Header("Input:")]
    #region Input
    // The current state that this ships rudder is set to,
    // -1 being full port, 1 being full starboard
    [SerializeField] [Range(-1,1)] private float _rudder;
    public float rudder
    {
        get
        {
            return _rudder;
        }

        set
        {
            _rudder = value;
        }
    }

    // The target speed, that the ship's engine is accelerating towards
    // 0.0 = stop
    // 0.25 = slow ahead
    // 0.5 = half ahead
    // 0.75 = cruise ahead
    // 1.0 = full ahead
    [SerializeField] [Range(0,1)] private float _targetSpeed;
    public float targetSpeed
    {
        get
        {
            return _targetSpeed;
        }

        set
        {
            _targetSpeed = value;
        }
    }

    [SerializeField] private ShipInstance _target;
    public ShipInstance target
    {
        get
        {
            return _target;
        }	
    }
    #endregion


    protected override void Initialise()
    {
        OnShipInstanceCreated?.Invoke(this);
    }

    private void OnDestroy()
    {
        OnShipInstanceDestroyed?.Invoke(this);
    }


    public void Setup(Ship ship)
    {
        shipData = ship;
        ship.instance = this;
        name = ship.GetFullName();

        sections = GetComponentsInChildren<ShipSection>();
        damageControls = new List<DamageControlInstance>();

        // Intiialsie all sections & Gather all component slots
        List<ComponentSlot> allSlots = new List<ComponentSlot>();
        
        // Don't forget about armour:
        allSlots.Add(armourSlot);
        armourSlot.Initialise(this, null);

        foreach (ShipSection section in sections)
        {
            section.Setup(this);
            allSlots.AddRange(section.slots);
        }

        componentSlots = allSlots.ToArray();

        if (ship != SessionMaster.PlayerShip)
        {
            // Since this is not a player ship, it needs to be controlled by an AI 
            AI = gameObject.AddComponent<ShipAIController>();
        }
        else
        {
            // This is a player ship
            OnPlayerShipCreated?.Invoke(this);
            SessionMaster.MoveAllPlayerCharactersToBridge(this);

            gameObject.AddComponent<AudioListener>(); // This low listens for audio
        }
    }


    protected override void GameTick()
    {
        if (isSinking)
        {
            _sinkTimer += Time.deltaTime;
            sinkLerpValue = _sinkTimer / TIME_TO_SINK;

            shipSpriteRenderer.color = Color.Lerp(Color.white, Color.clear, sinkLerpValue);
        
            if (_sinkTimer > TIME_TO_SINK)
            {
                OnSunk?.Invoke();
                Destroy(gameObject);
            }
            return;
        }
        
        if (target != null)
        {
            if (target.isSinking)
            {
                SetTarget(null);
            }
        }
    }

    protected override void UpdateTick()
    {
        UpdateTargetLine();
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 tagPos = transform.position + new Vector3(0, UIDisplayOffset, 0);    
        Gizmos.DrawLine(transform.position, tagPos);
        
        Vector3 boxSizeApprox = new Vector3(10, 5, 0.1f);
        Gizmos.DrawCube(tagPos, boxSizeApprox);
        
    }

    public void SetTarget(ShipInstance shipInstance)
    {
        if (shipInstance == null)
        {
            _target = null;
            OnTargetSet?.Invoke(null);
        }
        else
        {
            if (_target != shipInstance)
            {
                // If we have switched target from targetting nothing or from another target
                // Then start on DoNotFire
                _attackOrder = AttackOrder.DoNotFire;
            }
            else
            {
                // If we have ALREADY targetted this ship and we are targettign it again,
                // Switch our fire mode
                if (_attackOrder == AttackOrder.DoNotFire)
                {
                    _attackOrder = AttackOrder.FireAtWill;
                }
                else if (_attackOrder == AttackOrder.FireAtWill)
                {
                    _attackOrder = AttackOrder.DoNotFire;
                }
            }

            _target = shipInstance;
            OnTargetSet?.Invoke(shipInstance);
        }

        
 
    }

    public void StartSinking()
    {
        _sinkTimer = 0f;
        sinkLerpValue = 0f;

        _isSinking = true;

        SetTarget(null);

        OnStartedSinking?.Invoke();
        OnShipStartedSinking?.Invoke(this);
    }

    private void UpdateTargetLine()
    {
        if (shipData.isSelectedByGameMaster && _target != null)
        {
            shipTargetLine.enabled = true;

            Vector3[] positions = new Vector3[2];
            positions[0] = transform.position;
            positions[1] = _target.transform.position;

            shipTargetLine.SetPositions(positions);
       
            if (attackOrder == AttackOrder.FireAtWill)
            {
                shipTargetLine.endColor = Color.red;
                shipTargetLine.startColor = Color.red;
            }
            else
            {
                shipTargetLine.endColor = Color.yellow;
                shipTargetLine.startColor = Color.yellow;   
            }
        }
        else
        {
            shipTargetLine.enabled = false;
        }
    }


}

}