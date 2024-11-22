using System.Collections;
using System.Collections.Generic;
using Ships;
using UnityEngine;

public class ShipBridgeInstance : MonoBehaviour, IShipComponentInstance
{
    public ComponentEffectiveness turnEffectiveness;

    public ComponentEffectiveness[] GetComponentEffectivenesses()
    {
        ComponentEffectiveness[] componentEffectivenesses = new ComponentEffectiveness[1];

        componentEffectivenesses[0] = turnEffectiveness;

        return componentEffectivenesses;
    }

    public void Setup(ShipInstance ship, ComponentSlot componentSlot)
    {
        turnEffectiveness = new ComponentEffectiveness("Bridge", "Manning the helm");
    
        ship.bridge = this;

        // ...
    }
}
