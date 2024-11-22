using System.Collections;
using System.Collections.Generic;
using Ships;
using UnityEngine;

public class ShipGunInitialiser : MonoBehaviour, IShipComponentInstance
{
    public ComponentEffectiveness[] GetComponentEffectivenesses()
    {
        return new ComponentEffectiveness[0];
    }

    public void Setup(ShipInstance ship, ComponentSlot componentSlot)
    {
        ShipGunType gunType = (ShipGunType) componentSlot.component;
        GameObject spawnedTurretObj = Instantiate(gunType.prefab, transform.parent);
        ShipGunScript gunScript = spawnedTurretObj.GetComponent<ShipGunScript>();

        gunScript.Setup(ship, componentSlot);
        componentSlot.componentInstance = gunScript;
    
        Destroy(gameObject);
    }
}
