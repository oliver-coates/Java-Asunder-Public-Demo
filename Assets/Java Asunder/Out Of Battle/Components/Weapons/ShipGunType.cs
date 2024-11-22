using KahuInteractive.HassleFreeAudio;
using UnityEngine;

namespace Ships
{


[CreateAssetMenu(fileName = "New Gun Type", menuName = "Java Asunder/Components/Gun", order = 0)]
public class ShipGunType : ShipComponent
{   
    [Header("The prefab to be spawned on the ship:")]
    public GameObject prefab;

    [Header("Stats:")]
    [Range(0, 100)]
    public int turnSpeed;

    [Range(0, 100)]
    public int reloadTime;

    [Range(0,100)]
    public int accuracy;    

    [Header("The maximum range (in meters)")]
    public int maxRange;

    [Header("The type of ammunuition fired by this gun")]
    public AmmunitionType ammo;

    [Header("The clip set to play when firing")]
    public ClipSet shootClipSet;
}

}