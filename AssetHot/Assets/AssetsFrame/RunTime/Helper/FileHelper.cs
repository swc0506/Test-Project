using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ZM.AssetFrameWork
{
    public class FileHelper
    {
        public static void DeleteFile(string folderPath)
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
    }
}