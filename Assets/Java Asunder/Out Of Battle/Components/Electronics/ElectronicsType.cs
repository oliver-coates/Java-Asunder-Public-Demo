using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ships
{

[CreateAssetMenu(fileName = "New Electronics Type", menuName = "Java Asunder/Components/Electronics", order = 0)]
public class ElectronicsType : ShipComponent
{
    public enum Electronics
    {
        BallisticComputer,
        Sonar,
        Radar
    }

    [Header("Electronics Type:")]
    public Electronics type;
    
    [Header("Strength:")]
    [Range(1, 100)]
    public int speed = 1;
    [Range(1, 100)]
    public int range = 1;
    [Range(1, 100)]
    public int reliability = 1;

}
}