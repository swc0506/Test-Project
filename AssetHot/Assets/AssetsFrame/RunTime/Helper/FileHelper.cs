using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ZM.AssetFrameWork
{
    public class FileHelper
    {
        public static void DeleteFolder(string folderPath)
        {
            if (Directory.Exists(folderPath))
            {
                string[] pathsArr = Directory.GetFiles(folderPath, "*");
                foreach (var path in pathsArr)
                {
                    if (File.Exists(path))
                        File.Delete(path);
                }
                Directory.Delete(folderPath);
            }
        }

        public static void WriteFile(string filePath, byte[] data)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            FileStream stream = File.Create(filePath);
            stream.Write(data, 0, data.Length);
            stream.Dispose();
            stream.Close();
        }
    }
}