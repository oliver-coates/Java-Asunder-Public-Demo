using System.Collections;
using System.Collections.Generic;
using Ships;
using UnityEngine;

public class ArmourInstance : BoardPiece, IShipComponentInstance
{
    public ComponentEffectiveness[] GetComponentEffectivenesses()
    {
        return null;
    }

    public void Setup(ShipInstance ship, ComponentSlot componentSlot)
    {
        ship.armourType = (ArmourType) componentSlot.component;
    }

    protected override void GameTick() {}

    protected override void Initialise() {}

    protected override void UpdateTick() {}
}
