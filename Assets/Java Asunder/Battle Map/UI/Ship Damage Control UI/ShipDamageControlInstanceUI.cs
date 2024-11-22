using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShipDamageControlInstanceUI : MonoBehaviour
{
    private DamageControlInstance _damageControl;

    [Header("References:")]
    [SerializeField] private TextMeshProUGUI _headerText;
    [SerializeField] private TextMeshProUGUI _descriptionText;

    public void Setup(DamageControlInstance damageControl)
    {
        _damageControl = damageControl;

        _headerText.text = damageControl.GetName();
    }

    public void Update()
    {
        if (_damageControl.damageToTarget == null)
        {
            _descriptionText.text = "Standby";
        }
        else
        {
            string damageName = _damageControl.damageToTarget.damageEffect.damageName;
            string sectionName = _damageControl.sectionToTarget.sectionName;

            _descriptionText.text = $"Fighting {damageName} in the {sectionName}";
        }

        
    }


}
