using System;
using System.Collections;
using System.Collections.Generic;
using Ships;
using UnityEngine;

public class ShipBuilder : MonoBehaviour
{
    private readonly Vector3 ENEMY_SPAWN_POSITION = new Vector3(0, 500, 0);
    private readonly Vector3 ENEMY_SPAWN_OFFSET = new Vector3(50, 0, 0); 
    private const float ROTATION_SPEED = 125f;

    public static event Action OnFinalShipPlaced;

    private int numShipsSpawned;

    [SerializeField] private Camera _developerCamera;

    private Queue<ShipInstance> _shipsToPlace;
    private ShipInstance _shipBeingPlaced;
    private bool _isPlacing;


    #region Initialisation & Destruction
    private void Awake()
    {
        GameMaster.OnReadyForShipSpawn += SpawnShip;
        GameMaster.OnBattleStart += ResetShipSpawnedNumber;
        GameMaster.OnAllShipsSpawned += TryPlaceNextShip;

        _shipsToPlace = new Queue<ShipInstance>();
    }

    private void OnDestroy()
    {
        GameMaster.OnReadyForShipSpawn -= SpawnShip;
        GameMaster.OnBattleStart -= ResetShipSpawnedNumber;
        GameMaster.OnAllShipsSpawned -= TryPlaceNextShip;
    }

    #endregion

    private void Update()
    {
        if (_isPlacing)
        {
            float rotationAmount = 0f;
            if (Input.GetKey(KeyCode.Q))
            {
                rotationAmount = -ROTATION_SPEED;
            }
            else if (Input.GetKey(KeyCode.E))
            {
                rotationAmount = ROTATION_SPEED;
            }

            // Rotate through scroll wheel
            _shipBeingPlaced.transform.Rotate(0, 0, rotationAmount * Time.deltaTime);

            // Find mosue position in world. clamp Z to correct height
            Vector3 mousePositionInWorld = _developerCamera.ScreenToWorldPoint(Input.mousePosition); 
            mousePositionInWorld.z = 0f;

            // Move to mouse position
            _shipBeingPlaced.transform.position = mousePositionInWorld;  

            if (Input.GetMouseButtonDown(0))
            {
                // Stop placing this ship
                TryPlaceNextShip();
            }


        }
    }

    #region Ship Spawning
    private void ResetShipSpawnedNumber()
    {
        numShipsSpawned = 0;
    }

    private void SpawnShip(Ship ship)
    {
        ShipInstance newShip = Instantiate(ship.shipClass.prefab, transform).GetComponent<ShipInstance>();

        newShip.Setup(ship);
        _shipsToPlace.Enqueue(newShip);

        Vector3 spawnPosition;
        if (newShip.shipData == SessionMaster.PlayerShip)
        {
            // Player ship starts at 0,0.0
            spawnPosition = Vector3.zero;
        }
        else
        {
            spawnPosition = ENEMY_SPAWN_POSITION + (ENEMY_SPAWN_OFFSET * numShipsSpawned);
            numShipsSpawned++;
        }

        newShip.transform.localPosition = spawnPosition;
        newShip.transform.localRotation = Quaternion.identity;

    }
    #endregion

    #region Ship Placemment
    private void TryPlaceNextShip()
    {
        if (_shipsToPlace.TryDequeue(out ShipInstance nextInstance))
        {
            _isPlacing = true;
            PlaceShip(nextInstance);        
        }
        else
        {
            // Finished placing ships
            _isPlacing = false;

            _shipBeingPlaced = null;
            _shipsToPlace = new Queue<ShipInstance>();

            OnFinalShipPlaced?.Invoke();
        }
    }

    private void PlaceShip(ShipInstance toPlace)
    {
        _shipBeingPlaced = toPlace;
    }


    #endregion
}
