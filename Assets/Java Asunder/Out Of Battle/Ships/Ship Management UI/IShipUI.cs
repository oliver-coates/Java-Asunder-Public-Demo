using System.Collections;
using System.Collections.Generic;
using Ships;
using UnityEngine;

public interface IShipUI
{
    public GameObject GetGameObject();

    public RectTransform GetRectTransform();

    public void AssignToShip(Ship ship);
}
