using System.Collections;
using System.Collections.Generic;
using KahuInteractive.HassleFreeAudio;
using UnityEngine;

namespace Ships
{

public class ShipGunScript : BoardPiece, IShipComponentInstance
{
    #region Magic numbers

    private const float TURRET_TURN_DEAD_ZONE = 0.05f;
    private const float TURRET_FIRE_ANGLE_ALLOWANCE = 0.9f; // How close the forward dot product must alight to the target for shots to be allowed to be fired
    private const float TURRET_RELOAD_RANDOMISATION = 0.1f;


    #endregion

    private ComponentSlot _componentSlot;
    private ShipInstance _ship;
    private SectionDamageState _sectionState;
    private FireAngleRestrictor _fireAngleRestrictor;


    [Header("State:")]
    public ShipInstance _target;
    [SerializeField] private bool _pointingTowardsTarget;
    [SerializeField] private float _distanceToTarget;
    private float _loadTimer;

    [Header("Effectiveness:")]
    public ComponentEffectiveness loadingEffectiveness;


    [Header("References:")]
    [SerializeField] private GunAimer _aimComponent;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Transform _turret;
    [SerializeField] private Transform _shootPoint;

    [Header("Settings:")]
    [SerializeField] private ShipGunType _gunType;

    [Header("Stats:")]
    [SerializeField] private float _baseTurnSpeed;
    [SerializeField] private float _baseReloadTime;
    [SerializeField] private float _baseInaccuracy;
    [SerializeField] private float _baseShotVelocity;

    
    #region Initialisation & Destruction
    public void Setup(ShipInstance ship, ComponentSlot componentSlot)
    {
        _ship = ship;
        _componentSlot = componentSlot;
        _sectionState = _componentSlot.shipSection.state;
        loadingEffectiveness = new ComponentEffectiveness("Loading", $"({componentSlot.slotName}) Loading");

        if (_aimComponent == null)
        {
            Debug.LogError($"No gun aim component script found for ship gun in {_componentSlot.slotName}");
            return;
        }
        _aimComponent.Setup(ship, componentSlot);

        _fireAngleRestrictor = GetComponentInParent<FireAngleRestrictor>();
        if (_fireAngleRestrictor == null)
        {
            Debug.LogError($"No fire angle restrictor script found for ship gun in {_componentSlot.slotName}");
        }

        _ship.OnTargetSet += SetTarget;

        // Setup base stats iwth the gun type:
        _gunType = (ShipGunType) _componentSlot.component;

        _baseTurnSpeed = Mathf.Lerp(Config.Ship.gunMinTurnSpeed,
                                    Config.Ship.gunMaxTurnSpeed,
                                    _gunType.turnSpeed / 100f);
        
        _baseReloadTime = Mathf.Lerp(Config.Ship.gunMaxReloadTime,
                                     Config.Ship.gunMinReloadTime,
                                     _gunType.reloadTime / 100f);
        
        _baseInaccuracy = Mathf.Lerp(Config.Ship.gunMinInaccuracy,
                                     Config.Ship.gunMaxInaccuracy,
                                     1f - (_gunType.accuracy / 100f));

        _baseShotVelocity = Mathf.Lerp(Config.Ship.shellMinVelocity,
                                       Config.Ship.shellMaxVelocity,
                                       _gunType.ammo.velocity / 100f);
    }

    private void OnDestroy()
    {
        _ship.OnTargetSet -= SetTarget;
    }
    #endregion

    protected override void Initialise() {  }

    protected override void GameTick()
    {
        loadingEffectiveness.Tick();
 
        if (_ship.isSinking)
        {
            _spriteRenderer.color = Color.Lerp(Color.white, Color.clear, _ship.sinkLerpValue);
            return;
        }

        TurnTurret();

        GunUpdate();    
    }

    protected override void UpdateTick()
    {
    }

    private void SetTarget(ShipInstance target)
    {
        _target = target;
        _aimComponent.GenerateNewShotBias();
    }


    #region Turret Rotation
    private void TurnTurret()
    {
        Vector3 dir;

        if (_target != null)
        {
            Vector3 aimLocation = _aimComponent.GetAimLocation(_target, _distanceToTarget, _baseShotVelocity);
            dir = (aimLocation -  transform.position).normalized;

            _distanceToTarget = Vector3.Distance(transform.position, aimLocation);

            Debug.DrawLine(transform.position, aimLocation, Color.red);

            TurnTurretTowards(dir);
        }
        else
        {
            // If no target, aim straight ahead            
            TurnTurretTowards(Vector3.up);
        }
    }

    private void TurnTurretTowards(Vector3 direction)
    {
        float dotRight = Vector3.Dot(direction, _turret.right);
        float dotForward = Vector3.Dot(direction, _turret.up);

        int turnDirection = 0;

        if (dotRight > TURRET_TURN_DEAD_ZONE)
        {
            turnDirection = -1;
        }
        else if (dotRight < (-TURRET_TURN_DEAD_ZONE))
        {
            turnDirection = 1;
        }
        if (dotForward < (-1 + TURRET_TURN_DEAD_ZONE))
        {
            turnDirection = 1;
        }

        _turret.Rotate(0, 0, turnDirection * _baseTurnSpeed * Time.deltaTime);

        _pointingTowardsTarget = dotForward > TURRET_FIRE_ANGLE_ALLOWANCE;
    }


    #endregion

    #region Gunnery
    private void GunUpdate()
    {
        // The crew consistently loads the gun
        _loadTimer += (Time.deltaTime * loadingEffectiveness.value * _sectionState.effectivenessMultiplier);

        if (_target is null)
        {
            return;
        }

        bool gunLoaded = _loadTimer > _baseReloadTime;
        bool canSeeEnemy = CanSeeEnemy();
        bool allowedToFire = _ship.attackOrder == ShipInstance.AttackOrder.FireAtWill;
        bool withinRange = _distanceToTarget < _gunType.maxRange;
        
        if (gunLoaded && _pointingTowardsTarget && allowedToFire && withinRange && canSeeEnemy)
        {
            if (CanFireWithoutHittingSelf())
            {
                Shoot();
                float reloadTimeRange = TURRET_RELOAD_RANDOMISATION * _baseReloadTime;
                _loadTimer = Random.Range(-reloadTimeRange, reloadTimeRange);
            }    
        }
    }

    private void Shoot()
    {
        GameObject shellObj = Instantiate(_gunType.ammo._prefab);

        shellObj.transform.position = _shootPoint.position;
        shellObj.transform.position = new Vector3(shellObj.transform.position.x,
                                                 shellObj.transform.position.y,
                                                  -3f);
        shellObj.transform.rotation = _shootPoint.rotation;

        // Add random rotation
        float inaccuracy = Random.Range(-(_baseInaccuracy/2f), _baseInaccuracy/2f);
        shellObj.transform.Rotate(0, 0, inaccuracy);

        // Add in a random angle
        float randomAngle = Random.Range(-(_baseInaccuracy/2f), _baseInaccuracy/2f);

        
        ShellInstance shell = shellObj.GetComponent<ShellInstance>();
        shell.Fire(_gunType.ammo, _distanceToTarget, randomAngle, _baseShotVelocity);

        AudioEngine.PlaySound(_gunType.shootClipSet, transform.position);
    }

    private bool CanFireWithoutHittingSelf()
    {
        if (_fireAngleRestrictor.restrictorEnabled)
        {
            float localZ = _turret.localEulerAngles.z;

            bool result = _fireAngleRestrictor.IsGunAllowedToFire(localZ); 
            return result;
        }

        return true;
    }

    private bool CanSeeEnemy()
    {
        Vector2 direction = (_target.transform.position - _shootPoint.transform.position).normalized; 
        Vector2 origin = ((Vector2)_shootPoint.transform.position) + (direction * 3f);

        Debug.DrawRay(origin, direction * _gunType.maxRange, Color.yellow);

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, _gunType.maxRange);
        if (hit.collider != null)
        {
            // Debug.Log($"Hit: {hit.collider.gameObject.name}");

            if (hit.collider.tag == "ShipSection")
            {
                ShipSection section = hit.collider.GetComponent<ShipSection>();

                if (section != null)
                {
                    return true;
                }
            }
        }
        return false;
    }

    #endregion

    public ComponentEffectiveness[] GetComponentEffectivenesses()
    {
        List<ComponentEffectiveness> output = new List<ComponentEffectiveness>();

        output.Add(loadingEffectiveness);
        if (_aimComponent.usesFireControl == false)
        {
            // Guns which do not use fire control can be aimed!
            output.Add(_aimComponent.aimEffectiveness);
        }

        return output.ToArray();
    }
}

}