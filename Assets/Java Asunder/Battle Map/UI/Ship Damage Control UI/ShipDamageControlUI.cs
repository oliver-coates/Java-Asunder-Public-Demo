using System.Collections;
using System.Collections.Generic;
using Ships;
using UnityEngine;



public class ShipDamageControlUI : MonoBehaviour
{

    private const float UI_ELEMENT_SPACING = 80f;

    [SerializeField] private bool _autoAssignToPlayerShip;
    [SerializeField] private ShipInstance _shipInstance;
   
    private ShipDamageControlInstanceUI[] _damageControlElements;

    [Header("References:")]
    [SerializeField] private RectTransform _rct;
    [SerializeField] private Transform _contentHolder;
    [SerializeField] private GameObject _shipSectionDamagePrefab;


    private void Awake()
    {
        if (_autoAssignToPlayerShip)
        {
            ShipInstance.OnPlayerShipCreated += AssignToShip;
        }
    
        _damageControlElements = new ShipDamageControlInstanceUI[0];
    }

    private void OnDestroy()
    {
        if (_autoAssignToPlayerShip)
        {
            ShipInstance.OnPlayerShipCreated -= AssignToShip;
        }
    }





    public void AssignToShip(ShipInstance shipInstance)
    {
        DeleteOldElements();    
    
        _damageControlElements = new ShipDamageControlInstanceUI[shipInstance.sections.Length];

        int i = 0;        
        foreach (DamageControlInstance damageControl in shipInstance.damageControls)
        {
            ShipDamageControlInstanceUI newUIElement = Instantiate(_shipSectionDamagePrefab, _contentHolder).GetComponent<ShipDamageControlInstanceUI>();
            newUIElement.Setup(damageControl);

            // Position:
            RectTransform rct = newUIElement.GetComponent<RectTransform>();
            rct.anchoredPosition = new Vector2(0, i * UI_ELEMENT_SPACING);
            i++;
        }

        _rct.sizeDelta = new Vector2(_rct.sizeDelta.x, 10 + (i * UI_ELEMENT_SPACING));
    }

    private void DeleteOldElements()
    {
        for (int i = 0; i < _damageControlElements.Length; i++)
        {
            Destroy(_damageControlElements[i]);
        }

        _damageControlElements = new ShipDamageControlInstanceUI[0];
    }



}
