using System;
using System.Collections;
using System.Collections.Generic;
using KahuInteractive.HassleFreeSaveLoad;
using Ships;
using UnityEngine;

public static class SessionMaster
{
    public static event Action OnLoaded;
    public static event Action OnPlayerCountChanged;
    public static event Action OnShipCountChanged;

    public static List<PlayerCharacter> PlayerCharacters;
    public static List<Ship> Ships;
    public static List<Ship> ActiveShips
    {
        get
        {
            List<Ship> _activeShips = new List<Ship>();

            foreach(Ship ship in Ships)
            {
                if (ship.isIncludedInBattle)
                {
                    _activeShips.Add(ship);
                }
            }
            
            return _activeShips;
        }
    }
    public static Ship PlayerShip;


    public static void Initialise()
    {
        PlayerCharacters = new List<PlayerCharacter>();
        Ships = new List<Ship>();
    
        ObjectRequestHandler.Initialise();
        LoadDataFromSaveFile();
    
        OnLoaded?.Invoke();
    }

    private static void LoadDataFromSaveFile()
    {
        List<SerializedObject> serializedObjects = SaveLoad.SerializedObjects;

        foreach (SerializedObject loadedObject in serializedObjects)
        {
            if (loadedObject is PlayerCharacter)
            {
                AddPlayerCharacter((PlayerCharacter) loadedObject);
            }
            else if (loadedObject is Ship)
            {
                AddShip((Ship) loadedObject);
            }
        } 
    }

    #region Player Characters
    public static void AddPlayerCharacter(PlayerCharacter playerCharacter)
    {
        PlayerCharacters.Add(playerCharacter);

        OnPlayerCountChanged?.Invoke();
    }

    public static void RemovePlayerCharacter(PlayerCharacter playerCharacter)
    {
        PlayerCharacters.Remove(playerCharacter);
        SaveLoad.UntrackSerializedObject(playerCharacter);

        OnPlayerCountChanged?.Invoke();
    }
    
    public static void MoveAllPlayerCharactersToBridge(ShipInstance playerShip)
    {
        foreach (PlayerCharacter playerCharacter in PlayerCharacters)
        {
            playerCharacter.TeleportTo(playerShip.bridgeComponentSlot);
        }
    }
    #endregion

    #region Ships

    public static void AddShip(Ship toAdd)
    {
        Ships.Add(toAdd);

        if (toAdd.isIncludedInBattle)
        {
            ActiveShips.Add(toAdd);
        }

        if (toAdd.isPlayerShip)
        {
            PlayerShip = toAdd;
        }

        OnShipCountChanged?.Invoke();
    }

    public static void RemoveShip(Ship toRemove)
    {
        Ships.Remove(toRemove);
        SaveLoad.UntrackSerializedObject(toRemove);
        
        GameObject.Destroy(toRemove);

        OnShipCountChanged?.Invoke();
    }

    #endregion


}
