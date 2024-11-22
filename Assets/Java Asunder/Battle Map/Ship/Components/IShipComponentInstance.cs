using System;
using System.Collections;
using System.Collections.Generic;
using Ships;
using UnityEngine;

public interface IShipComponentInstance
{
    public abstract void Setup(ShipInstance ship, ComponentSlot componentSlot);

    public abstract ComponentEffectiveness[] GetComponentEffectivenesses();
}
