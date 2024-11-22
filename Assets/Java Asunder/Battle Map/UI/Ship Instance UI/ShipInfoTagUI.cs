using System.Collections;
using System.Collections.Generic;
using Ships;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Battlemap
{

public class ShipInfoTagUI : MonoBehaviour
{
    private ShipInstance _ship;
    private bool _initialised;


    [Header("UI References")]
    [SerializeField] private Image _flagImage;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _descriptionText;


    public void Initialise(ShipInstance ship)
    {
        _ship = ship;

        _initialised = true;

        _flagImage.sprite = ship.shipData.nation.flag;
        _nameText.text = ship.shipData.GetFullName();
        _descriptionText.text = ship.shipData.GetFullDescription();
    
        _ship.OnStartedSinking += DestroyMe;
    }

    private void Update()
    {
        if (!_initialised)
        {
            return;
        }

        transform.position = _ship.transform.position + new Vector3(0, _ship.UIDisplayOffset, 0);
    }

    private void DestroyMe()
    {
        Destroy(gameObject);
    }
}

}