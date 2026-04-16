using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Core
{
    public static class FileUtils
    {
        private static void TryCreateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        private static void TryCreateFile(string path, byte[] buff)
        {
            TryCreateDirectory(Path.GetDirectoryName(path));
            using (Stream fs = new FileStream(path, FileMode.Create))
            {
                fs.Write(buff, 0, buff.Length);
            }
        }

        private static void TryCopyFile(string srcPath, string desPath)
        {
            TryCreateDirectory(Path.GetDirectoryName(desPath));
            File.Copy(srcPath, desPath, true);
        }

        private static void TryMoveFile(string srcPath, string desPath)
        {
            TryCreateDirectory(Path.GetDirectoryName(desPath));
            if (File.Exists(desPath))
            {
                File.Delete(desPath);
            }

            File.Move(srcPath, desPath);
        }

        public static bool ExistsDirectory(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                try
                {
                    return Directory.Exists(path);
                }
                catch (Exception e)
                {
                    Logger.ErrorFormat("ExistsDirectory Exception:{0}", e.Message);
                }
            }

            return false;
        }

        public static bool DeleteDirectory(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                try
                {
                    if (Directory.Exists(path))
                    {
                        Directory.Delete(path, true);
                        return true;
                    }
                }
                catch (Exception e)
                {
                    Logger.ErrorFormat("DeleteDirectory Exception:{0}", e.Message);
                }
            }

            return false;
        }

        public static bool CreateDirectory(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                try
                {
                    TryCreateDirectory(path);
                    return true;
                }
                catch (Exception e)
                {
                    Logger.ErrorFormat("CreateDirectory Exception:{0}", e.Message);
                }
            }

            return false;
        }

        public static bool ExistsFile(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                try
                {
                    return File.Exists(path);
                }
                catch (Exception e)
                {
                    Logger.ErrorFormat("ExistsFile Exception:{0}", e.Message);
                }
            }

            return false;
        }

        public static bool DeleteFile(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                try
                {
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                        return true;
                    }
                }
                catch (Exception e)
                {
                    Logger.ErrorFormat("DeleteFile Exception:{0}", e.Message);
                }
            }

            return false;
        }

        public static bool CreateFile(string path, byte[] buff)
        {
            if (!string.IsNullOrEmpty(path) && null != buff)
            {
                try
                {
                    TryCreateFile(path, buff);
                    return true;
                }
                catch (Exception e)
                {
                    Logger.ErrorFormat("CreateFile Exception:{0}", e.Message);
                }
            }

            return false;
        }

        public static bool CreateFile(string path, string text)
        {
            if (!string.IsNullOrEmpty(path) && null != text)
            {
                try
                {
                    TryCreateDirectory(Path.GetDirectoryName(path));
                    using (Stream fs = new FileStream(path, FileMode.Create))
                    {
                        using (StreamWriter sw = new StreamWriter(fs))
                        {
                            sw.Write(text);
                        }
                    }

                    return true;
                }
                catch (Exception e)
                {
                    Logger.ErrorFormat("CreateFile Exception:{0}", e.Message);
                }
            }

            return false;
        }

        public static byte[] ReadFile(string path)
        {
            if (ExistsFile(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    byte[] buffer = new byte[fs.Length];
                    int bytesRead = 1;
                    while (bytesRead > 0)
                    {
                        bytesRead = fs.Read(buffer, 0, buffer.Length);
                    }

                    return buffer;
                }
            }

            return null;
        }

        public static bool CopyFile(string srcPath, string desPath)
        {
            if (!string.IsNullOrEmpty(srcPath) && !string.IsNullOrEmpty(desPath))
            {
                try
                {
                    if (File.Exists(srcPath))
                    {
                        TryCopyFile(srcPath, desPath);
                        return true;
                    }
                }
                catch (Exception e)
                {
                    Logger.ErrorFormat("CopyFile Exception:{0}", e.Message);
                }
            }

            return false;
        }

        public static bool MoveFile(string srcPath, string desPath)
        {
            if (!string.IsNullOrEmpty(srcPath) && !string.IsNullOrEmpty(desPath))
            {
                try
                {
                    if (File.Exists(srcPath))
                    {
                        TryMoveFile(srcPath, desPath);
                        return true;
                    }
                }
                catch (Exception e)
                {
                    Logger.ErrorFormat("MoveFile Exception:{0}", e.Message);
                }
            }

            return false;
        }

        public static bool CopyDirectoryFiles(string srcDir, string desDir, string extension = ".*")
        {
            if (!string.IsNullOrEmpty(srcDir) && !string.IsNullOrEmpty(desDir))
            {
                try
                {
                    srcDir = srcDir.Replace("\\", "/");
                    desDir = desDir.Replace("\\", "/");
                    if (Directory.Exists(srcDir))
                    {
                        string[] filePaths = Directory.GetFiles(srcDir, "*" + extension, SearchOption.AllDirectories);
                        foreach (var item in filePaths)
                        {
                            string desPath = item.Replace("\\", "/");
                            desPath = desDir + desPath.Substring(srcDir.Length);
                            TryCopyFile(item, desPath);
                        }

                        return true;
                    }
                }
                catch (Exception e)
                {
                    Logger.ErrorFormat("CopyFile Exception:{0}", e.Message);
                }
            }

            return false;
        }

        public static bool CopyDirectoryFiles(string srcDir, string desDir, HashSet<string> ignoreExtensions)
        {
            if (!string.IsNullOrEmpty(srcDir) && !string.IsNullOrEmpty(desDir))
            {
                try
                {
                    srcDir = srcDir.Replace("\\", "/");
                    desDir = desDir.Replace("\\", "/");
                    if (Directory.Exists(srcDir))
                    {
                        string[] filePaths = Directory.GetFiles(srcDir, "*.*", SearchOption.AllDirectories);
                        foreach (var item in filePaths)
                        {
                            string extension = Path.GetExtension(item);
                            if (!ignoreExtensions.Contains(extension))
                            {
                                string desPath = item.Replace("\\", "/");
                                desPath = desDir + desPath.Substring(srcDir.Length);
                                TryCopyFile(item, desPath);
                            }
                        }

                        return true;
                    }
                }
                catch (Exception e)
                {
                    Logger.ErrorFormat("CopyFile Exception:{0}", e.Message);
                }
            }

            return false;
        }

        public static bool MoveDirectory(string srcDir, string desDir)
        {
            if (!string.IsNullOrEmpty(srcDir) && !string.IsNullOrEmpty(desDir))
            {
                try
                {
                    srcDir = srcDir.Replace("\\", "/");
                    desDir = desDir.Replace("\\", "/");
                    if (Directory.Exists(srcDir))
                    {
                        string[] filePaths = Directory.GetFiles(srcDir, "*.*", SearchOption.AllDirectories);
                        foreach (var item in filePaths)
                        {
                            string desPath = item.Replace("\\", "/");
                            desPath = desDir + desPath.Substring(srcDir.Length);
                            TryMoveFile(item, desPath);
                        }

                        return true;
                    }
                }
                catch (Exception e)
                {
                    Logger.ErrorFormat("MoveDirectory Exception:{0}", e.Message);
                }
            }

            return false;
        }

        public static string ReadText(string path)
        {
            if (ExistsFile(path))
            {
                string content = File.ReadAllText(path);
                return content;
            }

            return null;
        }
        
        public static string GetMD5(Stream stream)
        {
            if (null != stream)
            {
                byte[] buff = MD5.Create().ComputeHash(stream);
                return MD5ByteToString(buff);
            }

            return null;
        }
        
        public static string GetMD5(byte[] data)
        {
            if (null != data)
            {
                byte[] buff = MD5.Create().ComputeHash(data);
                return MD5ByteToString(buff);
            }

            return null;
        }
        
        private static string MD5ByteToString(byte[] data)
        {
            if (null != data)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    sb.Append(data[i].ToString("x2"));
                }

                return sb.ToString();
            }

            return null;
        }
        

        public static string GetFileMD5(string path)
        {
            if (ExistsFile(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    return GetMD5(fs);
                }
            }

            return null;
        }

        public static long GetFileSize(string path)
        {
            if (ExistsFile(path))
            {
                return new FileInfo(path).Length;
            }

            return 0;
        }

        public static string GetStringMD5(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                byte[] data = Encoding.UTF8.GetBytes(text);
                byte[] buff = MD5.Create().ComputeHash(data);
                return MD5ByteToString(buff);
            }

            return null;
        }

        public static List<string> GetFiles(string dir, string extension = ".*")
        {
            List<string> files = new List<string>();
            if (ExistsDirectory(dir))
            {
                string[] filePaths = Directory.GetFiles(dir, "*" + extension, SearchOption.AllDirectories);
                foreach (var item in filePaths)
                {
                    files.Add(item.Replace("\\", "/"));
                }
            }

            return files;
        }

        public static List<string> GetFiles(string dir, HashSet<string> extensions)
        {
            List<string> files = new List<string>();
            if (ExistsDirectory(dir))
            {
                string[] filePaths = Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories);
                foreach (var item in filePaths)
                {
                    string extension = Path.GetExtension(item);
                    if (extensions.Contains(extension))
                    {
                        files.Add(item.Replace("\\", "/"));
                    }
                }
            }

            return files;
        }

        public static List<string> GetFilesByIgnoreExtensions(string dir, HashSet<string> ignoreExtensions)
        {
            List<string> files = new List<string>();
            if (ExistsDirectory(dir))
            {
                string[] filePaths = Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories);
                foreach (var item in filePaths)
                {
                    string extension = Path.GetExtension(item);
                    if (!ignoreExtensions.Contains(extension))
                    {
                        files.Add(item.Replace("\\", "/"));
                    }
                }
            }

            return files;
        }


        public static List<string> GetSubDirectories(string dir)
        {
            List<string> paths = new List<string>();
            if (ExistsDirectory(dir))
            {
                string[] dirs = Directory.GetDirectories(dir);
                foreach (var item in dirs)
                {
                    paths.Add(item.Replace("\\", "/"));
                }
            }

            return paths;
        }

        public static Texture2D Base64ToTexture2D(string base64Str)
        {
            if (!string.IsNullOrEmpty(base64Str))
            {
                Texture2D tex = new Texture2D(4, 4);
                byte[] data = Convert.FromBase64String(base64Str);
                tex.LoadImage(data);
                return tex;
            }

            return null;
        }

        public static string Texture2DToBase64(Texture2D tex)
        {
            if (null != tex)
            {
                byte[] bytes = tex.EncodeToPNG();
                return Convert.ToBase64String(bytes);
            }

            return null;
        }
    }
}