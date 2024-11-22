using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Ships
{

[CreateAssetMenu(fileName = "New Armour Type", menuName = "Java Asunder/Components/Armour", order = 0)]
public class ArmourType : ShipComponent
{
    [Header("Armour Strength:")]
    public int strength = 1;

}
}