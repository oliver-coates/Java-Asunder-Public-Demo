using System;
using System.Collections;
using System.Collections.Generic;
using KahuInteractive.UIHelpers;
using Ships;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShipSelectableUI : MonoBehaviour, IShipUI
{
    public static event Action<Ship> OnShipSelected;

    private readonly Color DESELCTED_COLOR = new Color(0.75f, 0.75f, 0.75f, 0.75f);
    private readonly Color SELECTED_COLOR = new Color(1f, 1f, 1f, 1f);

    private Ship _ship;

    [Header("UI References:")]
    public RectTransform rectTransform;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private Image _flagImage;
    [SerializeField] private Image _playerShipImage;

    public void AssignToShip(Ship ship)
    {
        if (_ship == ship)
        {
            // We are already assigned, return
            return;
        }

        // Unsubscribe from old ship events:
        if (_ship != null)
        {
            _ship.OnChange -= RefreshUI;
        }

        // This is a new ship: Go Ahead with reassigning.
        _ship = ship;

        _ship.OnChange += RefreshUI;

        RefreshUI();
    }

    private void OnDestroy()
    {
        _ship.OnChange -= RefreshUI;
    }

    private void RefreshUI()
    {
        _nameText.text = _ship.GetFullName();
        _flagImage.sprite = _ship.nation.flag;

        _descriptionText.text = _ship.GetFullDescription();


        if (_ship.isSelectedByGameMaster)
        {
            _flagImage.color = SELECTED_COLOR;
            _nameText.color = SELECTED_COLOR;
            _descriptionText.color = SELECTED_COLOR;
        }
        else
        {
            _flagImage.color = DESELCTED_COLOR;
            _nameText.color = DESELCTED_COLOR;
            _descriptionText.color = DESELCTED_COLOR;
        }

        if (_ship.isPlayerShip)
        {
            _playerShipImage.enabled = true;
        }
        else
        {
            _playerShipImage.enabled = false;
        }
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public RectTransform GetRectTransform()
    {
        return rectTransform;
    }

    public void Select()
    {
        OnShipSelected?.Invoke(_ship);
    }
}
