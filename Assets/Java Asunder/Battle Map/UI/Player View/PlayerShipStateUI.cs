using System.Collections;
using System.Collections.Generic;
using Ships;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerShipStateUI : MonoBehaviour
{
    private ShipInstance _playerShip;

    [Header("UI References:")]
    [SerializeField] private TextMeshProUGUI _engineOrderText;
    [SerializeField] private Image _engineSpeedImage;
    [SerializeField] private TextMeshProUGUI _engineSpeedText;

    [SerializeField] private RectTransform _rudderTargetImage;
    [SerializeField] private TextMeshProUGUI _rudderOrderText;

    [SerializeField] private TextMeshProUGUI _speedText;
    private float _speedSmoothed;

    #region Initialisation & Destruction
    private void Awake()
    {
        ShipInstance.OnPlayerShipCreated += Initialise;
    }

    private void OnDestroy()
    {
        ShipInstance.OnPlayerShipCreated -= Initialise;
    }

    #endregion



    private void Initialise(ShipInstance playerShip)
    {
        _playerShip = playerShip;
    }

    private void Update()
    {
        if (GameMaster.BattleUnderway == false || _playerShip == null)
        {
            // UI does not need to update when the game is not running
            return;
        }

        // ENGINE:
        _engineOrderText.text = GetEngineOrder(_playerShip.targetSpeed);
        _engineSpeedImage.fillAmount = _playerShip.engine.currentEngineSpeed;
        _engineSpeedText.text = $"{_playerShip.engine.currentEngineSpeed * 100:n0}%";

        // RUDDER:
        _rudderOrderText.text = GetRudderOrder(_playerShip.rudder);
        float rudderLerp = 1f - ((1f + _playerShip.rudder) / 2f);
        _rudderTargetImage.anchoredPosition = Vector2.Lerp(new Vector2(-100, 0), new Vector2(100, 0), rudderLerp);
    
        // SPEED:
        float knots = _playerShip.engine.speed;
        if (GameMaster.BattlePaused == false)
        {
            // Only smooth when game is running
            _speedSmoothed = Mathf.Lerp(_speedSmoothed, knots, Time.deltaTime);
        }
        _speedText.text = $"{_speedSmoothed:0.#}kt";
    }

    #region Decorators

    public string GetEngineOrder(float engineTargetSpeed)
    {

        if (engineTargetSpeed < 0.05f)
        {
            return "Stand By";
        }
        else if (engineTargetSpeed >= 0.05f && engineTargetSpeed < 0.3f)
        {
            return "Dead Slow";
        }
        else if (engineTargetSpeed >= 0.3f && engineTargetSpeed < 0.6f)
        {
            return "Slow";
        }
        else if (engineTargetSpeed >= 0.6f && engineTargetSpeed < 0.9f)
        {
            return "Half";
        }
        else
        {
            return "Full Steam Ahead";
        }
    }

    
    public string GetRudderOrder(float rudderTargetAmount)
    {
        if (rudderTargetAmount < -0.75f)
        {
            return "Hard to Starboard";

        }
        else if (rudderTargetAmount < -0.5f)
        {
            return "Half Starboard";
        }
        else if (rudderTargetAmount < -0.05)
        {
            return "Slow Starboard";
        }
        else if (rudderTargetAmount < 0.05f)
        {
            return "Dead Ahead";
        }
        else if (rudderTargetAmount < 0.5f)
        {
            return "Slow Port";
        }
        else if (rudderTargetAmount < 0.75f)
        {
            return "Half Port";
        }
        else
        {
            return "Hard to Port";
        }
        
    }

    #endregion
}
