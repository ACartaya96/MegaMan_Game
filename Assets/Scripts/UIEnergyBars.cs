using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEnergyBars : MonoBehaviour
{

    public static UIEnergyBars Instance= null;

    [System.Serializable]
    public struct EnergyBarsSctruct
    {
        public Image mask;
        public float size;
    }
    public EnergyBarsSctruct[] energyBarsStructs;

    public enum EnergyBars {PlayerHealth, PlayerWeapon};

    [SerializeField] Sprite[] energySprites;

    public enum EnergyBarsTypes
    {
        PlayerLife,
        SuperArm
    };

    void Awake()
    {
        if (Instance == null)
        {
        Instance = this;
        }
    }
    void Start()
    {
        foreach (EnergyBars energyBar in Enum.GetValues(typeof(EnergyBars)))
        {
            energyBarsStructs[(int)energyBar].size = energyBarsStructs[(int)energyBar].mask.rectTransform.rect.height;
        }
    }
    public void SetValue(EnergyBars energyBar, float value)
    {
        energyBarsStructs[(int)energyBar].mask.rectTransform.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Vertical, energyBarsStructs[(int)energyBar].size * value);
    }
    public void SetImage(EnergyBars energyBar, EnergyBarsTypes energyBarsType)
    {
       energyBarsStructs[(int)energyBar].mask.gameObject.transform.GetChild(0).GetComponent<Image>().sprite = energySprites[(int)energyBarsType];
    }
    public void SetVisibility(EnergyBars energyBar, bool visible)
    {
        energyBarsStructs[(int)energyBar].mask.gameObject.transform.parent.GetComponent<CanvasGroup>().alpha = visible ? 1f: 0f;
    }
}
