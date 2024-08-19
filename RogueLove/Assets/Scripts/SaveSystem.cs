using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static void SaveMap (WalkerGenerator map) {
        BinaryFormatter formatter = new();

        string pathMap = Application.persistentDataPath + "/map.chris";
        FileStream stream = new(pathMap, FileMode.Create);

        MapData dataMap = new(map);

        formatter.Serialize(stream, dataMap);
        stream.Close();
        Debug.Log("SAVED MAP");
    }

    public static MapData LoadMap () {

        string pathMap = Application.persistentDataPath + "/map.chris";

        if (File.Exists(pathMap)) {
            BinaryFormatter formatter = new();
            FileStream stream = new(pathMap, FileMode.Open);

            MapData dataMap = formatter.Deserialize(stream) as MapData;
            stream.Close();
            Debug.Log("LOADED MAP");
            return dataMap;

        } else {
            Debug.LogError("Map save file not found in " + pathMap);
            return null;
        }
    }

    public static void SavePlayer (PlayerController player, Weapon weapon) {
        BinaryFormatter formatter = new();

        string pathPlayer = Application.persistentDataPath + "/player.franny";
        FileStream stream = new(pathPlayer, FileMode.Create);

        PlayerData dataPlayer = new(player, weapon);

        formatter.Serialize(stream, dataPlayer);
        stream.Close();
        Debug.Log("SAVED PLAYER");
    }

    public static PlayerData LoadPlayer() {

        string pathPlayer = Application.persistentDataPath + "/player.franny";

        if (File.Exists(pathPlayer)) {
            BinaryFormatter formatter = new();
            FileStream stream = new(pathPlayer, FileMode.Open);

            PlayerData dataPlayer = formatter.Deserialize(stream) as PlayerData;
            stream.Close();
            Debug.Log("LOADED PLAYER");
            return dataPlayer;

        } else {
            Debug.LogError("Player save file not found in " + pathPlayer);
            return null;
        }
    }

    public static void SaveHome (HomeManager manager) {
        BinaryFormatter formatter = new();

        string pathHome = Application.persistentDataPath + "/home.soni";
        FileStream stream = new(pathHome, FileMode.Create);

        HomeData dataHome = new(manager);

        formatter.Serialize(stream, dataHome);
        stream.Close();
        Debug.Log("SAVED HOME");
    }

    public static HomeData LoadHome() {
        string pathHome = Application.persistentDataPath + "/home.soni";

        if (File.Exists(pathHome)) {
            BinaryFormatter formatter = new();
            FileStream stream = new(pathHome, FileMode.Open);

            HomeData dataHome = formatter.Deserialize(stream) as HomeData;
            stream.Close();
            Debug.Log("LOADED HOME");
            return dataHome;
        } else {
            Debug.LogError("Home save file not found in " + pathHome);
            return null;
        }
    }
}
