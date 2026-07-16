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
                try
                {
                    using (var reader = new StreamReader(file))
                    {

                        var profile = (UserProfile)serializer.Deserialize(reader);
                        profile.FileName = Path.GetFileNameWithoutExtension(file);
                        ret.Add(profile);
                        reader.Close();
                    }
                }
                catch (System.Exception exception)
                {
                    Debug.LogWarning($"Unable to load profile '{file}'. The file was ignored. {exception.Message}");
                    BackupCorruptFile(file);
                }
            }
        }
        return ret;
    }
    public static void SaveProfile(UserProfile profile)
    {
        if (profile == null)
        {
            Debug.LogError("WARNING! Null profile save attempt!");
            return;
        }
        var serializer = new XmlSerializer(typeof(UserProfile));
        string target = Path.Combine(Application.persistentDataPath, "Profiles");
        if (!Directory.Exists(target))
            Directory.CreateDirectory(target);
        if (profile.FileName == null || profile.FileName.Length==0)
        {
            profile.FileName = Random.Range((int)0, int.MaxValue).ToString();
            while (CheckUnique(profile.FileName)==false)
            {
                profile.FileName = Random.Range((int)0, int.MaxValue).ToString();
            }
            target = Path.Combine(target, profile.FileName + ".xml");
            File.Create(target).Close();
        }
        else
            target = Path.Combine(target, profile.FileName + ".xml");
        SerializeWithTemp(target, profile, new XmlSerializer(typeof(UserProfile)));
    }
    public static bool LoadSettings()
    {
        var path = Path.Combine(Application.persistentDataPath, "Settings.xml");
        if (File.Exists(path))
        {
            try
            {
                using (var reader = new StreamReader(path))
                {
                    var serializer = new XmlSerializer(typeof(SettingsSet));
                    SkyforgeLoader.SettingsSet = (SettingsSet)serializer.Deserialize(reader);
                    reader.Close();
                }
                return true;
            }
            catch (System.Exception exception)
            {
                Debug.LogWarning($"Unable to load settings! {exception.Message}");
                BackupCorruptFile(path);
                return false;
            }
        }
        return false;
    }
    public static void SaveSettings()
    {
        if (!Directory.Exists(Path.Combine(Application.persistentDataPath)))
            Directory.CreateDirectory(Path.Combine(Application.persistentDataPath));
        string filePath = Path.Combine(Application.persistentDataPath, "Settings.xml");
        if (File.Exists(filePath))
            File.Create(filePath).Close();
        SerializeWithTemp(filePath, SkyforgeLoader.SettingsSet, new XmlSerializer(typeof(SettingsSet)));
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
    private static void SerializeWithTemp<T>(string path, T value, XmlSerializer serializer)
    {
        string temporaryPath = path + ".tmp";
        try
        {
            using (StreamWriter writer = new StreamWriter(temporaryPath, false))
            {
                serializer.Serialize(writer, value);
                writer.Close();
            }
            File.Copy(temporaryPath, path, true);
        }
        finally
        {
            if (File.Exists(temporaryPath))
            {
                File.Delete(temporaryPath);
            }
        }
    }
    private static void BackupCorruptFile(string path)
    {
        try
        {
            if (!File.Exists(path))
            {
                return;
            }
            string backupPath = path + ".corrupt-" + System.DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".bak";
            File.Copy(path, backupPath, false);
        }
        catch (System.Exception exception)
        {
            Debug.LogWarning($"Unable to create a backup of corrupt file '{path}'. {exception.Message}");
        }
    }
    #endregion
}
