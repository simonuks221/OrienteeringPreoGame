using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffectController : MonoBehaviour
{
    [SerializeField]
    PostProcessVolume startVolume;

    private ColorGrading colorGradingStart;

    // Start is called before the first frame update
    void Start()
    {
        startVolume.profile.TryGetSettings(out colorGradingStart);
        colorGradingStart.active = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            StopCoroutine(FadeOut());
            colorGradingStart.active = true;
            StartCoroutine(FadeIn());
            startVolume.weight = 1f;
        }

        if(Input.GetKeyUp(KeyCode.E))
        {
            StopCoroutine(FadeIn());
            colorGradingStart.active = true;
            StartCoroutine(FadeOut());
            startVolume.weight = 0f;
        }
    }

    IEnumerator FadeIn()
    {
        for(float i = 0; i <= 1; i += 0.02f)
        {
            startVolume.weight = i;
            yield return null;
        }
    }

    IEnumerator FadeOut()
    {
        for(float i = 1; i >= 0; i -= 0.02f)
        {
            startVolume.weight = i;
            yield return null;
        }
    }
}
