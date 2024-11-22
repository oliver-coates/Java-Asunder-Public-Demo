using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStateUI : MonoBehaviour
{
    private const float VIGNETTE_FADE_TIME = 0.5f; 


    [Header("UI Refererences:")]
    [SerializeField] private Image _vignette;


    private void Awake()
    {
        _vignette.CrossFadeAlpha(0, 0.01f, true);        
    
        GameMaster.OnTimePause += GamePause;
        GameMaster.OnTimePlay += GamePlay;
    
        // The game starts paused
        GamePause();
    }



    private void GamePlay()
    {
        _vignette.CrossFadeAlpha(0, VIGNETTE_FADE_TIME, true);
    }

    private void GamePause()
    {
        _vignette.CrossFadeAlpha(1, VIGNETTE_FADE_TIME, true);
    }
}
