using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KahuInteractive.HassleFreeSaveLoad
{

// Copy and paste this CreateAssetMenu into your subclasses for quick creation in the editor:
[CreateAssetMenu(fileName = "New Serialized Object", menuName = "KI_HassleFreeSaveLoad/TestSerializedObject", order = 1)]
public class ExampleSerializedObject : SerializedObject
{
    [SerializeField] public string bannanaString = "Bannan!";

    [SerializeField] private int _banannaNumber = 0;

    protected override void Initialise()
    {
        _banannaNumber = Random.Range(0, 500);
    }

}

}
