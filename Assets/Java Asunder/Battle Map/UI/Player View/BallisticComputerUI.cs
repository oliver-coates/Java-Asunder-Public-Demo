using System.Collections;
using System.Collections.Generic;
using Ships;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BallisticComputerUI : MonoBehaviour
{
    [SerializeField] private ShipInstance _playerShip;
    private FireControlInstance _fireControl;

    [Header("UI References:")]
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private TextMeshProUGUI _confidenceText;
    [SerializeField] private TextMeshProUGUI _targetNameText;
    [SerializeField] private TextMeshProUGUI _targetSpeedText;
    [SerializeField] private TextMeshProUGUI _targetDistanceText;
    [SerializeField] private Image _confidenceBar;
    [SerializeField] private TextMeshProUGUI _gunOrdersText;


    private void Awake()
    {
        ShipInstance.OnPlayerShipCreated += Initialise;
        GameMaster.OnAllShipsSpawned += ForceInitialUpdate;
    }

    private void OnDestroy()
    {
        ShipInstance.OnPlayerShipCreated -= Initialise;
        GameMaster.OnAllShipsSpawned -= ForceInitialUpdate;
    }



    private void Initialise(ShipInstance playerShip)
    {
        _playerShip = playerShip;

        // Hide the UI if there is no fire control on this ship
        _fireControl = _playerShip.fireControl;
        if (_fireControl == null)
        {
            _canvasGroup.alpha = 0f;
        }
        else
        {
            _canvasGroup.alpha = 1f;
        }
    }

    private void ForceInitialUpdate()
    {
        UpdateBallisticComputer();
        UpdateGunOrders();
    }

    private void Update()
    {
        // Don't update when the game is not running.
        if (GameMaster.BattleUnderway == false)
        {
            return;
        }

        // Don't update if a player ship has not been set
        if (_playerShip == null)
        {
            return;
        }

        if (_fireControl != null)
        {
            UpdateBallisticComputer();
        }

        UpdateGunOrders();
    }

    private void UpdateBallisticComputer()
    {
        float confidence = _fireControl.confidence;
        _confidenceText.text = $"{confidence:n0}%";
        _confidenceBar.fillAmount = confidence / 100f;

        ShipInstance targetShip = _playerShip.target;
        if (targetShip == null)
        {
            _targetDistanceText.text = "";
            _targetNameText.text = "";
            _targetSpeedText.text = "";
        }
        else
        {
            _targetNameText.text = targetShip.shipData.GetFullName();
            _targetDistanceText.text = $"{_fireControl.distanceToTarget:n0}m";
            _targetSpeedText.text = $"{targetShip.engine.speed:n1}kt";
        }
    }

    private void UpdateGunOrders()
    {
        Color rawColor = Color.magenta;
        string text = "";
        if (_playerShip.target == null)
        {
            text = "Idle";
            rawColor = Color.gray;
        }
        else
        {
            if (_playerShip.attackOrder == ShipInstance.AttackOrder.DoNotFire)
            {
                text = "Ready";
                rawColor = Color.yellow;
            }
            else if (_playerShip.attackOrder == ShipInstance.AttackOrder.FireAtWill)
            {
                text = "Fire at will";
                rawColor = Color.red;
            }
        }

        Color displayColor = (Color.white * 0.3f) + (rawColor * 0.7f); 
        string colorString = ColorUtility.ToHtmlStringRGB(displayColor);
        string displayText = $"<color=#{colorString}>{text}</color>";

        _gunOrdersText.text = $"Guns: {displayText}";
    }
}
