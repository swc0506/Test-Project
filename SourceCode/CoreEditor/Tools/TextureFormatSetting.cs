using System;
using System.Collections.Generic;
using System.IO;
using Core;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CoreEditor.Tools
{
    public class TextureFormatSetting
    {
        [MenuItem("Assets/Tools/Texture/Compression 95% Format", false, 5200)]
        private static void BatchSetCompressionFormat95()
        {
            BatchSetCompressionFormat(95);
        }

        [MenuItem("Assets/Tools/Texture/Compression 90% Format", false, 5201)]
        private static void BatchSetCompressionFormat90()
        {
            BatchSetCompressionFormat(90);
        }

        [MenuItem("Assets/Tools/Texture/Compression 85% Format", false, 5202)]
        private static void BatchSetCompressionFormat85()
        {
            BatchSetCompressionFormat(85);
        }

        [MenuItem("Assets/Tools/Texture/Compression 80% Format", false, 5203)]
        private static void BatchSetCompressionFormat80()
        {
            BatchSetCompressionFormat(80);
        }

        [MenuItem("Assets/Tools/Texture/Compression 75% Format", false, 5204)]
        private static void BatchSetCompressionFormat75()
        {
            BatchSetCompressionFormat(75);
        }

        [MenuItem("Assets/Tools/Texture/Compression 70% Format", false, 5205)]
        private static void BatchSetCompressionFormat70()
        {
            BatchSetCompressionFormat(70);
        }

        [MenuItem("Assets/Tools/Texture/Compression 65% Format", false, 5206)]
        private static void BatchSetCompressionFormat65()
        {
            BatchSetCompressionFormat(65);
        }

        [MenuItem("Assets/Tools/Texture/Compression 60% Format", false, 5207)]
        private static void BatchSetCompressionFormat60()
        {
            BatchSetCompressionFormat(60);
        }

        [MenuItem("Assets/Tools/Texture/Compression 55% Format", false, 5208)]
        private static void BatchSetCompressionFormat55()
        {
            BatchSetCompressionFormat(55);
        }

        [MenuItem("Assets/Tools/Texture/Compression 50% Format", false, 5209)]
        private static void BatchSetCompressionFormat50()
        {
            BatchSetCompressionFormat(50);
        }

        [MenuItem("Assets/Tools/Texture/Check Texture Size Multiple Of 4", false, 5300)]
        private static void CheckTextureSizeMultipleOf4()
        {
            List<Object> assets = AssetUtils.GetSelectAssets();
            foreach (var item in assets)
            {
                CheckTextureSize(item, out var path, out var size, out var nearSize);
            }
        }

        [MenuItem("Assets/Tools/Texture/Resize Texture Size Multiple Of 4", false, 5301)]
        private static void ResizeTextureSizeMultipleOf4()
        {
            List<Object> assets = AssetUtils.GetSelectAssets();
            foreach (var item in assets)
            {
                if (CheckTextureSize(item, out var path, out var size, out var nearSize))
                {
                    TextureImporter textureImporter = (TextureImporter)AssetImporter.GetAtPath(path);

                    RenderTextureReadWrite readWrite = RenderTextureReadWrite.sRGB;
                    if (!textureImporter.sRGBTexture)
                    {
                        readWrite = RenderTextureReadWrite.Linear;
                    }

                    Texture2D newTexture = Resize((Texture2D)item, readWrite, (int)nearSize.x, (int)nearSize.y);
                    byte[] bytes = null;
                    TextureType textureType = AssetUtils.GetTextureType(path);
                    if (textureType == TextureType.PNG)
                    {
                        bytes = newTexture.EncodeToPNG();
                    }
                    else if (textureType == TextureType.JPG)
                    {
                        bytes = newTexture.EncodeToJPG();
                    }
                    else if (textureType == TextureType.EXR)
                    {
                        bytes = newTexture.EncodeToEXR();
                    }
                    else if (textureType == TextureType.TGA)
                    {
                        bytes = newTexture.EncodeToTGA();
                    }

                    if (null != bytes)
                    {
                        FileUtils.CreateFile(path, bytes);
                    }
                    else
                    {
                        Debug.LogWarningFormat("this texture resize fail:{0}", path);
                    }
                }
            }

            AssetDatabase.Refresh();
        }

        private static Texture2D Resize(Texture2D source, RenderTextureReadWrite readWrite, int width, int height)
        {
            RenderTexture rt = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.Default, readWrite);
            Graphics.Blit(source, rt);
            RenderTexture curr = RenderTexture.active;
            RenderTexture.active = rt;
            Texture2D result = new Texture2D(width, height, source.format, false);
            result.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            result.Apply();
            RenderTexture.active = curr;
            RenderTexture.ReleaseTemporary(rt);

            return result;
        }


        private static bool CheckTextureSize(Object item, out string path, out Vector2 size, out Vector2 nearSize)
        {
            path = null;
            size = GetTextureSize(item);
            nearSize = Vector2.zero;
            if (size.x > 0 && size.y > 0)
            {
                bool correctW = isMultipleOfFour(size.x, out nearSize.x);
                bool correctH = isMultipleOfFour(size.y, out nearSize.y);
                if (!correctW || !correctH)
                {
                    path = AssetDatabase.GetAssetPath(item);
                    Debug.LogFormat("{0},{1}->{2}", path, size.ToString(), nearSize.ToString());
                    return true;
                }
            }

            return false;
        }

        public static Vector2 GetTextureSize(Object item)
        {
            Vector2 size = Vector2.zero;
            if (item is Texture texture)
            {
                size.x = texture.width;
                size.y = texture.height;
            }
            else if (item is Sprite sprite)
            {
                size.x = sprite.texture.width;
                size.y = sprite.texture.height;
            }

            return size;
        }

        public static Vector2 GetTextureSize(string path)
        {
            Object item = AssetDatabase.LoadMainAssetAtPath(path);
            return GetTextureSize(item);
        }

        private static bool isMultipleOfFour(float num, out float near)
        {
            int val = 4;
            int res = (int)num % val;
            if (res > 0)
            {
                int offset = res / (float)val >= 0.5f ? 1 : 0;
                int times = (int)(num / (float)val);
                near = (times + offset) * val;
            }
            else
            {
                near = num;
            }

            return res == 0;
        }

        private static void BatchSetCompressionFormat(int compressionQuality)
        {
            List<Object> assets = AssetUtils.GetSelectAssets();
            foreach (var item in assets)
            {
                string path = AssetDatabase.GetAssetPath(item);
                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (null != importer)
                {
                    SetUITextureFormat(importer);
                    SetCompressionFormat(importer, compressionQuality);
                    importer.SaveAndReimport();
                }
            }
        }
        
        public static void SetUITextureFormat(TextureImporter importer)
        {
            bool hasAlpha = importer.DoesSourceTextureHaveAlpha();
            importer.isReadable = false;
            importer.mipmapEnabled = false;
            importer.filterMode = FilterMode.Bilinear;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.npotScale = TextureImporterNPOTScale.None;
            importer.alphaIsTransparency = hasAlpha;
            importer.alphaSource = hasAlpha ? TextureImporterAlphaSource.FromInput : TextureImporterAlphaSource.None;
        }

        public static void SetCompressionFormat(TextureImporter importer, int compressionQuality)
        {
            bool hasAlpha = importer.DoesSourceTextureHaveAlpha();

            TextureImporterPlatformSettings defaultSettings = importer.GetDefaultPlatformTextureSettings();
            defaultSettings.format = TextureImporterFormat.Automatic;
            defaultSettings.maxTextureSize = 2048;
            defaultSettings.textureCompression = TextureImporterCompression.Compressed;
            defaultSettings.crunchedCompression = true;
            defaultSettings.compressionQuality = compressionQuality;
            importer.SetPlatformTextureSettings(defaultSettings);

            TextureImporterPlatformSettings androidSettings = importer.GetPlatformTextureSettings("Android");
            if (!androidSettings.overridden)
            {
                androidSettings = new TextureImporterPlatformSettings();
                androidSettings.name = "Android";
                androidSettings.overridden = true;
            }

            if (hasAlpha)
            {
                androidSettings.format = TextureImporterFormat.ETC2_RGBA8Crunched;
                androidSettings.crunchedCompression = true;
            }
            else
            {
                androidSettings.format = TextureImporterFormat.ASTC_8x8;
                androidSettings.crunchedCompression = false;
            }

            androidSettings.compressionQuality = compressionQuality;
            importer.SetPlatformTextureSettings(androidSettings);


            TextureImporterPlatformSettings iOSSettings = importer.GetPlatformTextureSettings("iOS");
            if (!iOSSettings.overridden)
            {
                iOSSettings = new TextureImporterPlatformSettings();
                iOSSettings.name = "iOS";
                iOSSettings.overridden = true;
            }

            if (hasAlpha)
            {
                iOSSettings.format = TextureImporterFormat.ETC2_RGBA8Crunched;
                iOSSettings.crunchedCompression = true;
            }
            else
            {
                iOSSettings.format = TextureImporterFormat.ASTC_8x8;
                iOSSettings.crunchedCompression = false;
            }

            iOSSettings.compressionQuality = compressionQuality;
            importer.SetPlatformTextureSettings(iOSSettings);
        }

        public static void ClearCompressionFormat(TextureImporter importer)
        {
            TextureImporterPlatformSettings defaultSettings = importer.GetDefaultPlatformTextureSettings();
            defaultSettings.format = TextureImporterFormat.Automatic;
            defaultSettings.textureCompression = TextureImporterCompression.CompressedHQ;
            defaultSettings.crunchedCompression = false;
            defaultSettings.compressionQuality = 100;
            importer.SetPlatformTextureSettings(defaultSettings);

            TextureImporterPlatformSettings androidSettings = importer.GetPlatformTextureSettings("Android");
            androidSettings.overridden = false;
            androidSettings.format = TextureImporterFormat.Automatic;
            androidSettings.crunchedCompression = false;
            defaultSettings.compressionQuality = 100;
            importer.SetPlatformTextureSettings(androidSettings);


            TextureImporterPlatformSettings iOSSettings = importer.GetPlatformTextureSettings("iOS");
            iOSSettings.overridden = false;
            iOSSettings.format = TextureImporterFormat.Automatic;
            iOSSettings.crunchedCompression = false;
            iOSSettings.compressionQuality = 100;
            importer.SetPlatformTextureSettings(iOSSettings);
        }

    }
}