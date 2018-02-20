using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveLoad
{

    public static savefileClass savedGame;
    public static int[] playlist;

    public static void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();


        FileStream file = File.Create(Application.persistentDataPath + "/progress.sp");
        bf.Serialize(file, savedGame);

        file.Close();

        bf = new BinaryFormatter();

        FileStream playlistFile = File.Create(Application.persistentDataPath + "/musicPlaylist.sp");
        bf.Serialize(playlistFile, playlist);

        playlistFile.Close();
    }

    public static void Load()
    {
        if (!File.Exists(Application.persistentDataPath + "/progress.sp"))
        {
            savedGame = new savefileClass();


            if (!File.Exists(Application.persistentDataPath + "/musicPlaylist.sp"))
            {
                Debug.Log("Gotta create");

                playlist = new int[Constants.lengthOfPlaylist];
                for (int n = 0; n < playlist.Length; n++)
                    playlist[n] = n;

                Debug.Log(playlist[0]);
            }

            Save();
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/progress.sp", FileMode.Open);
        savedGame = (savefileClass)bf.Deserialize(file);

        file.Close();


        bf = new BinaryFormatter();
        FileStream musicPlaylist = File.Open(Application.persistentDataPath + "/musicPlaylist.sp", FileMode.Open);
        playlist = (int[])bf.Deserialize(musicPlaylist);

        musicPlaylist.Close();
    }

    public static void Delete()
    {
        savedGame = new savefileClass();

        playlist = new int[Constants.lengthOfPlaylist];
        for (int n = 0; n < playlist.Length; n++)
            playlist[n] = n;

        Save();
    }
}