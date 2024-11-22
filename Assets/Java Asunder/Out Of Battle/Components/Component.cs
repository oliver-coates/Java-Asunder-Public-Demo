using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Ships
{



public class ShipComponent : ScriptableObject
{
    [Header("Component Slot Data:")]
    public ShipComponentType componentType;
    public Nation sourceNation;
    public int price = 100;
}

}