using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FontAnimation : MonoBehaviour
{
    public static FontAnimation fontAnimation;
    public float animationDuration = 2f;
    int defaultFontSize;
    int maxFontSize;
    public int iteration = 10;

    public StringEvent textEvent;
    public IntEvent fontsizeEvent;
    public void AnimatingFont(int _val)
    {
        //defaultFontSize = text.fontSize;
        StartCoroutine(IAnimating(_val));
       
    }

    IEnumerator IAnimating(int val)
    {
        int t = val/iteration;
        for (int i = 0; i < iteration; i++)
        {
            
        }
        yield return null;
       
    }


}
