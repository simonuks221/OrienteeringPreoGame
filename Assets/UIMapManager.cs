
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMapManager : MonoBehaviour
{
    [SerializeField]
    Image smallMapImage;
    [SerializeField]
    Image bigMapImage;

    Vector2 startSmallMapPosition;
    Vector2 startBigMapPosition;
    RectTransform smallImageMapTransform;
    RectTransform bigImageMapTransform;
    Vector2 smallMapSize;
    Vector2 bigMapSize;

    void Start()
    {
        smallImageMapTransform = smallMapImage.GetComponent<RectTransform>();
        smallImageMapTransform.Rotate(new Vector3(0, 0, -Vector3.Angle(new Vector3(0, 0, 1), PlayerManager.Instance.transform.forward)));
        smallMapImage.sprite = Sprite.Create(TerrainMapMaker.Instance.texture, new Rect(0, 0, TerrainMapMaker.mapSizeX * TerrainMapMaker.mapResolution, TerrainMapMaker.mapSizeY * TerrainMapMaker.mapResolution), new Vector2(0.5f, 0.5f));

        bigImageMapTransform = bigMapImage.GetComponent<RectTransform>();
        bigImageMapTransform.Rotate(new Vector3(0, 0, -Vector3.Angle(new Vector3(0, 0, 1), PlayerManager.Instance.transform.forward)));
        bigMapImage.sprite = Sprite.Create(TerrainMapMaker.Instance.texture, new Rect(0, 0, TerrainMapMaker.mapSizeX * TerrainMapMaker.mapResolution, TerrainMapMaker.mapSizeY * TerrainMapMaker.mapResolution), new Vector2(0.5f, 0.5f));
        bigMapImage.enabled = false;

        startSmallMapPosition = smallMapImage.GetComponent<RectTransform>().position;
        startBigMapPosition = bigMapImage.GetComponent<RectTransform>().position;

        smallMapSize = smallImageMapTransform.sizeDelta;
        bigMapSize = bigImageMapTransform.sizeDelta;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            StopCoroutine(MoveMapBackwards());
            StartCoroutine(MoveMapForwards());
        }

        if (Input.GetKeyUp(KeyCode.M))
        {
            StopCoroutine(MoveMapForwards());
            StartCoroutine(MoveMapBackwards());
        }
    }

    IEnumerator MoveMapForwards()
    {
        for(float i = 0; i < 1; i += 0.075f)
        {
            smallImageMapTransform.position = startSmallMapPosition + (startBigMapPosition - startSmallMapPosition) * i;
            smallImageMapTransform.sizeDelta = smallMapSize + (bigMapSize - smallMapSize) * i;
            yield return null;
        }
        smallMapImage.enabled = false;
        bigMapImage.enabled = true;
    }

    IEnumerator MoveMapBackwards()
    {
        smallMapImage.enabled = true;
        bigMapImage.enabled = false;
        for(float i = 1; i > 0; i -= 0.075f)
        {
            smallImageMapTransform.position = startSmallMapPosition + (startBigMapPosition - startSmallMapPosition) * i;
            smallImageMapTransform.sizeDelta = smallMapSize + (bigMapSize - smallMapSize) * i;
            yield return null;
        }
    }
}
