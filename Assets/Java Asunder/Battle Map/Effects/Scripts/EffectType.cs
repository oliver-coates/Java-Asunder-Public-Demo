using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Effects
{

[CreateAssetMenu(fileName = "New Effect Type", menuName = "Java Asunder/Effects/New Effect", order = 1)]
public class EffectType : ScriptableObject
{
    public GameObject prefab;

}

}