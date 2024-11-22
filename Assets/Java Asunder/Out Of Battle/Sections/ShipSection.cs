using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ships
{

public class ShipSection : BoardPiece
{
    public string sectionName = "Unnamed section";

    [HideInInspector] public ShipInstance ship;

    [Header("Damage:")]
    public SectionDamageState state;

    [Header("Slots: (Auto-populated at runtime)")]
    public ComponentSlot[] slots;

    [Header("Decorative")]
    [SerializeField] private ParticleSystem _smokeEmitter;
    public ParticleSystem smokeEmitter
    {
        get
        {
            return _smokeEmitter;
        }	
    }

    protected override void Initialise() { }

    public void Setup(ShipInstance ship)
    {
        this.ship = ship;
        state = new SectionDamageState(ship, this);


        slots = GetComponentsInChildren<ComponentSlot>();
        foreach (ComponentSlot slot in slots)
        {
            slot.Initialise(ship, this);
        }
    }

    protected override void UpdateTick() { }

    protected override void GameTick()
    {
        state.Tick(Time.deltaTime);
    }

    public void Hit(AmmunitionType ammunitionType)
    {
        state.RecieveHit(ammunitionType);
    }


}

}