using System.Collections;
using System.Collections.Generic;
using Ships;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShipSelectionUI : MonoBehaviour
{

    [Header("References:")]
    [SerializeField] private TextMeshProUGUI _rudderText;
    [SerializeField] private Slider _rudderSlider;

    [SerializeField] private TextMeshProUGUI _speedText;
    [SerializeField] private Slider _speedSlider;


    private void Awake()
    {
        GameMaster.OnBattleStart += ResetUI;

        ResetUI();
    }

    private void OnDestroy()
    {
        GameMaster.OnBattleStart += ResetUI;
    }

    private void ResetUI()
    {
        _rudderSlider.SetValueWithoutNotify(0);
        _rudderText.text = "0";

        _speedSlider.SetValueWithoutNotify(EngineScript.ENGINE_SPEED_STARTING);
        _speedText.text = EngineScript.ENGINE_SPEED_STARTING.ToString();

    }


    public void UpdateRudderValue()
    {
        if (!GameMaster.BattleUnderway)
        {
            return;
        }

        float rudder = (_rudderSlider.value / 10f) / -1f;

        _rudderText.text = rudder.ToString();

        SessionMaster.PlayerShip.instance.rudder = rudder;
    }

    public void UpdateSpeedValue()
    {
        if (!GameMaster.BattleUnderway)
        {
            return;
        }

        float targetSpeedActual = _speedSlider.value;

        _speedText.text = _speedSlider.value.ToString();

        SessionMaster.PlayerShip.instance.targetSpeed = targetSpeedActual;
    }

}
