using UnityEngine;
using System.Collections;

[System.Serializable]
public enum DebugAO { none, normal, depth, final}

[ExecuteInEditMode]
public class DepthNormals : MonoBehaviour
{
	[SerializeField]
	LayerMask mask;
	[SerializeField]
	bool debug = false;
	public DepthTextureMode depthTextureMode = DepthTextureMode.Depth;

	public bool useDOF;

	public DebugAO debugAO = DebugAO.none;

	public Camera cam;
	public Material matDOF;



	//public Helicopter heli;
	public Transform player;

	public Transform debugFocusObject;

	public bool followPlayer = true;
	public bool focusPointMode = false;

	[SerializeField]
	DOFPresets dOFPreset;



	Transform focusObject;
	Vector3 focusPoint;

	//[SerializeField]
	//float sphereRadius = 0.25f;
	//public float zoomSpeed = 3;
	//public float focusSpeed = 4;
	//[SerializeField]
	//Vector2 pov;
	//[SerializeField]
	//AnimationCurve povCurve;
	//[SerializeField]
	//Vector2 povAdj;

	//[SerializeField]
	//Vector2 front = Vector2.one;

	//public int RT_SIZE = 256;
	//public float dofRadius = 1f;
	//public int dofIterations = 2;

	Vector4 adj;


	RenderTexture rtSplitter;
	RenderTexture rtBlur;

	int idUVStep;

	void GetPreset()
	{
		if (dOFPreset == null)
			dOFPreset = (DOFPresets)Resources.Load("DOFPresets");
	}

	private void ReleaseTextures()
	{
		if (rtSplitter != null)
		{
			DestroyImmediate(rtSplitter);
			rtSplitter = null;
		}
		if (rtBlur != null)
		{
			DestroyImmediate(rtSplitter);
			rtBlur = null;
		}
	}

	private void CreateTextures()
	{
		ReleaseTextures();
		rtSplitter = new RenderTexture(dOFPreset.RT_SIZE, dOFPreset.RT_SIZE, 0,RenderTextureFormat.ARGB32);
		rtSplitter.anisoLevel = 0;
		rtSplitter.wrapMode = TextureWrapMode.Clamp;
		rtSplitter.Create();
		rtBlur = new RenderTexture(dOFPreset.RT_SIZE, dOFPreset.RT_SIZE, 0, RenderTextureFormat.ARGB32);
		rtBlur.anisoLevel = 0;
		rtBlur.wrapMode = TextureWrapMode.Clamp;
		rtBlur.Create();
	}

	private void ReleaseMaterials()
	{
		if (matDOF != null)
		{
			DestroyImmediate(matDOF);
			matDOF = null;
		}


	}

	private void CreateMaterials()
	{
		ReleaseMaterials();

		if (useDOF)
		{
			Shader depth = Shader.Find("Custom/DepthNormals");
			matDOF = new Material(depth);
		}


	}

	private void Awake()
	{
		GetPreset();
	}

	private void Start()
	{
		//if (heli == null)
		//{
		//	heli = FindObjectOfType<Helicopter>();
		//}
	}

	private void OnEnable()
	{
		GetPreset();
		if (cam == null)
			cam = GetComponent<Camera>();
		cam.depthTextureMode = depthTextureMode;
		CreateTextures();
		CreateMaterials();
		idUVStep = Shader.PropertyToID("_UvStep");

		//heli = null;
	}

	bool fading = false;
	float ap;
	float _apZ;
	float _pov;

	void Update()
	{
		if(debugFocusObject != null )
		{
				focusPoint = debugFocusObject.position;
		}
		else
		{
			if (followPlayer)
			{

				//if (heli != null && heli.Height > 0)
				//{
				//	//focusPoint = heli.transform.position;
				//	if (!focusPointMode)
				//		focusPoint = heli.transform.position;
				//	else
				//	{
				//		RaycastHit hit;
				//		if (Physics.SphereCast(cam.transform.position, dOFPreset.sphereRadius, heli.transform.position - cam.transform.position, out hit, 200, mask))
				//		{
					

				//				focusPoint = hit.point;

				//		}

				//	}
		
				//}
					

	
				//else 
				//{
				//	if(player == null)
				//	{
				//		GameObject _go = GameObject.FindGameObjectWithTag("Player");
				//		if (_go != null)
				//			player = _go.transform;
				//		if (player == null)
				//			return;
				//	}

				//	else
				//		focusPoint = player.position;
				//}
			}
			else
			{
				RaycastHit hit;
				if (Physics.SphereCast(cam.transform.position, dOFPreset.sphereRadius, cam.transform.forward, out hit, 200, mask))
				{
					focusObject = hit.transform;
					if (!focusPointMode)
						focusPoint = focusObject.position;
					else

						focusPoint = hit.point;

				}
		}

		}



		CalcAP();

		_apZ = (dOFPreset.povAdj.x - ap) / (dOFPreset.povAdj.x - dOFPreset.povAdj.y);
			_pov = Mathf.Lerp(dOFPreset.pov.x, dOFPreset.pov.y, dOFPreset.povCurve.Evaluate( _apZ));
		

	

		if(Mathf.Abs(adj.x - ap) > 0.001f)
		{
			
			adj.x = Mathf.Lerp(adj.x, ap, Time.deltaTime * dOFPreset.zoomSpeed);
		}
		if (Mathf.Abs(adj.y - _pov) > 0.001f)
		{
			
			adj.y = Mathf.Lerp(adj.y, _pov, Time.deltaTime * dOFPreset.focusSpeed);
		}

		adj.z = dOFPreset.front.x;
		adj.w = dOFPreset.front.y;
	}

	void CalcAP()
	{
		float dist = (cam.transform.position - focusPoint).magnitude;
		ap =  (dist - cam.nearClipPlane) / (cam.farClipPlane - cam.nearClipPlane);
	}

	// Called by the camera to apply the image effect
	void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		GetPreset();

		if (matDOF == null)
			return;

		if (debugAO == DebugAO.depth)
		{
			rtSplitter.DiscardContents();
			Graphics.Blit(source, rtSplitter);
			for (int i = 0; i < dOFPreset.dofIterations; i++)
			{
				matDOF.SetVector(idUVStep, new Vector2(dOFPreset.dofRadius / dOFPreset.RT_SIZE, 0f));
				rtBlur.DiscardContents();
				Graphics.Blit(rtSplitter, rtBlur, matDOF, 1);

				matDOF.SetVector(idUVStep, new Vector2(0f, dOFPreset.dofRadius / dOFPreset.RT_SIZE));
				rtSplitter.DiscardContents();
				Graphics.Blit(rtBlur, rtSplitter, matDOF, 1);
			}
			matDOF.SetVector("_Adj", adj);
			matDOF.SetTexture("_Blur", rtSplitter);
			Graphics.Blit(source, destination, matDOF, 2);
		}
		
	}

	private void OnGUI()
	{
		if(debug)
		GUI.Button(new Rect(10, 10, 300, 100), focusPoint.ToString() + ((focusObject != null)? focusObject.gameObject.name : " null"));
	}

	private void OnDrawGizmos()
	{
		if(focusObject != null)
		{
			Gizmos.color = Color.cyan;
			Gizmos.DrawWireSphere(focusPoint, 0.2f);
		}
	}
}