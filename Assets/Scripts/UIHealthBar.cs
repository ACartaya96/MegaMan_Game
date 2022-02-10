using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthBar : MonoBehaviour
{
    public Image mask;
    float originalSize;
  
    public static UIHealthBar instance{get; private set; }

    void awake()
    {
        instance = this;
    }
    private void Start()
    {
       // originalSize = mask.rectTransform.rect.height;
    }
    public void setValue(float value)
    {
        //mask.rectTransform.SetSizeWithCurrentAnchors(rectTransform.Axis.Vertical, originalSize * value);
    }
  
}
