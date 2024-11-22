using System;
using System.Collections;
using System.Collections.Generic;
using Ships;
using UnityEngine;

public class BoardInteractionManager : MonoBehaviour
{
    public static event Action<float> DoTorpedoLaunch;

    [SerializeField] private Camera _camera;

    private void Update()
    {
        if (!GameMaster.BattleUnderway)
        {
            return;
        }

        CheckForInput();
    }

    private void CheckForInput()
    {
        Vector2 mousePositionInWorld = _camera.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetMouseButton(0))
            {
                SetShipDestination(mousePositionInWorld);
            }
            else if (Input.GetMouseButtonDown(1))
            {
                AttemptTargetShip(mousePositionInWorld);
            }
        
            if (Input.GetKeyDown(KeyCode.T))
            {
                AttemptDoPlayerTorpedoes(mousePositionInWorld);
            }
        }
    }

    private void SetShipDestination(Vector3 destination)
    {
        if (GameMaster.SelectedShip == null)
        {
            return;
        }

        if (GameMaster.SelectedShip.isPlayerShip)
        {
            // Cannot set destination for player ship
            // It is manually controlled by the player
            return;
        }

        GameMaster.SelectedShip.instance.AI.SetDestination(destination);
    }

    private void AttemptTargetShip(Vector2 mousePositionInWorld)
    {
        Vector3 startPos = new Vector3(mousePositionInWorld.x, mousePositionInWorld.y, 10f);

        RaycastHit2D[] hits = Physics2D.RaycastAll(startPos, Vector2.zero);

        bool hitEnemyShip = false;
        foreach (RaycastHit2D hit in hits)
        {            
            ShipSection section = hit.collider.GetComponent<ShipSection>();
            if (section != null)
            {
                if (section.ship != GameMaster.SelectedShip)
                {
                    SetShipTarget(section.ship);
                    hitEnemyShip = true;
                    break;
                }
            }
        }

        if (!hitEnemyShip)
        {
            SetShipTarget(null);
        }
    }

    private void SetShipTarget(ShipInstance target)
    {
        if (target == GameMaster.SelectedShip)
        {
            // Cannot target itself
            return;
        }

        GameMaster.SelectedShip.instance.SetTarget(target);
    }

    private void AttemptDoPlayerTorpedoes(Vector2 mousePositionInWorld)
    {
        
        if (Input.GetMouseButtonDown(0))
        {
            
        }
    }
}
