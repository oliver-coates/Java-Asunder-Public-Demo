 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ships;
using KahuInteractive.HassleFreeSaveLoad;
using System;

public class ShipManagementUI : MonoBehaviour
{
    [SerializeField] private GameObject _UIPrefab;
    [SerializeField] private List<IShipUI> _shipUIs;

    [SerializeField] private RectTransform _contentZoneRectTransform;

    // Move this to some global settings file later:
    [SerializeField] private Nation _defaultNation;

    private void Awake()
    {
        _shipUIs = new List<IShipUI>();
        SessionMaster.OnShipCountChanged += DrawShipUI;
    }

    private void OnDestroy()
    {
        SessionMaster.OnShipCountChanged -= DrawShipUI;
    } 

    private void DrawShipUI()
    {
        int shipUIIndex = 0;
        foreach (Ship ship in SessionMaster.Ships)
        {
            IShipUI shipUI;
            if (_shipUIs.Count > shipUIIndex)
            {
                // We already have a ship UI we can reuse:
                shipUI = _shipUIs[shipUIIndex];
            }
            else
            {
                // Create a new one
                shipUI = Instantiate(_UIPrefab, _contentZoneRectTransform.transform).GetComponent<IShipUI>();
                _shipUIs.Add(shipUI);
            }
     
            // Will ignore if already assigned
            shipUI.AssignToShip(ship);

            Vector2 pos = new Vector2(0, shipUIIndex * -70);
            shipUI.GetRectTransform().anchoredPosition = pos;

            shipUIIndex++;
        }


        // Cull any extra UI elements
        int extraUIElements = _shipUIs.Count - SessionMaster.Ships.Count;
        extraUIElements = Math.Clamp(extraUIElements, 0, 100); 

        for (int toDeleteIndex = 0; toDeleteIndex < extraUIElements; toDeleteIndex++)
        {
            IShipUI toDelete = _shipUIs[shipUIIndex];
            _shipUIs.RemoveAt(shipUIIndex);

            Destroy(toDelete.GetGameObject());
        }
    }

    public void CreateNewShip()
    {
        ObjectRequestHandler.RequestShipClass(CreateNewShip);
    }

    public void CreateNewShip(ShipClassType newShipClass)
    {
        // Debug.Log($"Creating new ship of type: {newShipClass.name}");

        // Create a new ship from provided type
        Ship newShip = SaveLoad.InstantiateSerializedObject<Ship>("Ship In Play");

        newShip.Initialise(newShipClass, _defaultNation);

        SessionMaster.AddShip(newShip);
    }
}
