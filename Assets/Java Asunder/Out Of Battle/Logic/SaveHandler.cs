using System.Collections;
using System.Collections.Generic;
using KahuInteractive.HassleFreeSaveLoad;
using UnityEngine;

public class SaveHandler : MonoBehaviour
{
    public void SaveGame()
    {
        SaveLoad.Save();
    }
}
