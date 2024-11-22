using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KahuInteractive.HassleFreeSaveLoad
{

// Copy and paste this CreateAssetMenu into your subclasses for quick creation in the editor:
[CreateAssetMenu(fileName = "New Serialized Object", menuName = "KI_HassleFreeSaveLoad/TestSerializedObject", order = 1)]
[System.Serializable]
public class SerializedObject : ScriptableObject
{
    [SerializeField] private int _persistentID = 0;
    public int persistentID
    {
        get
        {
            return _persistentID;
        }
    }

    /// <summary>
    /// Sets the persistent ID of this serialized object.
    /// Note that this should NOT be called by any script others than SaveLoad.cs upon object creation.
    /// </summary>
    public void SetPersistentID(int id)
    {
        _persistentID = id;
    }

    public void OnCreate()
    {
        Initialise();
    }

    /// <summary>
    /// Called when the object is first created.
    /// It will not be called upon being loaded into a running Unity application, use Awake() for that.
    /// Override this for data you need to execute only once, (like picking a random number on startup)
    /// No need to call base().
    /// </summary>
    protected virtual void Initialise()
    {

    }
}

}
