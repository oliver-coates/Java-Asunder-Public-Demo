
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace KahuInteractive.HassleFreeSaveLoad
{

public static class SaveLoad
{
    private const int MAXIMUM_IDS = 250000;

    public static event Action<string> OnSaveFinished;
    public static event Action<string> OnLoadFinished;
    public static event Action OnScriptableObjectListChanged;

    /// <summary>
    /// All of the Serialized Objects that the Save/Load manager is currently tracking.
    /// </summary>
    private static List<SerializedObject> _SerializedObjects;
    
    /// <summary>
    /// All of the Serialized Objets that the Save/Load manager is currently tracking (read only).
    /// Add objects using the InstaniateSerializedObject method.
    /// </summary>
    /// <value></value>
    public static List<SerializedObject> SerializedObjects
    {
        get
        {
            return _SerializedObjects;
        }
    }

    /// <summary>
    /// List of Scriptable objects to be deleted on next save.
    /// </summary>
    private static List<int> _ToDelete;

    // The most recently loaded from save name, this makes it easy for
    // developers to call Load("Save321") and then later call Save() to save 
    // it back under that "Save321" save name
    private static string _MostRecentSaveName;

    private static Dictionary<int, bool> _PersistentIDDictionary;


    #region Save
    /// <summary>
    /// Saves all SerializedObjects to the persistent data path under the save name which was most recently loaded from.
    /// </summary>
    public static void Save()
    {
        Save(_MostRecentSaveName);
    }

    /// <summary>
    /// Saves all SerialiedObjects to the persistent data path UNDER the provided save name.
    /// </summary>
    /// <param name="saveName"></param>
    public static void Save(string saveName)
    {
        #region Save path directory
        string savePath = Path.Combine(Application.persistentDataPath, saveName);
        // Check that a directory exists with this saveName
        if (Directory.Exists(savePath) == false)
        {
            // If there isn't a directory, create one for us to store stuff in
            Directory.CreateDirectory(savePath);
        } 
        #endregion

        #region Deleting marked files
        // Deletion of files marked for deletion:

        // Get the paths to all files in this directory
        string[] filePaths = Directory.GetFiles(savePath, "*", SearchOption.AllDirectories);
        foreach (string filePath in filePaths)
        {
            // Get name of this object without the path or extension
            int objectID = int.Parse(Path.GetFileNameWithoutExtension(filePath));

            // If it needs to be deleted, delete it!
            if (_ToDelete.Contains(objectID))
            {
                File.Delete(filePath);
                // Free up this ID in the dictionary 
                _PersistentIDDictionary[objectID] = false;
            }
        }
        #endregion

        // Addition or Overwiriting of new data:
        foreach (SerializedObject objectToSave in _SerializedObjects)
        {
            // Get the file data in JSON serialized form
            string jsonOut = JsonUtility.ToJson(objectToSave);

            // The file name that that this object should be saved under:
            string objectFileName = objectToSave.persistentID.ToString();
            // Get the type of this object as a string
            string objectTypeName = objectToSave.GetType().ToString();

            // Get the directory that this object should be saved under
            string objectSavePath = Path.Combine(savePath, objectTypeName);

            if (Directory.Exists(objectSavePath) == false)
            {
                // Create a directory here if it does not exist
                Directory.CreateDirectory(objectSavePath);
            }

            // The path that it should exist at (if already saved)
            string objectPath = Path.Combine(objectSavePath, objectFileName);

            // Then write data
            File.WriteAllText(objectPath, jsonOut);
        }

        OnScriptableObjectListChanged?.Invoke(); // Deletion may have caused a change
        OnSaveFinished?.Invoke(saveName);
    }
    #endregion

    #region Load
    public static bool Load(string saveName)
    {
        _PersistentIDDictionary = new Dictionary<int, bool>();
        _MostRecentSaveName = saveName;
        _SerializedObjects = new List<SerializedObject>();
        _ToDelete = new List<int>();

        string savePath = Path.Combine(Application.persistentDataPath, saveName);
        // Check that a directory exists with this saveName
        if (Directory.Exists(savePath))
        {
            // If a directory exists with this save name,
            // Get all subfolders, they signify which type of object we have
            string[] subdirectories = Directory.GetDirectories(savePath);

            // Go through each subdirectory, pulling out data and adding it to the serializedObject list
            foreach (string subdirectoryPath in subdirectories)
            {
                string subdirectoryName = Path.GetFileName(subdirectoryPath);
                Type typeName = Type.GetType(subdirectoryName);
            
                if (typeName == null)
                {
                    Debug.LogError($"SaveLoad: Type could not be parsed from {subdirectoryName}");
                    continue;
                }

                string[] filePaths = Directory.GetFiles(subdirectoryPath);

                foreach (string filePath in filePaths)
                {
                    string jsonIn = File.ReadAllText(filePath);

                    SerializedObject toOverwrite = (SerializedObject) ScriptableObject.CreateInstance(typeName);
                    JsonUtility.FromJsonOverwrite(jsonIn, toOverwrite);

                    TrackSerializedObject(toOverwrite);
                }
            } 

            OnScriptableObjectListChanged?.Invoke();
            OnLoadFinished?.Invoke(saveName);
            return true;
        } 
        else
        {
            // No directory has been found.
            // This is expected
            OnLoadFinished?.Invoke(saveName);
            return false;
        }
    }
    #endregion

    #region Instantiate

    /// <summary>
    /// Creates and returns a new Serialized Game Object which is now being
    /// tracked by the save load manager and will be saved to file upon the next Save();
    /// </summary>
    /// <typeparam name="T"> Type of object to create.</typeparam>
    /// <returns> The created object</returns>
    public static T InstantiateSerializedObject<T>(string name) where T : SerializedObject
    {
        if (_SerializedObjects == null)
        {
            _SerializedObjects = new List<SerializedObject>();
        }

        T NewObj = (T) ScriptableObject.CreateInstance(typeof(T));
        NewObj.name = name;

        NewObj.SetPersistentID(GetPersistentUID());
        NewObj.OnCreate();

        _SerializedObjects.Add(NewObj);

        OnScriptableObjectListChanged?.Invoke();

        return NewObj;
    }

    /// <summary>
    /// Starts tracking this scriptable object, so that it will be saved next save
    /// </summary>
    /// <param name="toTrack"> The object to start tracking. </param>
    private static void TrackSerializedObject(SerializedObject toTrack)
    {
        if (_SerializedObjects.Contains(toTrack))
        {
            Debug.Log($"SaveLoad alreadys tracks object : {toTrack.ToString()}");
            return;
        }

        // Note that this persistent ID is now in use
        _PersistentIDDictionary[toTrack.persistentID] = true;

        _SerializedObjects.Add(toTrack);
        OnScriptableObjectListChanged?.Invoke();
    }


    private static int GetPersistentUID()
    {
        int possibleID = 0;

        while (possibleID < MAXIMUM_IDS)
        {
            if (_PersistentIDDictionary.ContainsKey(possibleID))
            {
                // If an ID is already in the dictionary, check if it is in use
                if (_PersistentIDDictionary[possibleID] == false)
                {
                    _PersistentIDDictionary[possibleID] = true;
                    return possibleID;
                }
            }
            else
            {
                // If no unused ID could be found, add a new one
                _PersistentIDDictionary.Add(possibleID, true);
                return possibleID;
            }

            possibleID ++;
        }

        Debug.LogError($"Ran out of Persistent IDs!");
        return -1;
    }

    #endregion

    #region Destroy
    
    /// <summary>
    /// Stops tracking a serialized object.
    /// This serialzied object will be deleted from memory NEXT SAVE.
    /// </summary>
    /// <param name="toRemove"> The object to remove.</param>
    public static void UntrackSerializedObject(SerializedObject toRemove) 
    {
        SerializedObjects.Remove(toRemove);
        _ToDelete.Add(toRemove.persistentID);

        OnScriptableObjectListChanged?.Invoke();
    }

    #endregion
}

}