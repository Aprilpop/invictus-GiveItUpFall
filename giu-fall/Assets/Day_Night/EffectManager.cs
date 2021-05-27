using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using EasyButtons;

public enum OnOff { none, On, Off}

[System.Serializable]
public class ChromProps
{

	public float _Radius = -0.0025f;
	public float _Radius2 = -0.02f;
	public float _Str = 1.17f;
}



[ExecuteInEditMode]
public class EffectManager : MonoBehaviour {


	public static class EffectKeyWords
	{
		public static string GLOBAL_FOG = "GLOBAL_FOG";
		public static string _lightDir = "_lightDir";
		public static string GLO_USE_Bump = "GLOUSE_BUMP";
		public static string GLO_USE_SPEC = "GLO_USE_SPEC";
		public static string GLO_USE_SPECB = "GLO_USE_SPECB";
		public static string GLO_USE_RIM = "GLO_USE_RIM";
		public static string ForceTaget = "_ForceTaget";

		public static string _FogTex = "_FogTex";

		public static string ColorFade = "_ColorFade";
		public static string HeightFogColor = "_HeightFogColor";
		public static string FogColor = "_FogColor";
		public static string FogSE = "_FogSE";
		public static string FadeTarget = "_FadeTarget";
		public static string FadeDist = "_FadeDist";
		public static string BlendThreshold = "_BlendThreshold";

		public static string _RGBMOD = "_RGBMOD";
		public static string _BSCMOD = "_BSCMOD";
		public static string _tintScale = "_tintScale";
		public static string _tintColor = "_tintColor";
	}

	private static EffectManager instance = null;
	public static EffectManager Instance
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
	Vector3 RGBMOD;
	[SerializeField]
	Vector3 BSCMOD;
	[SerializeField]
	Vector2 tintScale = new Vector2(1.0f, -1.0f);
	[SerializeField]
	Vector3 tintColor;

	Camera cam;

	[Header("Overlay")]
	public Material overlayMaterialRef;
	Material overlayMaterial;
	Mesh overlayMesh;
	public float meshOffset = 1;

	[Header("LightNing")]
	public Material lightningMaterialRef;
	Material lightningMaterial;
	[SerializeField]
	Vector2 cubemapColor = new Vector2(0.2f, 0.5f);
	float _cubemapColor;
	Material cubemap;

	Mesh lightningyMesh;
	public float lightningmeshOffset = 1;



	[SerializeField]
	Vector2 lightningDelay;
	float _lightningDelay;
	[SerializeField]
	Vector2 lightningDuration;
	float _lightningDuration;
	float _lightningDurationO;
	[SerializeField]
	float color = 0.9f;
	//[SerializeField]
	//SfxArrayEnum lightningSound;

	[Header("Rain")]
	public Material rainMaterialRef;
	[SerializeField]
	Vector2 mainSpeed = Vector2.one;
	[SerializeField]
	float angle = 0.2f;
	float _angle;
	[SerializeField]
	float velo = 30;
	float _velo = 30;
	Vector3 lastPos;

	[System.Serializable]
	public class RainLayer
	{
		public Mesh mesh;
		public float planeScale;
		public Vector4 offset = Vector4.one;
		public Material rainMaterial;
		public float meshOffset = 1;
		public Vector2 Speed = new Vector2(0, 3);
	}

	public static RainLayer[] rainLayersS;

	//[EasyButtons.Button]
	public void CopyLayers()
	{
		rainLayersS = rainLayers;
	}
	//[EasyButtons.Button]
	public void PastLayers()
	{
		rainLayers = new RainLayer[rainLayersS.Length];
		rainLayers = (RainLayer[])rainLayersS.Clone();
	}

	[SerializeField]
	RainLayer[] rainLayers;

	[Header("Common Camera Effects")]
	[SerializeField]
	CommonCamEffectPres commonCamEffect;
	public CommonCamEffectPres CommonCamEffect { get{ return commonCamEffect;}}


	ChromaticDistortion chromatic;


	bool l;
	void SetOverlay()
	{
		Vector3 scaleT = (cam.aspect > 1) ? new Vector3(1f * cam.aspect, 1f, 1f) : new Vector3( 1f, 1f / cam.aspect, 1f);
		Vector3 campos = cam.transform.position + cam.transform.forward * meshOffset;

		Matrix4x4 matrix = Matrix4x4.TRS(campos, cam.transform.rotation, scaleT);
		
		overlayMaterial.SetPass(0);
		Graphics.DrawMeshNow(overlayMesh,matrix);
	}

	void SetLightning()
	{
		Vector3 scaleT = (cam.aspect > 1) ? new Vector3(1f * cam.aspect, 1f, 1f) : new Vector3(1f, 1f / cam.aspect, 1f);
		Vector3 campos = cam.transform.position + cam.transform.forward * meshOffset;

		Matrix4x4 matrix = Matrix4x4.TRS(campos, cam.transform.rotation, scaleT);

		if (_lightningDelay <= 0)
		{
			if(_lightningDuration <= 0 && !l)
			{
				_lightningDuration = _lightningDurationO = lightningDuration.RandomVec();
				_cubemapColor = cubemapColor.RandomVec();



				//if (lightningSound != SfxArrayEnum.unknown)
				//	SoundManager.Play(lightningSound, transform.position + Random.insideUnitSphere * Random.Range(-50, 50));


				l = true;
			}
			else if (_lightningDuration > 0 && l)
			{
				_lightningDuration -= Time.deltaTime;
				float cur = _lightningDuration / _lightningDurationO;

				float adj = (cur < 0.5f) ? cur * 2 : 1 - cur * 2;
				lightningMaterial.SetFloat("_ColorMulti", Mathf.Lerp(0, color, adj));
				cubemap.SetFloat("_ColorMulti", Mathf.Lerp(0, _cubemapColor, adj));
			}
			else
			{
				_lightningDelay = lightningDelay.RandomVec();
				l = false;
			}
		}
		else
		{
			_lightningDelay -= Time.deltaTime;
		}

		lightningMaterial.SetPass(0);
		Graphics.DrawMeshNow(lightningyMesh, matrix);
	}

	void SetRainLayer(RainLayer layer)
	{
		Vector3 scaleT = (cam.aspect > 1) ? new Vector3(1f * cam.aspect, 1f, 1f) : new Vector3(1f, 1f / cam.aspect, 1f);
		Vector3 forW = cam.transform.forward;
		forW.y = 0;
		Vector3 campos = cam.transform.position + forW * layer.meshOffset;
		Vector3 rot = cam.transform.rotation.eulerAngles;
		rot.x = 0;
		rot.z = 0;
		Matrix4x4 matrix = Matrix4x4.TRS(campos, Quaternion.Euler(rot), scaleT);
		layer.rainMaterial.SetVector("_MainTex_ST", new Vector4( layer.offset.x * layer.planeScale, layer.offset.y * layer.planeScale, layer.offset.z, layer.offset.w));
		layer.rainMaterial.SetFloat("_HorizontalSkew", _angle);
		layer.rainMaterial.SetVector("_Speed", layer.Speed * mainSpeed);

		
		layer.rainMaterial.SetPass(0);
		Graphics.DrawMeshNow(layer.mesh, matrix);
	}



	private void Awake()
	{
		if (!Application.isPlaying)
			return;


		if (instance != null && instance != this)
		{
			//Destroy(this.gameObject);
		}
		else
		{
			instance = this;
		}
		GetCommonCamEffect();
	}

	void GetCommonCamEffect()
	{
		if (commonCamEffect == null)
			commonCamEffect = (CommonCamEffectPres)Resources.Load("CommonCamEffectPres_01");
	}

#if UNITY_EDITOR
	OnOff lastValue;
#endif

	private void Start()
	{
		chromatic = GetComponent<ChromaticDistortion>();
		Set();
		SetEffectKeyWord(EffectKeyWords.GLO_USE_Bump);
		SetEffectKeyWord(EffectKeyWords.GLO_USE_SPEC, OnOff.On);
		SetEffectKeyWord(EffectKeyWords.GLO_USE_SPECB, OnOff.On);
		SetEffectKeyWord(EffectKeyWords.GLO_USE_RIM, OnOff.On);
#if UNITY_EDITOR
		if(Application.isPlaying)
		{
			//lastValue = (Shader.IsKeywordEnabled(EffectKeyWords.GLOBAL_FOG)) ? OnOff.On : OnOff.Off;
			SetEffectKeyWord(EffectKeyWords.GLOBAL_FOG, OnOff.On);
		}
		//else
		//	SetEffectKeyWord(EffectKeyWords.GLOBAL_FOG, lastValue);
#else
			SetEffectKeyWord(EffectKeyWords.GLOBAL_FOG, OnOff.On);
#endif
	}

	private void OnEnable()
	{
#if UNITY_EDITOR
		Set();
#endif
	}

	void GetCamera()
	{
		if (cam == null)
			cam = GetComponent<Camera>();
		if (cam == null)
			cam = Camera.main;
	}

	Mesh CreateMesh(float width, float height)
	{
		Mesh m = new Mesh();
		m.name = "OverlayMesh";
		m.vertices = new Vector3[] {
			new Vector3(-width, -height, meshOffset),
			new Vector3(width, -height, meshOffset),
			new Vector3(width, height, meshOffset),
			new Vector3(-width, height, meshOffset)
		};
		m.uv = new Vector2[] {
			new Vector2 (0, 0),
			new Vector2 (1, 0),
			new Vector2(1, 1),
			new Vector2 (0, 1)
		};
		m.triangles = new int[] { 0, 1, 2, 0, 2, 3 };
		return m;
	}



	void OnPostRender()
	{
		if (cam == null)
			return;
		if(overlayMaterialRef != null)
			SetOverlay();
		if (Application.isPlaying &&  lightningMaterialRef != null)
			SetLightning();

		SetGas();

		if (rainLayers == null || rainLayers.Length <= 0)
			return;
		_velo = (transform.position - lastPos).magnitude / Time.deltaTime;
		float dir = (transform.position.x - lastPos.x >0)?1:-1;
		lastPos = transform.position;

		_angle = (Mathf.Clamp(_velo,0, velo) / velo) * angle * dir;
		for (int i = 0; i < rainLayers.Length; i++)
		{
			SetRainLayer(rainLayers[i]);
		}

		
	}

	float fadeGas;
	float gasSin;

	void SetGas()
	{
		if(gasTime > 0)
		{
			gasTime -= Time.deltaTime;
		
			if (fadeGas < 1)
			{
				fadeGas += Time.deltaTime / commonCamEffect.gasFadeInOut.x;
			}
		}
		else
		{
			if (fadeGas >0)
			{
				fadeGas -= Time.deltaTime / commonCamEffect.gasFadeInOut.y;
			}
		}

		if(fadeGas >0)
		{
			overlayMaterial.SetColor("_ColorGas", commonCamEffect.gasColor);
			overlayMaterial.SetFloat("_FadeGas", fadeGas);

			gasSin = Mathf.Abs(Mathf.Sin(gasTime* commonCamEffect.gasPulseSpeed));
			overlayMaterial.SetFloat("_Fade2Gas",(1 - gasSin) +1);
		}
	}

	void Set()
	{
		GetCamera();

		if (overlayMaterialRef != null)
		{
			overlayMaterial = new Material( overlayMaterialRef);

            overlayMaterial.SetColor("_Color2", Color.white);
            overlayMaterial.SetFloat("_Fade",0);
            overlayMaterial.SetFloat("_Fade2", 1);
			overlayMaterial.SetFloat("_FadeGas", 0);
			overlayMesh = CreateMesh(1.0f, 1.0f);
		}
		else if (overlayMaterialRef == null )
		{
			if (overlayMesh != null)
			{
#if UNITY_EDITOR
				Object.DestroyImmediate(overlayMesh);
#else
				Object.Destroy(overlayMesh);
#endif
				overlayMesh = null;
			}
				
		}

		if (Application.isPlaying && lightningMaterialRef != null )
		{
			lightningMaterial = new Material(lightningMaterialRef);

			lightningyMesh = CreateMesh(1.0f, 1.0f);

			MeshRenderer renderer = DayNIght.Instance.CubeMap.GetComponent<MeshRenderer>();
			Material mat = new Material(renderer.sharedMaterial);
			renderer.sharedMaterial = cubemap = new Material(renderer.sharedMaterial);
		}

		if (rainMaterialRef != null && rainLayers.Length>0)
		{
			for (int i = 0; i < rainLayers.Length; i++)
			{
				RainLayer layer = rainLayers[i];
				layer.rainMaterial = new Material(rainMaterialRef);
				layer.mesh = CreateMesh(layer.planeScale, layer.planeScale);
			}
		}


		//SetOverlay();

		Shader.SetGlobalVector(EffectKeyWords._RGBMOD, RGBMOD);
		Shader.SetGlobalVector(EffectKeyWords._BSCMOD, BSCMOD);
		Shader.SetGlobalVector(EffectKeyWords._tintScale, tintScale);
		Shader.SetGlobalVector(EffectKeyWords._tintColor, tintColor);
	}

    public void ResetEffects()
    {
        Set();
    }

	public static void SetEffectKeyWord(string st,OnOff value = OnOff.none)
	{
		if(value == OnOff.none)
		{
			bool temp = Shader.IsKeywordEnabled(st);
			if(temp)
				Shader.DisableKeyword(st);
			else
				Shader.EnableKeyword(st);
		}
		else if (value == OnOff.On)
			Shader.EnableKeyword(st);
		else if (value == OnOff.Off)
			Shader.DisableKeyword(st);

		bool t = Shader.IsKeywordEnabled(st);
	}

	//[EasyButtons.Button("Global Fog ", ButtonMode.AlwaysEnabled)]
	public void SetGlobalFog(OnOff value = OnOff.none)
	{
		SetEffectKeyWord(EffectKeyWords.GLOBAL_FOG, value);
	}

	private void OnValidate()
	{
		if (!Application.isPlaying)
			Set();

		if (chromaMat != null)
			SetChroma();


	}

//	public void Hit( Vector2 _dam, float _damage = -1)
//	{
//		if (_damage > -1 && _damage < commonCamEffect.damageLimits.x)
//			return;
//		if (cur != null)
//		{
//			StopCoroutine(cur);
//			cur = null;
//		}
//#if UNITY_EDITOR
//		cur = HitR(_dam, (commonCamEffect.debugLimit > -1)? commonCamEffect.debugLimit : _damage);
//#else
//			cur = HitR(_dam, _damage);

//#endif

//		StartCoroutine(cur);
//	}

	float gasTime;

	public void GAs(float time)
	{
		gasTime += time;
	}

	#region Chroma


	[Header("Chromatic properties")]
	[Range(0, 1)]
	public float chromaAdj;
	//public ChromProps idle;
	//public ChromProps hit;

	float _Radius = -0.0025f;
	float _Radius2 = -0.02f;
	float _Str = 1.17f;


	Material chromaMat;

	void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
		if (chromaMat == null)
		{
			Shader chrom = Shader.Find("Scribe/ChromaticDistortion");
			chromaMat = new Material(chrom);

			SetChroma();
		}
#if UNITY_EDITOR
		SetChroma();
#endif
		Graphics.Blit(src, dest, chromaMat);
	}

	void SetChroma()
	{
		GetCommonCamEffect();
		float finalChromaAdj = Mathf.Max(chromaAdj, gasSin);
		_Radius = Mathf.Lerp(commonCamEffect.idle._Radius, commonCamEffect.hit._Radius, finalChromaAdj);
		_Radius2 = Mathf.Lerp(commonCamEffect.idle._Radius2, commonCamEffect.hit._Radius2, finalChromaAdj);
		_Str = Mathf.Lerp(commonCamEffect.idle._Str, commonCamEffect.hit._Str,Mathf.Clamp01(finalChromaAdj));

		chromaMat.SetFloat("_Radius", _Radius);
		chromaMat.SetFloat("_Radius2", _Radius2);
		chromaMat.SetFloat("_Str", _Str);
	}



	#endregion

	IEnumerator cur;

	//IEnumerator HitR(Vector2 _dam  ,float _damage = -1 )
	//{
	//	float heathRatio = GameLogic.Instance.Helicopter.Health / GameLogic.Instance.Helicopter.OriginalHealth ;
		
	//	overlayMaterial.SetColor("_Color2", commonCamEffect.hitColor);
	//	overlayMaterial.SetVector("_DamageDir", _dam);
	//	float last = overlayMaterial.GetFloat("_Fade");
	//	float timer = last;
	//	float _l = 1;
	//	float _a = 1;
	//	if (_damage > -1)
	//	{
	//		_a =Mathf.Max( (Mathf.Clamp(_damage, commonCamEffect.damageLimits.x, commonCamEffect.damageLimits.y) - commonCamEffect.damageLimits.x) / (commonCamEffect.damageLimits.y - commonCamEffect.damageLimits.x), commonCamEffect.hitColorMin);

	//		_l = Mathf.Lerp(commonCamEffect.damagePow.x, commonCamEffect.damagePow.y,_a);

	//	}
	//	overlayMaterial.SetFloat("_Fade2", _l );
	//	while (timer < 1)
	//	{
	//		timer += Time.deltaTime / commonCamEffect.fadeInOut.x;
			
	//		overlayMaterial.SetFloat("_Fade", (heathRatio < 0.4f)? Mathf.Max(timer * _a, commonCamEffect.hitColorRatio) : timer *_a);
	//		chromaAdj = timer;
	//		yield return null;
	//	}
	////	timer = 1;
	//	while ( timer > 0)
	//	{
	//		timer -= Time.deltaTime / commonCamEffect.fadeInOut.y;
	//		if((heathRatio < 0.4f) && timer * _a > commonCamEffect.hitColorRatio || (heathRatio >= 0.4f) && timer * _a > 0)
	//			overlayMaterial.SetFloat("_Fade", timer * _a);

	//		chromaAdj = timer;
	//		yield return null;
	//	}
	//	if(heathRatio < 0.4f)
	//		overlayMaterial.SetFloat("_Fade", commonCamEffect.hitColorRatio);


	//	cur = null;
	//}

}
