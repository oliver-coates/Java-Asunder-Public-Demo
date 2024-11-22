using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Config : MonoBehaviour
{
    [SerializeField] private ShipConfiguration _shipConfiguration;

    private static Config _Instance;

    public static ShipConfiguration Ship
    {
        get
        {
            return _Instance._shipConfiguration;
        }
    }

    private void Awake()
    {
        _Instance = this;
    }
}
