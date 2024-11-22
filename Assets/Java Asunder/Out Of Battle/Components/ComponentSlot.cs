using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ships
{



public class ComponentSlot : MonoBehaviour
{
    [Header("Decorative")]
    public string slotName;

    [Header("The section this component is in (Auto assigned at runtime)")]
    public ShipSection shipSection;

    [Header("Type of component that this is: (i.e. Medium Gun)")]
    public ShipComponentType componentType;
    
    [Header("The actual component that this is (i.e. 40mm gun)")]
    public ShipComponent component;

    [Header("The instaniated script related to this component:")]
    public IShipComponentInstance componentInstance;
    

    public void Initialise(ShipInstance ship, ShipSection shipSection)
    {
        this.shipSection = shipSection;

        if (componentType.genericPrefab == null || component == null)
        {
            return;
        }

        // Ensure that this component type matches this slots component type
        // E.g. - prevent a light gun from being added to a heavy gun slot
        if (component.componentType != componentType)
        {
            Debug.LogError($"The provided component {component.name}'s type ({component.componentType.name}) does not match this slots component type ({componentType.name})");
            return;
        }
    
        componentInstance = Instantiate(componentType.genericPrefab, transform.position, transform.rotation, transform).GetComponent<IShipComponentInstance>();    

        if (componentInstance == null)
        {
            Debug.LogError($"Ship component instance not found on gameobject {name}");
            return;
        }
    
        componentInstance.Setup(ship, this);
    }

}

}