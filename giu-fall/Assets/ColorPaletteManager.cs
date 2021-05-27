using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
public class ColorPaletteManager : MonoBehaviour
{
    private static ColorPaletteManager _instance;
    public static ColorPaletteManager Instance { get { return _instance; } }

    public const string PaletteColors = "_PaletteColors";
    public const string _HeightFogColorID = "_HeightFogColor";
    public const string _FogColorID = "_FogColor";
    public const string _FogSEID = "_FogSE";
    public const string _FadeTargetID = "_FadeTarget";

    [SerializeField]
    Light sun;

    [SerializeField]
    Color _sunColor;
    [SerializeField]
    float _sunInensity;

    [SerializeField]
    Color _ambientColor;

    [SerializeField]
    Color _GroundFogColor;

    [SerializeField]
    Color _HeightFogColor;

    [SerializeField]
    Color FogColor;

    [SerializeField]
    Vector2 fogHeightMinMax;

    [SerializeField]
    Vector2 fogMinMax;

    [SerializeField]
    Vector2 fogGroundMinMax;

    [SerializeField]
    ColorPalette[] ColorPalettes;

	[SerializeField]
	Color[] debug;

    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SetEnvironmentColor(0);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            SetEnvironmentColor(1);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            SetEnvironmentColor(2);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            SetEnvironmentColor(3);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            SetEnvironmentColor(4);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            SetEnvironmentColor(5);
        }

    }

    [SerializeField] Image gradient;

   

    public void SetEnvironmentColor(int loaded = -1)
    {
        int index = 0;
        if (loaded > -1)
            index = loaded;
        else
        {
            if (ProfileManager.Instance.levelColor == ColorPalettes.Length - 1)
                ProfileManager.Instance.levelColor = 0;
            else
                ProfileManager.Instance.levelColor++;

            index = ProfileManager.Instance.levelColor;
        }
       
        ProfileManager.Instance.Save();

        Color[] selectedColors = new Color[11] 
        {
            ColorPalettes[index].ColorGroup.Bg1,
            ColorPalettes[index].ColorGroup.Bg2,
            ColorPalettes[index].ColorGroup.Tower,
            ColorPalettes[index].ColorGroup.SimplePlatform,
            ColorPalettes[index].ColorGroup.GlassPlatform,
            ColorPalettes[index].ColorGroup.Fog,
            ColorPalettes[index].ColorGroup.AmbientColor,
            ColorPalettes[index].ColorGroup.SunColor,
            ColorPalettes[index].ColorGroup.GroundFog,
            ColorPalettes[index].ColorGroup.HeightFog,
            ColorPalettes[index].ColorGroup.Spike
        };

        SetColorS(selectedColors);
        _ambientColor = ColorPalettes[index].ColorGroup.AmbientColor;
        FogColor = ColorPalettes[index].ColorGroup.Fog;
        _sunColor = ColorPalettes[index].ColorGroup.SunColor;
        _GroundFogColor = ColorPalettes[index].ColorGroup.GroundFog;
        _HeightFogColor = ColorPalettes[index].ColorGroup.HeightFog;
        gradient.color = ColorPalettes[index].ColorGroup.Bg1;
        ShaderPropSet();
    }

	public void SetColorS(Color[] _color)
	{
		Vector4[] temp = new Vector4[_color.Length];
		for (int i = 0; i < temp.Length; i++)
		{
			temp[i] = new Vector4(_color[i].r, _color[i].g, _color[i].b, _color[i].a);
		}

		Shader.SetGlobalVectorArray(PaletteColors, temp);

		Vector4[] temp2 = Shader.GetGlobalVectorArray(PaletteColors);

		debug = new Color[temp2.Length];

		for (int i = 0; i < debug.Length; i++)
		{
			debug[i] = new Color(temp2[i].x, temp2[i].y, temp2[i].z, temp2[i].w);
		}
        ShaderPropSet();

    }

    public void ShaderPropSet()
    {
        

       // Shader.DisableKeyword("USE_GroundFOGText");
        //Shader.SetGlobalVector("_FogCenter", pos);
        //Camera.main.backgroundColor = FogColor;
        sun.color = _sunColor;
        sun.intensity = _sunInensity;
        RenderSettings.ambientLight = _ambientColor;

        //if (fogTexture != null)
        //{
        //    Shader.EnableKeyword("USE_GroundFOGText");
        //    Shader.SetGlobalTexture(_FogTexID, fogTexture);
        //}
        //else
        //{
        //    Shader.DisableKeyword("USE_GroundFOGText");
        //    Shader.SetGlobalTexture(_FogTexID, null);
        //}




        //Shader.SetGlobalVector(_FogLayer1ID, fogLayerA);
        //Shader.SetGlobalVector(_FogLayer2ID, fogLayerB);
        //Shader.SetGlobalColor("_GroundFogColor", _GroundFogColor);
        //Shader.SetGlobalColor(_HeightFogColorID, _HeightFogColor);
        //Shader.SetGlobalColor(_FogColorID, FogColor);
        //Shader.SetGlobalVector(_FogSEID, new Vector4(fogMinMax.x, fogMinMax.y, fogGroundMinMax.x, fogGroundMinMax.y));
        //Shader.SetGlobalVector(_FadeTargetID, fogHeightMinMax);
    }
    
    private void OnValidate()
    {
        //SetEnvironmentColor();

        //SetColorS(ColorPlate);

    }
}
