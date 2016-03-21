using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.Text;

public static class SaveGameSystem
{
    public static bool SaveGame(SaveGame saveGame, string name)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        using (FileStream stream = new FileStream(GetSavePath(name), FileMode.Create))
        {
            try
            {
                Debug.Log(GetSavePath(name));
                formatter.Serialize(stream, saveGame);
            }
            catch (Exception)
            {
                return false;
            }
        }

        return true;
    }

    public static MySaveGame LoadGame(string name)
    {
        if (!DoesSaveGameExist(name))
        {
            Debug.Log("NOT EXIST");
            return null;
        }

        BinaryFormatter formatter = new BinaryFormatter();
        using (FileStream stream = new FileStream(GetSavePath(name), FileMode.Open))
        {
            try
            {
                MySaveGame s = new MySaveGame();
                s.Objects = (List<GameObject>)formatter.Deserialize(stream);
                Debug.Log(s.Objects.Count);
                return s as MySaveGame;
            }
            catch (Exception e)
            {
                Debug.Log(e);
                return null;
            }
        }
    }

    public static bool DeleteSaveGame(string name)
    {
        try
        {
            File.Delete(GetSavePath(name));
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }

    public static bool DoesSaveGameExist(string name)
    {
        return File.Exists(GetSavePath(name));
    }

    private static string GetSavePath(string name)
    {
        return Path.Combine(Application.persistentDataPath, name + ".sav");
    }
}

