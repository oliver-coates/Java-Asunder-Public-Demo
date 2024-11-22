using System.Collections;
using System.Collections.Generic;
using Ships;
using UnityEngine;



public class ShipDamageUI : MonoBehaviour
{
    private const float UI_ELEMENT_SPACING = 100f;

    [SerializeField] private bool _autoAssignToPlayerShip;
    [SerializeField] private ShipInstance _shipInstance;
   
    private ShipSectionDamageUI[] _sectionDamageUIElements;

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
    
        _sectionDamageUIElements = new ShipSectionDamageUI[0];
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
    
        _sectionDamageUIElements = new ShipSectionDamageUI[shipInstance.sections.Length];

        int i = 0;        
        foreach (ShipSection section in shipInstance.sections)
        {
            ShipSectionDamageUI newUIElement = Instantiate(_shipSectionDamagePrefab, _contentHolder).GetComponent<ShipSectionDamageUI>();
            newUIElement.Setup(section);

            // Position:
            RectTransform rct = newUIElement.GetComponent<RectTransform>();
            rct.anchoredPosition = new Vector2(0, i * UI_ELEMENT_SPACING);
            i++;
        }

        _rct.sizeDelta = new Vector2(_rct.sizeDelta.x, 10 + (i * UI_ELEMENT_SPACING));
    }

    private void DeleteOldElements()
    {
        for (int i = 0; i < _sectionDamageUIElements.Length; i++)
        {
            Destroy(_sectionDamageUIElements[i]);
        }

        _sectionDamageUIElements = new ShipSectionDamageUI[0];
    }


}
