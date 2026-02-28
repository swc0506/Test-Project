using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chessboard_88 : MonoBehaviour
{
    public int textureWidth = 256;
    public int textureHeight = 256;
    public int tileCount = 8;
    //棋盘颜色
    public Color color1 = Color.white;
    public Color color2 = Color.black;

    private void Start()
    {
        UpdateTexture();
    }

    /// <summary>
    /// 更新棋盘纹理
    /// </summary>
    public void UpdateTexture()
    {
        //根据纹理宽高和棋盘格子数，创建一个纹理
        Texture2D texture = new Texture2D(textureWidth, textureHeight);
        for (int x = 0; x < textureWidth; x++)
        {
            for (int y = 0; y < textureHeight; y++)
            {
                texture.SetPixel(x, y, (x / (textureWidth / tileCount) + y / (textureHeight / tileCount)) % 2 == 0 ? color1 : color2);
            }
        }
        texture.Apply();

        Renderer component = this.GetComponent<Renderer>();
        if (component != null)
        {
            component.sharedMaterial.mainTexture = texture;
        }
    }
}
