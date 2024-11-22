using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ships
{


[CreateAssetMenu(fileName = "New Damage Control Type", menuName = "Java Asunder/Components/Damage Control", order = 1)]
public class DamageControlType : ShipComponent
{
    [Header("Effectiveness:")]
    [Range(0,100)]
    public int fireFighting = 1;
    [Range(0,100)]
    public int counterFlooding = 1;
}

}