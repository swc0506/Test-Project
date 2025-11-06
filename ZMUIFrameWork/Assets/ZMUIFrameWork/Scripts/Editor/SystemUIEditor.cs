using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SystemUIEditor : Editor
{
    [InitializeOnLoadMethod]
    private static void InitEditor()
    {
        //监听hierarchy发生改变的委托
        EditorApplication.hierarchyChanged += HanderTextOrImageRaycast;
    }

    private static void HanderTextOrImageRaycast()
    {
        GameObject obj = Selection.activeGameObject;
        if (obj != null)
        {
            if (obj.name.Contains("Text"))
            {
                Text text = obj.GetComponent<Text>();
                if (text != null)
                {
                    text.raycastTarget = false;
                }
            }
            else if (obj.name.Contains("Image"))
            {
                Image image = obj.GetComponent<Image>();
                if (image != null)
                {
                    image.raycastTarget = false;
                }
                else
                {
                    RawImage rawImage = obj.GetComponent<RawImage>();
                    if (rawImage != null)
                    {
                        rawImage.raycastTarget = false;
                    }
                }
            }
        }
    }
}
