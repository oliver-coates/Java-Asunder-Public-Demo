using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is pretty bad to use an enum here - especially in global space!
// Consider reworking this
public enum CameraFocusType
{
    FreeCam = 1,        // Follows the developer camera.
    ShipOverview = 2,   // Focusing entirely on the ship, up close. Could show component status/damage control here
    Spotting = 3,       // General overview of the surrounding area of the ship
    Gunnery = 4,        // Shows both the player ship and the ship they are targetting
    Target = 5          // Focus on the targetted ship
}

public class CameraFocusManager : MonoBehaviour
{
    public static event Action<CameraFocusType> OnChanged;

    private void Awake()
    {
        GameMaster.OnBattleStart += GoToOverviewFocus;
    }   

    private void OnDestroy()
    {
        GameMaster.OnBattleStart -= GoToOverviewFocus;
    }



    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            CheckForInput();
        }
    }

    private void GoToOverviewFocus()
    {
        OnChanged.Invoke(CameraFocusType.ShipOverview);
    }


    private void CheckForInput()
    {
        CameraFocusType newFocusType; 

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {   
            newFocusType = CameraFocusType.FreeCam;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            newFocusType = CameraFocusType.ShipOverview;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            newFocusType = CameraFocusType.Spotting;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            newFocusType = CameraFocusType.Gunnery;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            newFocusType = CameraFocusType.Target;
        }
        else
        {
            return;
        }

        OnChanged?.Invoke(newFocusType);
    }
}
