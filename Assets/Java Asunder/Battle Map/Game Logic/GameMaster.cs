using System;
using System.Collections;
using System.Collections.Generic;
using Ships;
using UnityEngine;

public class GameMaster : MonoBehaviour
{


    #region Game Event Callbacks
    public static event Action OnBattleStart;
    public static event Action OnBattleEnd;
    public static event Action OnTimePause;
    public static event Action OnTimePlay;
    
    public static event Action<Ship> OnReadyForShipSpawn;
    public static event Action OnAllShipsSpawned;
    #endregion

    [Header("Game State:")]
    private static GameMaster _Instance;

    // Is there a battle currently underway
    [SerializeField] private bool _battleUnderway;
    public static bool BattleUnderway
    {
        get
        {
            return _Instance._battleUnderway;
        }	
    }

    // Is time paused?
    [SerializeField] private bool _battlePaused;
    public static bool BattlePaused
    {
        get
        {
            return _Instance._battlePaused;
        }	
    }

    [SerializeField] private List<BoardPiece> _boardPieces;

    [SerializeField] private Ship _selectedShip;
    public static Ship SelectedShip
    {
        get
        {
            return _Instance._selectedShip;
        }	
    }

    [SerializeField] private DamageType[] _damageTypes;
    public static DamageType[] damageTypes
    {
        get
        {
            return _Instance._damageTypes;
        }
    }


    #region Initialistion
    private void Awake()
    {
        _Instance = this;

        _boardPieces = new List<BoardPiece>();
        BoardPiece.OnBoardPieceInitialised += AddBoardPiece;
        BoardPiece.OnBoardPieceDestroyed += RemoveBoardPiece;
        ShipSelectableUI.OnShipSelected += ShipSelected;

        DisasterUI.OnStartShowing += PauseBattle;
        DisasterUI.OnFinishedShowing += PlayBattle;
    }

    private void OnDestroy()
    {
        BoardPiece.OnBoardPieceInitialised -= AddBoardPiece;
        BoardPiece.OnBoardPieceDestroyed -= RemoveBoardPiece;
        ShipSelectableUI.OnShipSelected -= ShipSelected;

        DisasterUI.OnStartShowing -= PauseBattle;
        DisasterUI.OnFinishedShowing -= PlayBattle;
    }



    #endregion

    #region Runtime

    private void Update()
    {
        if (BattlePaused == false)
        {
            TickPlayerCharacters();
        }

        UpdateDebug();
    }

    private void TickPlayerCharacters()
    {
        foreach (PlayerCharacter playerCharacter in SessionMaster.PlayerCharacters)
        {
            playerCharacter.Tick(Time.deltaTime);
        }
    }

    #endregion

    #region Board Pieces
    private void AddBoardPiece(BoardPiece piece)
    {
        _boardPieces.Add(piece);
    }

    private void RemoveBoardPiece(BoardPiece piece)
    {
        _boardPieces.Remove(piece);
    }
    #endregion

    #region Battle start & end

    public void StartBattle()
    {
        if (SessionMaster.PlayerShip == null)
        {
            // Prevent a battle from being started IF there is no player ship
            return;
        }

        // Delete all old peices
        ClearBoard();

        OnBattleStart?.Invoke();
        _battleUnderway = true;
        
        // Ensure the game starts paused.
        PauseBattle();

        // Ensure no ship in selected
        _selectedShip?.SetIsSelected(false);
        _selectedShip = null;

        SpawnShips();
    }

    public void EndBattle()
    {
        _battleUnderway = false;
        OnBattleEnd?.Invoke();
    }

    /// <summary>
    /// Destroys all board peices
    /// </summary>
    private void ClearBoard()
    {
        foreach (BoardPiece piece in _boardPieces)
        {
            Destroy(piece);
        }
    }

    private void SpawnShips()
    {
        List<Ship> shipsToSpawn = SessionMaster.ActiveShips;
        foreach (Ship ship in shipsToSpawn)
        {
            OnReadyForShipSpawn?.Invoke(ship);
        }

        OnAllShipsSpawned?.Invoke();
    }

    #endregion

    #region Battle time control
    private void PlayBattle()
    {
        if (_battleUnderway == false)
        {
            Debug.Log($"Attempting to pause a battle which is not running");
            return;
        }
        _battlePaused = false;

        OnTimePlay.Invoke();
    }

    private void PauseBattle()
    {
        if (_battleUnderway == false)
        {
            Debug.Log($"Attempting to play a battle which is not running");
            return;
        }
        _battlePaused = true;

        OnTimePause.Invoke();
    }
    #endregion

    #region Ship Selection

    private void ShipSelected(Ship ship)
    {
        _selectedShip?.SetIsSelected(false);
        _selectedShip = ship;
        _selectedShip.SetIsSelected(true);
    }


    #endregion

    #region Debug

    

    private void UpdateDebug()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            CheckForDebugCommand();
        }
    }

    private void CheckForDebugCommand()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _battleUnderway)
        {
            if (_battlePaused)
            {
                PlayBattle();
            }     
            else
            {
                PauseBattle();
            }
        }
    }
    #endregion
}
