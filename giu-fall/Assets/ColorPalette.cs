using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ColorGroup
{
    public Color Bg1;
    public Color Bg2;
    public Color Tower;
    public Color SimplePlatform;
    public Color GlassPlatform;
    public Color Fog;
    public Color AmbientColor;
    public Color SunColor;
    public Color GroundFog;
    public Color HeightFog;
    public Color Spike;
}

public enum eColorPaletteColors
{
    Bg1,
    Bg2,
    Tower,
    SimplePlatform,
    GlassPlatform,
    Fog,
    Spike
}

[CreateAssetMenu(fileName = "ColorPalette", menuName = "ScriptAbles/ColorPalette")]
public class ColorPalette : ScriptableObject
{
	public string info = "";

    [SerializeField]
    ColorGroup colorGroup;

    public ColorGroup ColorGroup { get { return colorGroup; } }

    
}


