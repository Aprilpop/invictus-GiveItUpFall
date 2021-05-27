using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class FlareElement{
	public Vector4 uvOffset;
	public Color color;
	public bool rotate;
	public float scale;
	public float offset;
};


[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class Bloom : MonoBehaviour {

    static bool enablebloom = true;
    static List<Bloom> blooms = new List<Bloom>();
    public static bool EnableBloom
    {
        get
        {
            return enablebloom;
        }

        set
        {
            if (value != enablebloom)
            {
                enablebloom = value;
                for (int i = 0; i < blooms.Count; ++i)
                    blooms[i].enabled = value;
            }
        }
    }
    
    
    // Bloom Stuff
    [Header("Color properties")]
    public Vector3	RGBModifier = new Vector3 (0f, 0f, 0f);
	public Vector3	BSCModifier = new Vector3 (1f, 1f, 1f);

    [Header("Bloom properties")]
    public bool     enableBloom = true;
    public float	SplitIntensity=0f;
	public bool		useBoomColor = false;
	public Color	bloomColor = new Color(1f,1f,1f);
	public float	bloomRadius=1f;
	public Vector2 bloomRatio = new Vector2(3, 0.5f);
	public float	bloomIntensity = 1f;
	public int		bloomIterations = 2;
	public bool		showBoomonly;

	public int 		RT_SIZE = 128;
	Material		matBloom;
	RenderTexture	rtSplitter;
	RenderTexture	rtBlur;

	int 			idSplitter;
	int				idSplitColor;
	int				idUVStep;
	int 			idModChannel;
	int 			idModColor;
	int 			idBlurInt;
	int 			idBlurTex;


	// Flare stuff
	[Header("Flare properties")]
	public bool				enableFlare = true;
	public GameObject		flareNode;
	public float			flareScale = 0.5f;
    public Texture2D        dirtTexture;
    public Texture2D 		flareTexture;
	public FlareElement[]   flareElements;

	Camera 					mainCam;
	Material				matFlare;
	Mesh					planeMesh;
    Mesh                    flareMesh;
    Mesh                    flareTestMesh;
	Vector2 				flarePos;
	bool					renderFlare = false;

	int						idAlpha;
	int						idMainTex;
	int						idFlareTex;
	int						idCenter;
	int						idUVOffset;
	int						idFlareColor;




	Mesh CreateMesh(float width, float height)
	{
		Mesh m = new Mesh();
		m.name = "ScriptedMesh";
		m.vertices = new Vector3[] {
			new Vector3(-width, -height, 0.01f),
			new Vector3(width, -height, 0.01f),
			new Vector3(width, height, 0.01f),
			new Vector3(-width, height, 0.01f)
		};
		m.uv = new Vector2[] {
			new Vector2 (0, 0),
			new Vector2 (1, 0),
			new Vector2(1, 1),
			new Vector2 (0, 1)
		};
		m.triangles = new int[] { 0, 1, 2, 0, 2, 3};
		return m;
	}


    Mesh CreateFlareTestMesh(float width, float height)
    {
        Mesh m = new Mesh();
        m.name = "ScriptedMesh";
        m.vertices = new Vector3[] {
			new Vector3(-width, -height, 0f),
			new Vector3(width, -height, 0f),
			new Vector3(width, height, 0f),
			new Vector3(-width, height, 0f),

            new Vector3(-width, -height, 1f),
			new Vector3(width, -height, 1f),
			new Vector3(width, height, 1f),
			new Vector3(-width, height, 1f)
		};

        m.colors = new Color[]
        {
			new Color(1,1,1,1),
			new Color(1,1,1,1),
			new Color(1,1,1,1),
			new Color(1,1,1,1),

            new Color(0,0,0,0),
			new Color(0,0,0,0),
			new Color(0,0,0,0),
			new Color(0,0,0,0)
        };
        m.triangles = new int[] { 0, 1, 2, 0, 2, 3, 4, 5, 6, 4, 6, 7 };
        return m;
    }


	private void ReleaseTextures()
	{
		if (rtSplitter != null) {
			DestroyImmediate(rtSplitter);
			rtSplitter = null;
		}
		if (rtBlur != null) {
			DestroyImmediate(rtSplitter);
			rtBlur = null;
		}
	}
	
	private void CreateTextures()
	{
		ReleaseTextures ();
		rtSplitter = new RenderTexture (RT_SIZE, RT_SIZE, 0);
		rtSplitter.anisoLevel = 0;
		rtSplitter.wrapMode = TextureWrapMode.Clamp;
		rtSplitter.Create ();
		rtBlur = new RenderTexture (RT_SIZE, RT_SIZE, 0);
		rtBlur.anisoLevel = 0;
		rtBlur.wrapMode = TextureWrapMode.Clamp;
		rtBlur.Create ();
	}


	private void ReleaseMaterials()
	{
		if ( matBloom != null ){
			DestroyImmediate( matBloom );
			matBloom = null;
		}
		
		if ( matFlare != null ){
			DestroyImmediate( matFlare );
			matFlare = null;
		}
	}
	
	private void CreateMaterials()
	{
		ReleaseMaterials ();

		Shader bloom =	Shader.Find ("Scribe/Bloom");

		idSplitter = Shader.PropertyToID("_Splitter");
		idSplitColor = Shader.PropertyToID("_Color");
		idUVStep = Shader.PropertyToID("_UvStep");
		idModChannel = Shader.PropertyToID("_ModChannel");
		idModColor = Shader.PropertyToID("_ModColor");
		idBlurInt = Shader.PropertyToID("_BlurInt");
		idBlurTex = Shader.PropertyToID("_BlurTex");

		matBloom = new Material(bloom);



		Shader flare = Shader.Find("Scribe/Flare");

		idAlpha = Shader.PropertyToID("_Alpha");
		idMainTex = Shader.PropertyToID("_MainTex");
		idFlareTex = Shader.PropertyToID("_FlareTex");
		idCenter = Shader.PropertyToID("_Center");
		idUVOffset = Shader.PropertyToID("_UVOffset");
		idFlareColor = Shader.PropertyToID("_Color");

		matFlare = new Material(flare);
	}


	void Awake()
	{
		mainCam = GetComponent<Camera>();
		planeMesh = CreateMesh (1.0f, 1.0f);
        flareTestMesh = CreateFlareTestMesh(0.01f, 0.01f);
        flareMesh = new Mesh();
        flareMesh.vertices = new Vector3[flareElements.Length * 4];
        flareMesh.uv = new Vector2[flareElements.Length * 4];
        flareMesh.colors = new Color[flareElements.Length * 4];
        int[] triangles = new int[flareElements.Length * 6];
        for (int i = 0; i < flareElements.Length; ++i)
        {
            triangles[i * 6] = 0 + i * 4;
            triangles[i * 6 + 1] = 1 + i * 4;
            triangles[i * 6 + 2] = 2 + i * 4;
            triangles[i * 6 + 3] = 0 + i * 4;
            triangles[i * 6 + 4] = 2 + i * 4;
            triangles[i * 6 + 5] = 3 + i * 4;
        }
        flareMesh.triangles = triangles;

        this.enabled = Bloom.enablebloom;
        blooms.Add(this);
    }

    private void OnDestroy()
    {
        blooms.Remove(this);
    }


    void OnEnable()
	{
		CreateMaterials();
		CreateTextures ();
		Bloom.enablebloom = true;
	}
	
	void OnDisable()
	{
		ReleaseMaterials();
		ReleaseTextures ();
	}




	// Use this for initialization
	void Start () {



	
	}

	// Update is called once per frame
	void Update () {

		if (!enableFlare || flareNode == null)
        {
            renderFlare = false;
            return;
        }
		Vector3 sPos = mainCam.WorldToScreenPoint(flareNode.transform.position/* * 1000.0f*/);
		float x = Screen.width * 0.5f;
		float y = Screen.height * 0.5f;

		flarePos.x = (sPos.x - x) / x;
		flarePos.y = ((sPos.y - y) / y);
        if (flarePos.x < 1.0 && flarePos.x > -1.0 && flarePos.y < 1.0 && flarePos.y > -1.0 && sPos.z > 0)
        {
            renderFlare = true;

            Vector2 dir = -flarePos;
            dir.Normalize();
            Matrix4x4 rot,scale,trans;

            Vector3[] vertices = new Vector3[flareElements.Length * 4];
            Vector2[] uv = new Vector2[flareElements.Length * 4];
            Color[] colors = new Color[flareElements.Length * 4];

            for (int i = 0; i < flareElements.Length; ++i)
            {
                rot = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, (flareElements[i].rotate) ? (flarePos.x + flarePos.y) * 180.0f : 0f), new Vector3(1f, 1f, 1f));
                scale = Matrix4x4.Scale(new Vector3((flareElements[i].uvOffset.w - flareElements[i].uvOffset.x) * flareElements[i].scale * flareScale, (flareElements[i].uvOffset.z - flareElements[i].uvOffset.y) * flareElements[i].scale * flareScale, 1f));
                trans = scale * rot;
                trans.SetColumn(3, new Vector4(flarePos.x + dir.x * flareElements[i].offset, flarePos.y + dir.y * flareElements[i].offset, 0.99f, 1f));


                vertices[i * 4] = trans.MultiplyPoint(new Vector3(-1f, -1f, 0.01f));
                vertices[i * 4 + 1] = trans.MultiplyPoint(new Vector3(1f, -1f, 0.01f));
                vertices[i * 4 + 2] = trans.MultiplyPoint(new Vector3(1f, 1f, 0.01f));
                vertices[i * 4 + 3] = trans.MultiplyPoint(new Vector3(-1f, 1f, 0.01f));

                Vector4 r = flareElements[i].uvOffset;
                float tx = (r.x < r.z) ? r.x : r.z;
                float ty = (r.y < r.w) ? 1f - r.y : 1f - r.w;
                float sx = Mathf.Abs(r.z - r.x);
                float sy = Mathf.Abs(r.w - r.y);
                uv[i * 4] = new Vector2(0 * sx + tx, 0 * -sy + ty);
                uv[i * 4 + 1] = new Vector2(1 * sx + tx, 0 * -sy + ty);
                uv[i * 4 + 2] = new Vector2(1 * sx + tx, 1 * -sy + ty);
                uv[i * 4 + 3] = new Vector2(0 * sx + tx, 1 * -sy + ty);

                colors[i * 4] =  flareElements[i].color;
                colors[i * 4 + 1] = flareElements[i].color;
                colors[i * 4 + 2] = flareElements[i].color;
                colors[i * 4 + 3] = flareElements[i].color;
            }
            flareMesh.vertices = vertices;
            flareMesh.uv = uv;
            flareMesh.colors = colors;

        }
        else
        {
            renderFlare = false;
        }

	}
	void OnPostRender()
	{
		if (enableFlare && renderFlare) {

			Matrix4x4 mat = Matrix4x4.Scale(new Vector3(1f,mainCam.aspect, 1f));
			mat.SetColumn(3,new Vector4(flarePos.x,flarePos.y,0.0f,1f));
            matFlare.SetPass(0);
            Graphics.DrawMeshNow(flareTestMesh, mat);
		}

	}

	void OnRenderImage(RenderTexture src, RenderTexture dest) {
        if (dest != null)
            dest.DiscardContents();
        if (enableBloom) {
			matBloom.SetFloat (idSplitter, SplitIntensity);
			rtSplitter.DiscardContents ();
			if (!useBoomColor) {
				Graphics.Blit (src, rtSplitter, matBloom, 0);
			} else {
				matBloom.SetColor (idSplitColor, bloomColor);
				Graphics.Blit (src, rtSplitter, matBloom, 1);
			}
			
			for (int i = 0; i < bloomIterations; i++) {
				matBloom.SetVector (idUVStep, new Vector2 (bloomRadius * bloomRatio.x / RT_SIZE, 0f));
				rtBlur.DiscardContents ();
				Graphics.Blit (rtSplitter, rtBlur, matBloom, 2);
				
				matBloom.SetVector (idUVStep, new Vector2 (0f, bloomRadius * bloomRatio.y / RT_SIZE));
				rtSplitter.DiscardContents ();
				Graphics.Blit (rtBlur, rtSplitter, matBloom, 2);
			}
			matBloom.SetVector (idModChannel, RGBModifier);
			matBloom.SetVector (idModColor, BSCModifier);
			matBloom.SetFloat (idBlurInt, bloomIntensity);
			matBloom.SetTexture (idBlurTex, rtSplitter);
			if (!showBoomonly)
				Graphics.Blit (src, dest, matBloom, 3);
			else
				Graphics.Blit (src, dest, matBloom, 4);
		} 
		else
		{
            matBloom.SetVector(idModChannel, RGBModifier);
            matBloom.SetVector(idModColor, BSCModifier);
            Graphics.Blit(src, dest,matBloom,5);
			
		}

	

		if (enableFlare && renderFlare)
        {
			Graphics.SetRenderTarget (dest);
            matFlare.SetTexture(idFlareTex, src);
            matFlare.SetVector(idCenter, new Vector2((flarePos.x * 0.5f) + 0.5f, (flarePos.y * 0.5f) + 0.5f));

            if (dirtTexture != null)
            {
                matFlare.SetTexture(idMainTex, dirtTexture);
                matFlare.SetVector(idUVOffset, new Vector4(1f, 1f, 0f, 0f));
                matFlare.SetColor(idFlareColor, Color.white);
                matFlare.SetPass(1);
				Vector3 scaleT = (mainCam.aspect > 1) ? new Vector3(1f, 1f * mainCam.aspect, 1f) : new Vector3(1f / mainCam.aspect, 1f, 1f);
				Matrix4x4 mat = Matrix4x4.Scale(scaleT);
				mat.SetColumn(3,new Vector4(0f ,0f,0.99f,1f));
				Graphics.DrawMeshNow(planeMesh, mat);
            }



            matFlare.SetTexture(idMainTex, flareTexture);
			matFlare.SetPass (2);
			Graphics.DrawMeshNow (flareMesh, Matrix4x4.identity);

			Matrix4x4 rot,scale,trans;
            
			Vector2 dir = -flarePos;
			dir.Normalize();

			for (int i = 0; i < flareElements.Length;++i)
			{
				rot = Matrix4x4.TRS(Vector3.zero,Quaternion.Euler (0f, 0f, (flareElements[i].rotate) ?  (flarePos.x+flarePos.y)*180.0f : 0f),new Vector3 (1f, 1f, 1f));
				scale = Matrix4x4.Scale(new Vector3(flareElements[i].scale * flareScale, flareElements[i].scale * mainCam.aspect * flareScale, 1f));
				trans = scale * rot;
				trans.SetColumn(3,new Vector4(flarePos.x + dir.x * flareElements[i].offset ,flarePos.y + dir.y * flareElements[i].offset,0.99f,1f));

				Vector4 r =	flareElements[i].uvOffset;
				float tx = (r.x < r.z ) ? r.x : r.z;
				float ty = (r.y < r.w ) ? r.y : r.w;
				float sx = Mathf.Abs(r.z-r.x);
				float sy = Mathf.Abs(r.w-r.y);
				matFlare.SetVector(idUVOffset,new Vector4(sx,-sy,tx,1.0f - ty));
				matFlare.SetColor(idFlareColor,flareElements[i].color);
				matFlare.SetPass (1);
				Graphics.DrawMeshNow (planeMesh, trans);
 			}
          
		}

	}


	
}
