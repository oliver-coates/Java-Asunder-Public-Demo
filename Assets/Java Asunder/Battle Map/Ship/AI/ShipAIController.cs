using System.Collections;
using System.Collections.Generic;
using Ships;
using UnityEngine;

public class ShipAIController : BoardPiece
{
    #region Tuners

    public const float OVERSTEER_AMOUNT = 1.5f;

    #endregion

    ShipInstance _ship;

    [Header("State:")]
    [SerializeField] private Vector3 _targetDestination;
    [SerializeField] private ShipInstance _targetShip;

    private void Awake()
    {
        _ship = GetComponent<ShipInstance>();

        _targetDestination = new Vector3(Random.Range(-100f, 100f), 0, Random.Range(-100f, 100f));
    
    }

    protected override void GameTick()
    {
    }

    protected override void Initialise()
    {
    }

    protected override void UpdateTick()
    {
        SetRudderTowardsDestination();

        UpdateDirectionLineRenderer();
        UpdateEngineSpeed();
    }

    private void SetRudderTowardsDestination()
    {
        Vector3 directionToTarget = transform.position - _targetDestination;

        // Determine if the destination is left or right of the ship
        float dot = Vector3.Dot(transform.right, directionToTarget.normalized);

        // Add in 'oversteer'
        dot = Mathf.Clamp(dot * OVERSTEER_AMOUNT, -1f, 1f);

        _ship.rudder = dot;
    }

    private void UpdateEngineSpeed()
    {
        _ship.targetSpeed = 1f;
    }

    private void UpdateDirectionLineRenderer()
    {
        if (_ship.shipData.isSelectedByGameMaster)
        {
            _ship.shipDirectionLine.enabled = true;

            Vector3[] lineRendererPositons = new Vector3[2];
            lineRendererPositons[0] = transform.position;
            lineRendererPositons[1] = _targetDestination;

            _ship.shipDirectionLine.SetPositions(lineRendererPositons);
        }
        else
        {
            _ship.shipDirectionLine.enabled = false;
        }
    }

    public void SetDestination(Vector3 destination)
    {
        _targetDestination = destination;
    }

    public void SetTarget(ShipInstance ship)
    {
        ship.SetTarget(ship);
    }
}
