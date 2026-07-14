using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public static class SaveManager
{
    //The class handling save files in the game. For now they're just small xml files, this may change as the game grows.
    #region Methods
    public static List<UserProfile> LoadAllProfiles()
    {
        var ret = new List<UserProfile>();
        if(Directory.Exists(Path.Combine(Application.persistentDataPath, "Profiles")))
        {
            var fileList = Directory.GetFiles(Path.Combine(Application.persistentDataPath, "Profiles"));
            var serializer = new XmlSerializer(typeof(UserProfile));
            foreach (var file in fileList)
            {
                using (var reader = new StreamReader(file))
                {
                    var profile = (UserProfile)serializer.Deserialize(reader);
                    profile.FileName = Path.GetFileNameWithoutExtension(file);
                    ret.Add(profile);
                    reader.Close();
                }
            }
        }
        return ret;
    }
    public static void SaveProfile(UserProfile profile)
    {
        var serializer = new XmlSerializer(typeof(UserProfile));
        if (!Directory.Exists(Path.Combine(Application.persistentDataPath, "Profiles")))
            Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "Profiles"));
        if (profile.FileName == null || profile.FileName.Length==0)
        {
            profile.FileName = Random.Range((int)0, int.MaxValue).ToString();
            while (CheckUnique(profile.FileName)==false)
            {
                profile.FileName = Random.Range((int)0, int.MaxValue).ToString();
            }
            File.Create(Path.Combine(Application.persistentDataPath, "Profiles", profile.FileName + ".xml")).Close();
        }
        using (var writer = new StreamWriter(Path.Combine(Application.persistentDataPath, "Profiles", profile.FileName + ".xml")))
        {
            serializer.Serialize(writer, profile);
            writer.Close();
        }
    }
    public static bool LoadSettings()
    {
        if (File.Exists(Path.Combine(Application.persistentDataPath, "Settings.xml")))
        {
            using (var reader = new StreamReader(Path.Combine(Application.persistentDataPath, "Settings.xml")))
            {
                var serializer = new XmlSerializer(typeof(SettingsSet));
                SkyforgeLoader.SettingsSet = (SettingsSet)serializer.Deserialize(reader);
                reader.Close();
            }
            return true;
        }
        return false;
    }
    public static void SaveSettings()
    {
        if (!Directory.Exists(Path.Combine(Application.persistentDataPath)))
            Directory.CreateDirectory(Path.Combine(Application.persistentDataPath));
        if(File.Exists(Path.Combine(Application.persistentDataPath, "Settings.xml")))
            File.Create(Path.Combine(Application.persistentDataPath, "Settings.xml")).Close();
        using (var writer = new StreamWriter(Path.Combine(Application.persistentDataPath, "Settings.xml")))
        {
            var serializer = new XmlSerializer(typeof(SettingsSet));
            serializer.Serialize(writer, SkyforgeLoader.SettingsSet);
            writer.Close();
        }
    }
    private static bool CheckUnique(string fileName)
    {
        var fileList = Directory.GetFiles(Path.Combine(Application.persistentDataPath, "Profiles"));
        foreach (var file in fileList)
        {
            if (Path.GetFileNameWithoutExtension(file) == fileName)
                return false;
        }
        return true;
    }
    #endregion
}
