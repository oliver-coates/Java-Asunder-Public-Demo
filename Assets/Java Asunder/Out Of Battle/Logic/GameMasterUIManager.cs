using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMasterUIManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup _outOfBattleCanvas;
    [SerializeField] private CanvasGroup _battleCanvas;


    private void Awake()
    {
        GameMaster.OnBattleStart += BattleStart;
        GameMaster.OnBattleEnd += BattleEnd;

        BattleEnd();
    }

    private void OnDestroy()
    {
        GameMaster.OnBattleStart -= BattleStart;
        GameMaster.OnBattleEnd -= BattleEnd;
    }

    private void BattleStart()
    {
        _battleCanvas.alpha = 1f;
        _battleCanvas.blocksRaycasts = true;
        _battleCanvas.interactable = true;

        _outOfBattleCanvas.alpha = 0f;
        _outOfBattleCanvas.blocksRaycasts = false;
        _outOfBattleCanvas.interactable = false;
    }

    private void BattleEnd()
    {
        _battleCanvas.alpha = 0f;
        _battleCanvas.blocksRaycasts = false;
        _battleCanvas.interactable = false;

        _outOfBattleCanvas.alpha = 1f;
        _outOfBattleCanvas.blocksRaycasts = true;
        _outOfBattleCanvas.interactable = true;
    }
}
