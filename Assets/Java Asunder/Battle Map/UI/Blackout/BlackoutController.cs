using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class BlackoutController : MonoBehaviour
{
    private const float FADE_TIME = 2f;

    [SerializeField] private CanvasGroup _canvasGroup;


    private bool fadingIn;
    private float timer;

    private void Awake()
    {
        GameMaster.OnBattleStart += ForceFadeIn;
        ShipBuilder.OnFinalShipPlaced += StartFadeOut;

        _canvasGroup.alpha = 0f;
    }

    private void OnDestroy()
    {
        GameMaster.OnBattleStart -= ForceFadeIn;
        ShipBuilder.OnFinalShipPlaced -= StartFadeOut;

    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.E))
        {
            if (fadingIn)
            {
                StartFadeOut();
            }
            else
            {
                StartFadeIn();
            }
        }

        if (fadingIn)
        {
            timer += Time.deltaTime;
        }
        else
        {
            timer -= Time.deltaTime;
        }

        timer = Mathf.Clamp(timer, 0, FADE_TIME);
        _canvasGroup.alpha = timer / FADE_TIME; 
    }

    private void StartFadeIn()
    {
        fadingIn = true;
    }

    private void StartFadeOut()
    {
        fadingIn = false;
    }

    private void ForceFadeIn()
    {
        fadingIn = true;
        timer = FADE_TIME;
    }



}
