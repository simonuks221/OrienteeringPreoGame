using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BaseMapObject : MonoBehaviour
{
    public bool drawOnMap = true;
    [SerializeField]
    Texture2D mapTexture;
    public int mapTextureWidth;
    public int mapTextureHeight;

    public BaseMapObject()
    {
        try{
            mapTextureWidth = mapTexture.width;
        mapTextureHeight = mapTexture.height;
        }
        catch (Exception e){

        }
        
    }

    virtual public Color[,] mapTexturePixels()
    {
        Color[,] objectTexture = new Color[mapTextureWidth, mapTextureHeight];

        for(int x = 0; x < mapTextureWidth; x++){
            for(int y = 0; y < mapTextureHeight; y++){
                objectTexture[x, y] = mapTexture.GetPixel(x, y);
            }
        }

        return objectTexture;
    }
}
