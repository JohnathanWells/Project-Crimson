using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveLoad
{

    public static savefileClass savedGame;

    public static void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/progress.sp");
        bf.Serialize(file, SaveLoad.savedGame);
        file.Close();
    }

    public static void Load()
    {
        if (!File.Exists(Application.persistentDataPath + "/progress.sp"))
        {
            savedGame = new savefileClass();
            Save();
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/progress.sp", FileMode.Open);
        SaveLoad.savedGame = (savefileClass)bf.Deserialize(file);
        file.Close();
    }

    public static void Delete()
    {
        savedGame = new savefileClass();
        Save();
    }
}