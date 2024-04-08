using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{

    public static void SaveMap (WalkerGenerator map) {
        BinaryFormatter formatter = new BinaryFormatter();

        string path = Application.persistentDataPath + "/player.franny";
        FileStream stream = new FileStream(path, FileMode.Create);

        MapData data = new MapData(map);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static MapData LoadMap () {

        string path = Application.persistentDataPath + "/player.franny";

        if (File.Exists(path)) {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            MapData data = formatter.Deserialize(stream) as MapData;
            stream.Close();
            return data;

        } else {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }
}
