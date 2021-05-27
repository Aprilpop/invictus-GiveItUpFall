using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ChromaticDistortion : MonoBehaviour {
	//	[System.Serializable]
	//	public class ChromProps
	//	{

	//		public float _Radius = -0.0025f;
	//		public float _Radius2 = -0.02f;
	//		public float _Str = 1.17f;
	//	}
	//	[Header("Chromatic properties")]
	//	[Range(0, 1)]
	//	public float adj;
	//public ChromProps idle;
	//public ChromProps hit;

	//	float _Radius = -0.0025f;
	//	float _Radius2 = -0.02f;
	//	float _Str = 1.17f;


	//	Material mat;

	//	void OnRenderImage(RenderTexture src, RenderTexture dest)
	//	{
	//		if(mat == null)
	//		{
	//			Shader chrom = Shader.Find("Scribe/ChromaticDistortion");
	//			mat = new Material(chrom);

	//			SetChroma();
	//		}
	//#if UNITY_EDITOR
	//		SetChroma();
	//#endif
	//		Graphics.Blit(src, dest, mat);
	//	}

	//	void SetChroma()
	//	{
	//		_Radius = Mathf.Lerp(idle._Radius, hit._Radius, adj);
	//		_Radius2 = Mathf.Lerp(idle._Radius2, hit._Radius2, adj);
	//		_Str = Mathf.Lerp(idle._Str, hit._Str, adj);

	//		mat.SetFloat("_Radius", _Radius);
	//		mat.SetFloat("_Radius2", _Radius2);
	//		mat.SetFloat("_Str", _Str);
	//	}

	//	private void OnValidate()
	//	{
	//		if (mat == null)
	//			return;

	//		SetChroma();
	//	}
}
