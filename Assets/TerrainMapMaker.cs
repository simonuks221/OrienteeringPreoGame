using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContourPoint{
    public int x, y;
    public int connections = 0;
    public bool startOfCont = false;
    public bool partOfCont = false;

    public ContourPoint(int _x, int _y){
        x = _x;
        y = _y;
    }
}

public class TerrainMapMaker : MonoBehaviour
{
    public static TerrainMapMaker Instance;
    [SerializeField]
    Terrain currentTerrain;
    
    float[,] height;
    public const int mapSizeX = 100, mapSizeY = 100;
    public const int mapResolution = 10;
    public Texture2D texture;
    
    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        texture = new Texture2D(mapSizeX * mapResolution, mapSizeY * mapResolution);

        DrawFoliage();

        for(int i = 1; i < 3; i++){
            DrawContours(i * 5);
        }

        DrawMapObjects();
        DrawCurveObjects();
    }

    void DrawCurveObjects()
    {
        BezierCurve[] allBezierCurves = FindObjectsOfType(typeof(BezierCurve)) as BezierCurve[];
        foreach(BezierCurve c in allBezierCurves)
        {
            foreach(Vector3 v in c.curvePoints)
            {
                texture.SetPixel((int)v.x * mapResolution, (int)v.z * mapResolution, Color.black);
            }
            
        }

        texture.Apply();
    }

    void DrawFoliage()
    {
        for(int x = 0; x < mapSizeX * mapResolution; x++){
            for(int y = 0; y < mapSizeY * mapResolution; y++){
                texture.SetPixel(x, y, Color.yellow);
            }
        }

        TreeInstance[] treeInstances = currentTerrain.terrainData.treeInstances;
        foreach(TreeInstance t in treeInstances){
            for(int xx = -3; xx < 3; xx++){
                for(int yy = -3; yy < 3; yy++){
                    texture.SetPixel((int)(t.position.x * mapSizeX * mapResolution + xx), (int)(t.position.z * mapSizeY * mapResolution + yy), Color.white);
                }
            }
        }

        texture.Apply();
    }

    void DrawLine(ContourPoint p1, ContourPoint p2)
    {
        float frac = 1/MathF.Sqrt(MathF.Pow(p2.x - p1.x, 2)+ MathF.Pow(p2.y - p1.y, 2));
        float ctr = 0;

        float tx = 0;
        float ty = 0;
        while(tx != p2.x || ty != p2.y){
            tx = Mathf.Lerp(p1.x, p2.x, ctr);
            ty = Mathf.Lerp(p1.y, p2.y, ctr);
            ctr += frac;
            texture.SetPixel((int)tx, (int)ty, Color.red);
        }
        
    }

    bool AnyNearbyContourPoints(List<ContourPoint> contourPoints, int currentIndex)
    {
        for(int i = 0; i < contourPoints.Count; i++)
        {
            if(i != currentIndex)
            {
                //if((contourPoints[i].x >= contourPoints[currentIndex].x - 1 && contourPoints[i].x <= contourPoints[currentIndex].x + 1) && (contourPoints[i].y >= contourPoints[currentIndex].y - 1 && contourPoints[i].y <= contourPoints[currentIndex].y + 1))
                //{
                //    return true;
                //}

                if(contourPoints[i].x == contourPoints[currentIndex].x && (contourPoints[i].y >= contourPoints[currentIndex].y - 1 && contourPoints[i].y <= contourPoints[currentIndex].y + 1))
                {
                    return true;
                }

                if (contourPoints[i].y == contourPoints[currentIndex].y && (contourPoints[i].x >= contourPoints[currentIndex].x - 1 && contourPoints[i].x <= contourPoints[currentIndex].x + 1))
                {
                    return true;
                }
            }
        }

        return false;
    }

    bool AnyNearbyEmpty(List<ContourPoint> contourPoints, int currentIndex)
    {
        int amountOfEmpty = 0;
        for(int x = -1; x <= 1; x++){
            for(int y = -1; y <= 1; y++)
            {

            }
        }

        if(amountOfEmpty == 0){
            return false;
        }

        return true;
    }

    void DrawContours(float heightAt)
    {
        height = new float[mapSizeX * mapResolution, mapSizeY * mapResolution];
        for(int x = 0; x < mapSizeX * mapResolution; x++){
            for(int y = 0; y < mapSizeY * mapResolution; y++){
                height[x, y] = currentTerrain.SampleHeight(new Vector3((float)x / mapResolution, 0, (float)y / mapResolution));
            }
        }

        List<ContourPoint> contourPoints = new List<ContourPoint>();
        for(int x = 0; x < mapSizeX * mapResolution; x++){
            for(int y = 0; y < mapSizeY * mapResolution; y++){
                if(height[x, y] > heightAt - 0.05f && height[x, y] < heightAt + 0.05f)
                {
                    contourPoints.Add(new ContourPoint(x, y));
                }
            }
        }

        //Clear out nearly spaced points
        for (int i = 0; i < contourPoints.Count; i++)
        {
            if (AnyNearbyContourPoints(contourPoints, i))
            {
                //contourPoints.RemoveAt(i);
                //i--;
            }
        }

        for(int i = 0; i < 10; i++)
        {
            if(AnyNearbyEmpty(contourPoints, i)){
                //contourPoints.RemoveAt(i);
                //i--;
            }
        }

        /*
        for (int i = 0; i < contourPoints.Count; i++)
        {
            if(!contourPoints[i].partOfCont && !contourPoints[i].startOfCont)
            {
                contourPoints[i].startOfCont = true;
                if(!FindNextContourSegment(contourPoints, i))
                {
                    Debug.LogError("Couldn't successfully draw connecting contour line");
                    return;
                }
                texture.Apply();
            }
        }
        */

        foreach(ContourPoint c in contourPoints){
            texture.SetPixel(c.x, c.y, Color.green);
        }

        texture.Apply();
    }

    bool FindNextContourSegment(List<ContourPoint> contourPoints, int currentIndex)
    {
        int closestContourIndex = FindClosestContourPoint(contourPoints, currentIndex);
        if(closestContourIndex != -1)
        {
            contourPoints[closestContourIndex].partOfCont = true;
            DrawLine(contourPoints[currentIndex], contourPoints[closestContourIndex]);
            texture.Apply();
            if(contourPoints[closestContourIndex].startOfCont)
            {
                return true;
            }
            else
            {
                if(!FindNextContourSegment(contourPoints, closestContourIndex))
                {
                    return false;
                }
            }
        }
        else
        {
            return false;
        }

        return true;
    }

    int FindClosestContourPoint(List<ContourPoint> contourPoints, int current)
    {
        int closestIndex = -1;
        float closestDistance = 10000000;
        for(int i = 0; i < contourPoints.Count; i++)
        {
                if(i != current && !contourPoints[i].partOfCont)
                {
                    float dist = MathF.Sqrt(MathF.Pow(contourPoints[i].x - contourPoints[current].x, 2)+ MathF.Pow(contourPoints[i].y - contourPoints[current].y, 2));
                    if (contourPoints[i].startOfCont)
                    {
                        dist *= 2;
                    }
                    if(dist < closestDistance){
                        closestDistance = dist;
                        closestIndex = i;
                    }
                }
        }

        return closestIndex;
    }

    

    void DrawMapObjects()
    {
        BaseMapObject[] allBaseMapObjectComponents = FindObjectsOfType(typeof(BaseMapObject)) as BaseMapObject[];

        foreach(BaseMapObject c in allBaseMapObjectComponents){
            if(c.drawOnMap){
                float positionX = c.gameObject.transform.position.x;
                float positionY = c.gameObject.transform.position.z;

                Color[,] objectTexture = c.mapTexturePixels();     
                for(int x = 0; x < c.mapTextureWidth; x++){
                    for(int y = 0; y < c.mapTextureHeight; y++){
                        texture.SetPixel((int)positionX * mapResolution  + x - c.mapTextureWidth/2, (int)positionY * mapResolution + y - c.mapTextureHeight/2, objectTexture[x, y]);
                    }
                }
            }
        }
        texture.Apply();
    }
}
