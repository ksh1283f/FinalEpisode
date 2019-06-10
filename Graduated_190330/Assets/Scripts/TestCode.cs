using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lofle.Tween;

public class TestCode : Singletone<Testcode>
{

    [SerializeField] UITweenColorAlpha tweenColorAlpha;

    void Start()
    {
        tweenColorAlpha.PlayForward();
    }
    // public float minimum = 0.0f;
    // public float maximum = 1f;
    // public float duration = 5.0f;
    // private float startTime;
    // public SpriteRenderer sprite;
    // void Start()
    // {
    //     startTime = Time.time;
    //     startTime = Time.deltaTime;
    // }
    // void Update()
    // {
    //     float t = (Time.time - startTime) / duration;
    //     sprite.color = new Color(1f, 1f, 1f, Mathf.SmoothStep(minimum, maximum, t));
    //     Debug.LogError("smoothStep: " + Mathf.SmoothStep(minimum, maximum, t));

    //     //float startTime = Time.deltaTime;
    //     //while (!isDirectingEnd)
    //     //{
    //     //    if (backgroundImage == null)
    //     //        yield break;

    //     //    float t = (Time.deltaTime - startTime) / fadeDuration;
    //     //    alpha = Mathf.SmoothStep(minFade, maxFade, t);
    //     //    backgroundImage.color = new Color(1f, 1f, 1f, alpha);
    //     //    if (alpha >= maxFade)
    //     //        isDirectingEnd = true;
    //     //    Debug.Log("alpha: " + alpha);
    //     //    Debug.Log("t: " + t);
    //     //    yield return null;
    //     //}
    // }
}
