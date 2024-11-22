using System.Collections;
using System.Collections.Generic;
using Ships;
using UnityEngine;

public class TorpedoLauncher : BoardPiece, IShipComponentInstance
{
    private DeckMountType _type;
    [SerializeField] private AmmunitionType _torpedoToShoot;
    [SerializeField] private float _loadTimer;

    public ComponentEffectiveness[] GetComponentEffectivenesses()
    {
        return new ComponentEffectiveness[0];
    }

    private void Awake()
    {
        BoardInteractionManager.DoTorpedoLaunch += LaunchTorpedoes;
    }

    private void OnDestroy()
    {
        BoardInteractionManager.DoTorpedoLaunch -= LaunchTorpedoes;
    }

    public void Setup(ShipInstance ship, ComponentSlot componentSlot)
    {
        componentSlot.componentInstance = this;

        _type = (DeckMountType) componentSlot.component; 

        _torpedoToShoot = _type.torpedo;
    }

    protected override void GameTick()
    {
        _loadTimer -= Time.deltaTime;
    }

    protected override void Initialise()
    {

    }

    protected override void UpdateTick()
    {

    }
    
    private void LaunchTorpedoes(float angle)
    {
        GameObject torpedoObject = Instantiate(_torpedoToShoot._prefab);

        torpedoObject.transform.eulerAngles = new Vector3(0, 0, angle);
    }

}
