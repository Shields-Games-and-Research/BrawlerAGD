using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;
using static Constants;
using static EvolutionManager;

/// <summary>
/// Class used for writing, reading, and deleting generation and game data to disk.
/// </summary>
public class DataLogger : MonoBehaviour
{
    public static DataLogger instance = null;

    void Awake() 
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this) 
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Reads a JSON from file and returns the corresponding serialized object
    /// </summary>
    public static T ReadJson<T>(string filename)
    {
        //print("filename reading: " + filename);
        // Write to file
        if (!File.Exists(filename))
        {
            throw new FileNotFoundException(Constants.FILE_NOT_FOUND);
        }
        // If the file exists, read from it
        else
        {
            string rawSerializedObj = File.ReadAllText(filename);
            T serializedObj = JsonUtility.FromJson<T>(rawSerializedObj);
            //print(serializedObj);
            return serializedObj;
        }
    }

    /// <summary>
    /// Writes a JSON file from a serialized object to disk at the filename
    /// </summary>
    public void WriteJson<T>(string filename, T serializedObj)
    {
        string serializedJSON = JsonUtility.ToJson(serializedObj);
        File.WriteAllText(filename, serializedJSON);
    }

}
