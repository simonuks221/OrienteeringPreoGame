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
    public const int mapResolution = 2;
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
                if(height[x, y] > heightAt - 0.1f && height[x, y] < heightAt + 0.1f)
                {
                    contourPoints.Add(new ContourPoint(x, y));
                }
            }
        }

        for(int i = 0; i < contourPoints.Count; i++)
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

/*
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
                if(height[x, y] > heightAt - 0.1f && height[x, y] < heightAt + 0.1f)
                {
                    contourPoints.Add(new ContourPoint(x, y));
                    contourPoints[contourPoints.Count - 1].connectedTo = new ContourPoint(-1, -1);
                }
            }
        }

        for(int i = 0; i < contourPoints.Count; i++)
        {
            if(contourPoints[i].connections != 2)
            {
                int closestIndex = FindClosestContourPoint(contourPoints, i);
                if(closestIndex != -1)
                {
                    DrawLine(contourPoints[closestIndex], contourPoints[i]);
                    contourPoints[i].connections++;
                    contourPoints[closestIndex].connections++;
                    contourPoints[i].connectedTo = contourPoints[closestIndex];
                    contourPoints[closestIndex].connectedTo = contourPoints[i];
                    i--;
                }
            }
        }

        foreach(ContourPoint c in contourPoints){
            texture.SetPixel(c.x, c.y, Color.green);
        }

        texture.Apply();
    }

    */

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

                Color[,] objectTexture = new Color[64, 64];
                for(int x = 0; x < 64; x++){
                    for(int y = 0; y < 64; y++){
                        objectTexture[x, y] = c.mapTexture.GetPixel(x, y);
                    }
                }

                for(int x = 0; x < 64; x++){
                    for(int y = 0; y < 64; y++){
                        texture.SetPixel((int)positionX + x - 32, (int)positionY + y - 32, objectTexture[x, y]);
                    }
                }

               
                Debug.Log("Object at: " + positionX + " " + positionY);
            }
        }
        texture.Apply();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
