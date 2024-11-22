using System.Collections;
using System.Collections.Generic;
using KahuInteractive.HassleFreeSaveLoad;
using UnityEngine;

public class SessionInitialiser : MonoBehaviour
{
    [SerializeField] private string _saveName = "unnamedSave";

    private void Start()
    {
        SaveLoad.Load(_saveName);

        SessionMaster.Initialise();
    }
}
