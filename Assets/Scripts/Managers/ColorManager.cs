using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum ColorsEnum
{
    None,
    Blue,
    Yellow,
    Red,
    DarkBlue,
    Green,
    Pink,
    Orange,
    Purple,
    Turquoise,
    Grey

}

[Serializable]
public class ColorClass
{
    public ColorsEnum colors;
    public Material bigMat;
}


public class ColorManager : MonoBehaviour
{
    public static ColorManager instance;

    public List<ColorClass> colors = new List<ColorClass>();

    public void Awake()
    {
        instance = this;
    }
    public Material GetMaterialFromColor(ColorsEnum en)
    {
        foreach (var item in colors)
        {
            if (item.colors==en)
            {
                  return item.bigMat;
            }
        }
        return null;
    }
}
