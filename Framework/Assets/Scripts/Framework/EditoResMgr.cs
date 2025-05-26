using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EditoResMgr : BaseManager<EditoResMgr>
{
    private string _rootPath = "Assets/Editor/ArtRes/";

    private EditoResMgr() { }

    //加载单个资源
    public T LoadEditorRes<T>(string path) where T : Object
    {
#if UNITY_EDITOR
        string suffixName = "";
        if (typeof(T) == typeof(GameObject))
            suffixName = ".prefab";
        else if (typeof(T) == typeof(Material))
            suffixName = ".mat";
        else if (typeof(T) == typeof(Texture))
            suffixName = ".png";
        else if (typeof(T) == typeof(AudioClip))
            suffixName = ".mp3";
        T res = AssetDatabase.LoadAssetAtPath<T>(_rootPath + path + suffixName);
        return res;
#else
        return null;
#endif
    }
    
    //加载图集相关资源的
    public Sprite LaodSprite(string path, string spriteName)
    { 
#if UNITY_EDITOR
        //加载图集子资源
        Object[] sprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(_rootPath + path);
        //遍历
        foreach (var item in sprites)
        {
            if(spriteName == item.name)
                return item as Sprite;
        }

        return null;
#else
        return null;
#endif
    }

    //加载图集文件中所有的子图集
    public Dictionary<string, Sprite> LoadSprites(string path)
    {
#if UNITY_EDITOR
        Dictionary<string, Sprite> spriteDic = new Dictionary<string, Sprite>();
        Object[] sprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(_rootPath + path);
        foreach (var item in sprites)
        {
            spriteDic.Add(item.name, item as Sprite);
        }

        return spriteDic;
#else
        return null;
#endif
    }
}
