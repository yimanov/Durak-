using System.IO;
using UnityEngine;

namespace Meta
{
    public class SaveUtility
    {
        public void WriteFile<T>(T data, string directory, string fileName) where T : class
        {
            var jsonString = JsonUtility.ToJson(data);
            var path = GetFullPath(directory, fileName);
            
            var directoryInfo = new DirectoryInfo(GetDirectoryPath(directory));
            
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }

            File.WriteAllText(path, jsonString);
        }

        public void WriteFile(string data, string directory, string fileName)
        {
            var path = GetFullPath(directory, fileName);
            var directoryInfo = new DirectoryInfo(GetDirectoryPath(directory));
            
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }

            File.WriteAllText(path, data);
        }

        public T ReadFile<T>(string directory, string fileName, bool local = false) where T : class
        {
            var jsonString = string.Empty;
            
            if (local)
            {
                var asset = Resources.Load<TextAsset>(directory + "/" + fileName.Split('.')[0]);
                if (asset == null)
                    return null;
                jsonString = asset.text;
            }
            else
            {
                var path = GetFullPath(directory, fileName);
                
                if (!File.Exists(path))
                {
                    return null;
                }

                jsonString = File.ReadAllText(path);
            }

            return string.IsNullOrEmpty(jsonString) ? null : JsonUtility.FromJson<T>(jsonString);
        }

        public string ReadFile(string directory, string fileName, bool local = false)
        {
            if (local)
            {
                var asset = Resources.Load<TextAsset>(directory + "/" + fileName.Split('.')[0]);
                return asset == null ? null : asset.text;
            }

            var path = GetFullPath(directory, fileName);
            
            return !File.Exists(path) ? null : File.ReadAllText(path);
        }
        
        public void RemoveFile(string directory, string fileName)
        {
            var path = GetFullPath(directory, fileName);
            var directoryInfo = new DirectoryInfo(GetDirectoryPath(directory));
            
            if (!directoryInfo.Exists)
            {
                return;
            }

            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public void RemoveFolder(string directory)
        {
            var directoryInfo = new DirectoryInfo(GetDirectoryPath(directory));
            
            if (directoryInfo.Exists)
            {
                directoryInfo.Delete(true);
            }
        }
        
        public string GetFullPath(string directory, string fileName, string format = ".json")
        {
            var path = Path.Combine(Application.persistentDataPath, directory, fileName);
            return path + format;
        }

        public string GetDirectoryPath(string directory)
        {
            return Path.Combine(Application.persistentDataPath, directory);
        }
    }
}