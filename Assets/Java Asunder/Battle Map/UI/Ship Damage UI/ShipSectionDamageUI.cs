using System.Collections;
using System.Collections.Generic;
using Ships;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShipSectionDamageUI : MonoBehaviour
{

    [SerializeField] private ShipSection _shipSection;

    [Header("UI References:")]
    [SerializeField] private TextMeshProUGUI _sectionNameText;
    [SerializeField] private TextMeshProUGUI _sectionStateText;

    [SerializeField] private Image _integrityPercentImage;
    [SerializeField] private TextMeshProUGUI _integrityPercentText;

    public void Setup(ShipSection shipSection)
    {
        _shipSection = shipSection;

        _sectionNameText.text = _shipSection.sectionName;
        
        RefreshText();
    }

    private void Update()
    {
        if (GameMaster.BattleUnderway == false || GameMaster.BattlePaused)
        {
            return;
        }

        if (_shipSection == null)
        {
            return;
        }

        RefreshText();
    }

    private void RefreshText()
    {
        int integrityAsInt = (int)_shipSection.state.integrity;
        _integrityPercentText.text = $"{integrityAsInt}%";

        float integrityAsRange = integrityAsInt / 100f;
        _integrityPercentImage.fillAmount = integrityAsRange;


        string sectionDescriptionString = "";
        if (_shipSection.state.damages.Count == 0)
        {
            sectionDescriptionString = "Fine";
        }
        else
        {
            foreach (DamageInstance damage in _shipSection.state.damages)
            {
                sectionDescriptionString += damage.GetDescription() + ", ";
            }
        }

        _sectionStateText.text = sectionDescriptionString;
    }
}
