using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 批量移除 SkillEffect 文件夹下预制体的丢失(Missing)脚本组件
/// 使用方法：菜单栏 Tools -> Remove Missing Scripts
/// </summary>
public class RemoveScriptComponentTool : EditorWindow
{
    private string targetFolder = "Assets/GameData/BattleWorld/Prefabs/SkillEffect";
    private bool includeChildren = true;
    private bool previewMode = true;

    [MenuItem("Tools/Remove Missing Scripts")]
    public static void ShowWindow()
    {
        GetWindow<RemoveScriptComponentTool>("移除丢失组件");
    }

    private void OnGUI()
    {
        GUILayout.Label("批量移除 SkillEffect 预制体的丢失组件", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        targetFolder = EditorGUILayout.TextField("目标文件夹", targetFolder);
        EditorGUILayout.Space();

        includeChildren = EditorGUILayout.Toggle("包含子物体", includeChildren);
        previewMode = EditorGUILayout.Toggle("预览模式（只查看，不实际执行）", previewMode);

        EditorGUILayout.Space();

        if (GUILayout.Button(previewMode ? "扫描丢失组件" : "执行移除", GUILayout.Height(30)))
        {
            Execute();
        }
    }

    private void Execute()
    {
        if (!Directory.Exists(targetFolder))
        {
            Debug.LogError($"文件夹不存在: {targetFolder}");
            return;
        }

        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { targetFolder });
        if (prefabGuids.Length == 0)
        {
            Debug.Log("目标文件夹中没有找到预制体。");
            return;
        }

        int totalRemoved = 0;
        int modifiedPrefabs = 0;

        foreach (string guid in prefabGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            // 检测丢失组件数量
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null) continue;

            int missingCount = CountMissingScripts(prefab);
            if (missingCount == 0) continue;

            modifiedPrefabs++;

            if (previewMode)
            {
                Debug.Log($"[预览] {path} — 检测到 {missingCount} 个丢失组件");
            }
            else
            {
                // 实例化预制体并移除丢失组件
                GameObject instance = PrefabUtility.LoadPrefabContents(path);
                int removed = RemoveMissingScripts(instance);
                PrefabUtility.SaveAsPrefabAsset(instance, path);
                PrefabUtility.UnloadPrefabContents(instance);

                totalRemoved += removed;
                Debug.Log($"[已移除] {path} — 移除了 {removed} 个丢失组件");
            }
        }

        if (previewMode)
        {
            Debug.Log($"<color=yellow>预览完成：共扫描 {prefabGuids.Length} 个预制体，{modifiedPrefabs} 个存在丢失组件。请取消勾选\"预览模式\"后再次执行以应用修改。</color>");
        }
        else
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"<color=green>移除完成：处理了 {modifiedPrefabs} 个预制体，共移除 {totalRemoved} 个丢失组件。</color>");
        }
    }

    private int CountMissingScripts(GameObject go)
    {
        int count = 0;
        Transform[] targets = includeChildren ? go.GetComponentsInChildren<Transform>(true) : new[] { go.transform };
        foreach (var t in targets)
        {
            count += GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject);
        }
        return count;
    }

    private int RemoveMissingScripts(GameObject go)
    {
        int count = 0;
        Transform[] targets = includeChildren ? go.GetComponentsInChildren<Transform>(true) : new[] { go.transform };
        foreach (var t in targets)
        {
            int removed = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(t.gameObject);
            count += removed;
        }
        return count;
    }
}
