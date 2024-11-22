using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Ships
{
[CreateAssetMenu(fileName = "New Deck Mounting", menuName = "Java Asunder/Components/Deck Mount", order = 0)]
public class DeckMountType : ShipComponent
{
    [Header("Stats:")]
    public int torpedoesToFire = 1;

    public float loadTime = 10f;

    public AmmunitionType torpedo;

}

}