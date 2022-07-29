
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMapManager : MonoBehaviour
{
    [SerializeField]
    Image mapImageComponent;

    void Start()
    {
        RectTransform mapImageTransform = mapImageComponent.GetComponent<RectTransform>();
        mapImageTransform.Rotate(new Vector3(0, 0, -Vector3.Angle(new Vector3(0, 0, 1), PlayerManager.Instance.transform.forward)));
    }
    void Update()
    {
        mapImageComponent.sprite = Sprite.Create(TerrainMapMaker.Instance.texture, new Rect(0, 0, TerrainMapMaker.mapSizeX * TerrainMapMaker.mapResolution, TerrainMapMaker.mapSizeY * TerrainMapMaker.mapResolution), new Vector2(0.5f, 0.5f));
    }
}
