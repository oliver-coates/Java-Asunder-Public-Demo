using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ship Component Type", menuName = "Java Asunder/Ships/Component Type", order = 1)]
public class ShipComponentType : ScriptableObject
{
    public bool isLocationCharacterCanMoveTo = false;
    public GameObject genericPrefab;
}
