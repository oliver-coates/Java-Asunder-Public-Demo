using System.Collections;
using System.Collections.Generic;
using Effects;
using UnityEngine;

namespace Ships
{

[CreateAssetMenu(fileName = "New Ammunition Type", menuName = "Java Asunder/Components/Ammunition", order = 0)]
public class AmmunitionType : ShipComponent 
{
    [Header("Prefab:")]
    public GameObject _prefab;

    [Header("Damage:")]
    [Range(0,100)]
    public int damage;
    [Range(0,100)]
    public int pierce;
    [Range(0, 100)]
    public int velocity;

    [Range(0, 100)]
    public int reliability;

    [Header("Decoration:")]
    public EffectType splashEffect;
    public EffectType explosionEffect;


}



}