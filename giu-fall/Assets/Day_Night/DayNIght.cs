using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum DayPeriod
{
    DAY,
    NIGHT,
    UNKNOWN
};


[ExecuteInEditMode]
public class DayNIght : MonoBehaviour
{
	private static DayNIght instance;
	public static DayNIght Instance
	
		{
        get
        {
            return instance;
        }

    }

	private void OnDestroy()
	{
		instance = null;
	}



    [SerializeField]
	Light sun;
    [SerializeField]
    GameObject cubeMap;
    [SerializeField]
    Camera cam;
    Bloom bloom;
    [SerializeField]
    bool useBloom = false;
    [SerializeField]
    bool useFlare = false;
    [SerializeField]
    float flareOffset = 200;
    [SerializeField]
    GameObject flareNode = null;
    //[SerializeField]
    //float skyVectorAngle = 45;
    //[SerializeField]
    //float skyVectorAdjust = 2;
    //[SerializeField]
    //Transform skyVector = null;

	[Header("Adjust Stuff")]

    [SerializeField]
    Gradient gradientSunColor;
    [SerializeField]
    Gradient gradientAmbientColor;

    [SerializeField]
    AnimationCurve sunAngleX;
    [SerializeField]
    AnimationCurve sunAngleY;
    [SerializeField]
    AnimationCurve sunItensity;
    [SerializeField]
    AnimationCurve shadowItensity;
    [SerializeField]
    AnimationCurve nightMapItensity;

	[Header("Fog Stuff")]

	[SerializeField]
	Vector4 fogseDeb;

//#if UNITY_EDITOR
//	public bool NOFOG = false;
//#endif
	[SerializeField]
	bool targetedFog;
	public bool TargetedFog { get { return targetedFog; } }
#if UNITY_EDITOR
	[SerializeField]
	Vector3 fogTarget;
#endif
	[SerializeField]
	Gradient gradientFogColor;
	Color FogColor;
	[SerializeField]
	AnimationCurve fogMinCurve;
	[SerializeField]
	AnimationCurve fogMaxCurve;
	Vector2 fogMinMax;
    public Vector2 FogGroundMinMax { get { return fogGroundMinMax; } }
    public Vector2 FogMinMax { set { fogMinMax = value; } }

	[Space(10)]

	[Header("Ground Fog")]
	[SerializeField]
	Texture fogTexture;
	[SerializeField]
	bool IsGroundFogShaderSet;
	[SerializeField,ReadOnly]
	Texture fogTextureDebug;
	[SerializeField]
	Gradient gradientGroundFogColor;
	[SerializeField, ReadOnly]
	Color groundFogColorDeb;
	Color _GroundFogColor;

	[SerializeField]
	AnimationCurve groundFogMinCurve;
	[SerializeField]
	AnimationCurve groundFogMaxCurve;
	Vector2 fogGroundMinMax;
	[SerializeField]
	Vector4 fogLayerA = new Vector4(20, 20, 0.2f, 0);
	[SerializeField]
	Vector4 fogLayerB = new Vector4(50, 50, 0.2f, 0.2f);

	[Space(10)]
	[Header("Height Fog")]
	[SerializeField]
	Gradient gradientHeightFogColor;
	Color _HeightFogColor;
	[SerializeField]
	AnimationCurve heightFogMinCurve;
	[SerializeField]
	AnimationCurve heightFogMaxCurve;
	Vector2 fogHeightMinMax;

	[Header("Day Race Stuff")]

	public Cubemap dayCube;

	[Range(0.0f, 1.0f)]
	public float dayFloat = 0;

	[SerializeField]
	Vector2 dayRange = new Vector2( 0.2f, 0.5f);
    [SerializeField]
    Vector2 nightRange = new Vector2(0.8f, 1.0f);

    public bool timeElapse = false;
	public float speed;


    [Header("Night Race Stuff")]
	public Texture nightMap;
	public Vector4 nightMapTile;
	public GameObject glows;

	Vector3 _sunAngle;
	Color _sunColor;
	float _sunInensity;
	float _shadowItensity;
	Color _ambientColor;
	float _nightMapItensity;

    DayPeriod dayPeriod = DayPeriod.UNKNOWN;
	float dayFloatAdjust;


    public GameObject CubeMap { get { return cubeMap; } }

    public float GetRandomDayFloat(DayPeriod period )
    {
        Vector2 randomValue = Vector2.zero;
		if (period == DayPeriod.DAY || (period == DayPeriod.NIGHT && nightMap == null))
		{
			randomValue = dayRange;
		}
		else
		{
			randomValue = nightRange;

		}

		float value = Random.Range(randomValue.x, randomValue.y);
		Debug.Log(period + " " + randomValue + " " + value);
		return value ;
    }

    int _FogTexID;
    int _FogLayer1ID;
    int _FogLayer2ID;
    int _GroundColorID;
    int _HeightFogColorID;
    int _FogColorID;
    int _FogSEID;
    int _FadeTargetID;



    private void Awake()
	{
		_GroundColorID = Shader.PropertyToID("_GroundFogColor");
		_FogTexID = Shader.PropertyToID("_FogTex");
		_FogLayer1ID = Shader.PropertyToID("_FogLayer1");
		_FogLayer2ID = Shader.PropertyToID("_FogLayer2");

		_HeightFogColorID = Shader.PropertyToID("_HeightFogColor");
		_FogColorID = Shader.PropertyToID("_FogColor");
		_FogSEID = Shader.PropertyToID("_FogSE");
		_FadeTargetID = Shader.PropertyToID("_FadeTarget");
		if (!instance)
		{

			instance = this;



 


        }
        else
			DestroyImmediate(gameObject);
	}

	private void Start()
	{

		SetDay(dayFloat);
#if UNITY_EDITOR
		fogMinMax.x = fogMinCurve.Evaluate(dayFloat);
		fogMinMax.y = fogMaxCurve.Evaluate(dayFloat);
		Shader.SetGlobalVector("_FogCenter", new Vector4(0, 20, 0, 0));
#endif
	}

	private void OnEnable()
    {
    }

    // Update is called once per frame

    private void LateUpdate()
    {
		//if (instance == null)
		//	return;

		if (cam != null && cubeMap != null)
            cubeMap.transform.position = cam.transform.position;
    }

    void Update()
	{
		if (instance == null)
			return;

#if UNITY_EDITOR

		//        if (!Application.isPlaying)
		//        {
		if (cam == null)
                cam = Camera.main;
//        }
        if (timeElapse)
			{
				if (dayFloat < 1)
					dayFloat += Time.deltaTime * speed;
				else dayFloat = 0;

				SetDay(dayFloat);
			}
		//if(skyVector != null)
		//	SkyVectorUpdate();

		if (bloom == null)
		{
			if (useBloom)
				if (cam == null)
					bloom = Camera.main.GetComponent<Bloom>();
				else
					bloom = cam.GetComponent<Bloom>();
			//if (bloom != null)
			//	bloom.useBoomColor = true;
		}
		SetDay(dayFloat);
		//if (useBloom && useFlare)
		//	SetFlare();


#else
		if (timeElapse)
		{
			if (dayFloat < 1)
				dayFloat += Time.deltaTime * speed;
			else dayFloat = 0;

			SetDay(dayFloat);
		}
		//SkyVectorUpdate();
#endif

		//if (targetedFog)
		//{
		//	Shader.SetGlobalVector("_FogCenter", fogTarget);
		//}
		//else
		//{
		//	Shader.SetGlobalVector("_FogCenter", cam.transform.position);
		//}

		if (!targetedFog)
		{
			Vector3 pos = cam.transform.position;
			pos.y = 0;
			Shader.SetGlobalVector("_FogCenter", pos);
		}
	}

    public void SetDay(float timeofday,Camera thisCam = null)
	{
        dayFloat = timeofday;
        if (thisCam != null)
            cam = thisCam;
        else if (cam == null)
            cam = Camera.main;
        if (useBloom && bloom == null)
        {
            bloom = cam.GetComponent<Bloom>();
            if (bloom == null)
                useBloom = false;
            //else
            //    bloom.useBoomColor = true;
        }

        //if (useBloom && useFlare)
        //    SetFlare();

        if (timeofday > 1) timeofday = 1;
		else if (timeofday < 0) timeofday = 0;

		
		_sunColor = gradientSunColor.Evaluate(timeofday);
		_sunInensity = sunItensity.Evaluate(timeofday);
		_sunAngle =new Vector3( sunAngleX.Evaluate(timeofday), sunAngleY.Evaluate(timeofday), 0);
		_ambientColor = gradientAmbientColor.Evaluate(timeofday);
		_shadowItensity = Mathf.Clamp(shadowItensity.Evaluate(timeofday),0,1);
		_nightMapItensity = nightMapItensity.Evaluate(timeofday);

		FogColor = gradientFogColor.Evaluate(timeofday);
		if (!targetedFog)
		{
			fogMinMax.x = fogMinCurve.Evaluate(timeofday);
			fogMinMax.y = fogMaxCurve.Evaluate(timeofday);
		}


		_GroundFogColor = gradientGroundFogColor.Evaluate(timeofday);
		fogGroundMinMax.x = groundFogMinCurve.Evaluate(timeofday);
		fogGroundMinMax.y = groundFogMaxCurve.Evaluate(timeofday);

		_HeightFogColor = gradientHeightFogColor.Evaluate(timeofday);
		fogHeightMinMax.x = heightFogMinCurve.Evaluate(timeofday);
		fogHeightMinMax.y = heightFogMaxCurve.Evaluate(timeofday);

//#if UNITY_EDITOR
//		if(NOFOG)
//		{
//			FogColor.a = _GroundFogColor.a = _HeightFogColor.a= 0;
//		}
//#endif

		if (_nightMapItensity >=0)
			SetNight(DayPeriod.NIGHT);
		else
			SetNight(DayPeriod.DAY);
		
		sun.color = _sunColor;
		sun.intensity = _sunInensity;
		sun.shadowStrength = _shadowItensity;
		sun.transform.rotation = Quaternion.Euler(_sunAngle);
		RenderSettings.ambientLight = _ambientColor;

        if (useBloom)
            bloom.bloomColor = _sunColor;
        //if (flareNode != null && useFlare)
        //    flareNode.transform.position = cam.transform.position + Vector3.Normalize(-sun.transform.forward) * flareOffset;
    }

 //   void SkyVectorUpdate()
	//{
	//	Vector3 camRot = cam.transform.rotation.eulerAngles;
	//	skyVector.rotation = Quaternion.Euler(skyVectorAngle, camRot.y + 180, 0);
	//	Vector3 skyvec = Vector3.Normalize(skyVector.forward) * skyVectorAdjust;
	//	Shader.SetGlobalVector("_SkyVector", skyvec);
	//}

	void SetNight(DayPeriod period)
	{
        //Shader.SetGlobalFloat("_NightIntensity", _nightMapItensity);

		cam.backgroundColor = FogColor;
		if(fogTexture != null)
		{
			Shader.EnableKeyword("USE_GroundFOGText");
			Shader.SetGlobalTexture(_FogTexID, fogTexture);
		}
		else
		{
			Shader.DisableKeyword("USE_GroundFOGText");
			Shader.SetGlobalTexture(_FogTexID, null);
		}




		Shader.SetGlobalVector(_FogLayer1ID, fogLayerA);
		Shader.SetGlobalVector(_FogLayer2ID, fogLayerB);
		Shader.SetGlobalColor("_GroundFogColor", _GroundFogColor);
		Shader.SetGlobalColor(_HeightFogColorID, _HeightFogColor);
		Shader.SetGlobalColor(_FogColorID, FogColor);
		Shader.SetGlobalVector(_FogSEID, new Vector4(fogMinMax.x, fogMinMax.y, fogGroundMinMax.x, fogGroundMinMax.y));
		Shader.SetGlobalVector(_FadeTargetID, fogHeightMinMax);

		IsGroundFogShaderSet = Shader.IsKeywordEnabled("USE_GroundFOGText");
		fogTextureDebug = Shader.GetGlobalTexture(_FogTexID);
		fogseDeb = Shader.GetGlobalVector(_FogSEID);
		groundFogColorDeb = Shader.GetGlobalColor("_GroundFogColor");

		//if (period != dayPeriod)
		//      {
		//          dayPeriod = period;
		//          if (period == DayPeriod.NIGHT && nightMap != null)
		//          {
		//	//	Debug.Log("NIGHT");
		//              Shader.EnableKeyword("NIGHTRACE");
		//              Shader.SetGlobalTexture("_NightMap", nightMap);
		//              Shader.SetGlobalVector("_NightMapTile", new Vector3(nightMapTile.x,nightMapTile.y,1.0f/nightMapTile.z));
		//              if (glows != null)
		//                  glows.SetActive(true);
		//          }
		//          else
		//          {
		//              Shader.DisableKeyword("NIGHTRACE");
		//              if (glows != null)
		//                  glows.SetActive(false);
		//          }
		//          if (dayCube != null)
		//	{
		//		Shader.SetGlobalTexture("_Cube", dayCube);
		//		RenderSettings.defaultReflectionMode = UnityEngine.Rendering.DefaultReflectionMode.Custom;
		//		RenderSettings.customReflection = dayCube;

		//	}
		//}
	}


	public void EnableBloom()
	{
		if (bloom.enableBloom)
			bloom.enableBloom = false;
		else
			bloom.enableBloom = true;
	}

	//void SetFlare()
	//{
	//	if(flareNode == null)
	//	{
	//		flareNode = new GameObject("FlareNode");
	//		flareNode.transform.parent = this.transform;
	//	}
	//       if (bloom.flareNode != flareNode)
	//       {
	//           bloom.flareNode = flareNode;
	//           bloom.enableFlare = true;
	//       }
	//   }


	//float w = 300;
	//float h = 30;
	//private void OnGUI()
	//{
	//		GUI.Button(new Rect(Screen.width / 2 - w / 2, 30, w, h), (fogTexture != null)? ("KeyWord: " + Shader.IsKeywordEnabled("USE_GroundFOGText") + "  FogTExture: " + Shader.GetGlobalTexture("_FogTex").name) : "  FogTExture: NONE ");
		
	//}

    public void Teleport(Vector3 translate)
    {
        transform.position += translate;
        Vector3 pos = cam.transform.position;
        pos.y = 0;
        Shader.SetGlobalVector("_FogCenter", pos);
    }
}
