using System;
using System.Collections;
using System.Collections.Generic;
using Ships;
using UnityEngine;
using KahuInteractive.UIHelpers;

public static class ObjectRequestHandler
{
    
    private static BasicSelection.Category<ShipClassType>[] _shipTypeCategories;
    private static BasicSelection.Category<Nation> _nations;

    public static void Initialise()
    {
        AssembleShipTypesIntoCategories();
        AssembleNations();
    }

    private static void AssembleShipTypesIntoCategories()
    {
        _shipTypeCategories = new BasicSelection.Category<ShipClassType>[3];
        
        int i = 0;

        // Hard coding all types lol - this sucks - Fix this later:        
        #region Freighter
        ShipClassType[] freighterTypes = Resources.LoadAll<ShipClassType>("Ship Classes/Freighters");
        BasicSelection.Option<ShipClassType>[] freighterOptions = new BasicSelection.Option<ShipClassType>[freighterTypes.Length]; 
        i = 0;
        foreach (ShipClassType freighterType in freighterTypes)
        {
            freighterOptions[i] = new BasicSelection.Option<ShipClassType>(freighterType.name, freighterType);
            
            i++;
        }

        BasicSelection.Category<ShipClassType> freighterCategory = new BasicSelection.Category<ShipClassType>("Freighters", freighterOptions);
        _shipTypeCategories[0] = freighterCategory;
        #endregion
    
        #region Frigate
        ShipClassType[] frigateTypes = Resources.LoadAll<ShipClassType>("Ship Classes/Frigates");
        BasicSelection.Option<ShipClassType>[] frigateOptions = new BasicSelection.Option<ShipClassType>[frigateTypes.Length]; 
        i = 0;
        foreach (ShipClassType frigateType in frigateTypes)
        {
            frigateOptions[i] = new BasicSelection.Option<ShipClassType>(frigateType.name, frigateType);
            
            i++;
        }

        BasicSelection.Category<ShipClassType> frigateCategory = new BasicSelection.Category<ShipClassType>("Frigate", frigateOptions);
        _shipTypeCategories[1] = frigateCategory;
        #endregion
    
        #region Destroyers
        ShipClassType[] destroyerTypes = Resources.LoadAll<ShipClassType>("Ship Classes/Destroyers");
        BasicSelection.Option<ShipClassType>[] destroyerOptions = new BasicSelection.Option<ShipClassType>[destroyerTypes.Length]; 
        i = 0;
        foreach (ShipClassType destroyerType in destroyerTypes)
        {
            destroyerOptions[i] = new BasicSelection.Option<ShipClassType>(destroyerType.name, destroyerType);
            
            i++;
        }

        BasicSelection.Category<ShipClassType> destroyerCategory = new BasicSelection.Category<ShipClassType>("Destroyers", destroyerOptions);
        _shipTypeCategories[2] = destroyerCategory;
        #endregion
    }
    
    private static void AssembleNations()
    {
        Nation[] nations = Resources.LoadAll<Nation>("Nations");

        BasicSelection.Option<Nation>[] options = new BasicSelection.Option<Nation>[nations.Length];
        int index = 0;
        foreach (Nation nation in nations)
        {   
            options[index] = new BasicSelection.Option<Nation>(nation.nationName, nation);
            index++;
        }

        _nations = new BasicSelection.Category<Nation>("Nations", options);
    }

    public static void RequestShipClass(Action<ShipClassType> onFulfilled)
    {
        BasicSelection.RequestSelection(_shipTypeCategories, onFulfilled);
    }

    public static void RequestNationType(Action<Nation> onFulfilled)
    {
        BasicSelection.RequestSelection(_nations, onFulfilled);
    }
}
