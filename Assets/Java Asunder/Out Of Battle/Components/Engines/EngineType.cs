using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ships
{

[CreateAssetMenu(fileName = "New Engine Type", menuName = "Java Asunder/Components/Engine", order = 0)]
public class EngineType : ShipComponent
{
    [Header("Stats:")]
    [Range(0, 100)]
    public int strength = 1;
    [Range(0, 100)]
    public int agility = 1;
    [Range(0, 100)]
    public int reliability = 1; 
}
}