using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DOFPresets : ScriptableObject
{
	[SerializeField]
	public float sphereRadius = 0.25f;
	public bool focusPointMode = false;
	public float zoomSpeed = 3;
	public float focusSpeed = 4;

	Transform focusObject;
	//public Vector3 focusPoint;

	[SerializeField]
	public Vector2 pov;
	[SerializeField]
	public AnimationCurve povCurve;
	[SerializeField]
	public Vector2 povAdj;

	[SerializeField]
	public Vector2 front = Vector2.one;




	public int RT_SIZE = 256;
	public float dofRadius = 1f;
	public int dofIterations = 2;
}
