using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlPointObject : BaseMapObject
{
    public int controllNumber;

    public override Color[,] mapTexturePixels()
    {
        Color[,] objectTexture = new Color[64, 64];

        for(int x = 0; x < 64; x++){
            for(int y = 0; y < 64; y++){
                objectTexture[x, y] = Color.red;
            }
        }
        return objectTexture;
    }
}
