using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CoreEditor.Tools
{
    public class ModelImportSetting
    {
        public static void ClearModelImportDefaultMaterial(string path)
        {
            AssetImporter assetImporter = AssetImporter.GetAtPath(path);
            if (assetImporter is ModelImporter importer)
            {
                importer.materialImportMode = ModelImporterMaterialImportMode.None;
                importer.useSRGBMaterialColor = false;
                importer.SaveAndReimport();
            }
        }

        public static void ClearModelImportDefaultMaterialAndAnimations(string path)
        {
            AssetImporter assetImporter = AssetImporter.GetAtPath(path);
            if (assetImporter is ModelImporter importer)
            {
                importer.materialImportMode = ModelImporterMaterialImportMode.None;
                importer.useSRGBMaterialColor = false;
                if (!importer.assetPath.Contains("@"))
                {
                    importer.animationType = ModelImporterAnimationType.None;
                    importer.importAnimation = false;
                }
                else
                {
                    importer.animationType = ModelImporterAnimationType.Generic;
                    importer.importAnimation = true;
                }

                importer.SaveAndReimport();
            }
        }


        [MenuItem("Assets/Tools/Clear Model Import Material And Animations", false, 5101)]
        private static void ClearModelImportMaterialAndAnimations()
        {
            List<Object> assets = AssetUtils.GetSelectAssets();
            foreach (var item in assets)
            {
                string path = AssetDatabase.GetAssetPath(item);
                ClearModelImportDefaultMaterialAndAnimations(path);
            }
        }


        [MenuItem("Assets/Tools/Clear Model Import Material", false, 5102)]
        private static void ClearModelImportMaterial()
        {
            List<Object> assets = AssetUtils.GetSelectAssets();
            foreach (var item in assets)
            {
                string path = AssetDatabase.GetAssetPath(item);
                ClearModelImportDefaultMaterial(path);
            }
        }
    }
}