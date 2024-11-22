using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BoardPiece : MonoBehaviour
{
    public static event Action<BoardPiece> OnBoardPieceInitialised;
    public static event Action<BoardPiece> OnBoardPieceDestroyed;

    protected bool _battleUnderway = false;
    protected bool _gamePaused = true;


    #region Initialisation & Destruction
    private void Start()
    {
        OnBoardPieceInitialised.Invoke(this);
        Initialise();
    }

    private void OnDestroy()
    {
        OnBoardPieceDestroyed?.Invoke(this);
    }
    #endregion

    private void Update()
    {
        if (!GameMaster.BattleUnderway)
        {
            return;
        }

        if (!GameMaster.BattlePaused)
        {
            GameTick();
        }

        UpdateTick();
    }




    /// <summary>
    /// Called when this board piece is created.
    /// </summary>
    protected abstract void Initialise();

    /// <summary>
    /// Called every frame
    /// </summary>
    protected abstract void UpdateTick();

    /// <summary>
    /// Called every frame in which the game is unpaused.
    /// </summary>
    protected abstract void GameTick();

}
