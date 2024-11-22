using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace KahuInteractive.HassleFreeSaveLoad
{


/// <summary>
/// This class is a simple helper class which helps you see what is happening inside the 
/// Hasslefree save load system.
///  
/// Just add this class onto gameobject and inspect it in the editor.
/// Additionally, enable _debugLoggingEnabled to get debug logs
/// </summary>
public class SaveLoadDebugger : MonoBehaviour
{
    [Header("Buttons:")]
    [SerializeField] private string _debugSaveName;
    [SerializeField] private bool _debugForceSave;
    [SerializeField] private bool _debugForceLoad;

    [SerializeField] private bool _debugCreateTestObject;
    [SerializeField] private bool _debugDeleteRandomTestObject;


    [Header("Status:")]
    [SerializeField] private bool _debugLoggingEnabled;
    [SerializeField] private string _persistentDataPath;
    [SerializeField] private List<SerializedObject> _trackedScriptableObjects;

    private void Awake()
    {
        _persistentDataPath = Application.persistentDataPath;

        SaveLoad.OnLoadFinished += LoadFinished;
        SaveLoad.OnSaveFinished += SaveFinished;
        SaveLoad.OnScriptableObjectListChanged += TrackedObjectsChanged;
    }
    
    private void OnDestroy()
    {
        SaveLoad.OnLoadFinished -= LoadFinished;
        SaveLoad.OnSaveFinished -= SaveFinished;
        SaveLoad.OnScriptableObjectListChanged -= TrackedObjectsChanged;
    }

    private void SaveFinished(string saveName)
    {
        if (_debugLoggingEnabled)
        {
            Debug.Log($"SimpleSaveLoad: Sucessfully saved under name {saveName}");
        }
    }

    private void LoadFinished(string saveName)
    {
        if (_debugLoggingEnabled)
        {
            Debug.Log($"SimpleSaveLoad: Sucessfully loaded from name {saveName}");
        }
    }

    private void TrackedObjectsChanged()
    {
        _trackedScriptableObjects = SaveLoad.SerializedObjects;
    }

    private void Update()
    {
        if (_debugCreateTestObject)
        {
            _debugCreateTestObject = false;
            ExampleSerializedObject test = SaveLoad.InstantiateSerializedObject<ExampleSerializedObject>("Debug Object");
        }

        if (_debugForceLoad)
        {
            SaveLoad.Load(_debugSaveName);
            _debugForceLoad = false;
        }

        if (_debugForceSave)
        {
            SaveLoad.Save(_debugSaveName);
            _debugForceSave = false;
        }

        if (_debugDeleteRandomTestObject)
        {
            int randomNum = Random.Range(0, _trackedScriptableObjects.Count);
            SerializedObject toDelete = _trackedScriptableObjects[randomNum];

            SaveLoad.UntrackSerializedObject(toDelete);

            if (_debugLoggingEnabled)
            {
                Debug.Log($"Deleting object with id {toDelete.persistentID}");
            }

            _debugDeleteRandomTestObject = false;
        }

    }

}

}