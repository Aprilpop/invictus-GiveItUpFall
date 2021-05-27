using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonCamEffectPres : ScriptableObject
{

	[Header("Hit")]
	public Vector2 damageDir;
	[SerializeField]
	public Color hitColor = Color.red;
	[SerializeField]
	public Vector2 fadeInOut = new Vector2(1.5f, 3);
	[SerializeField]
	public Vector2 damageLimits = new Vector2(4, 30);
	[SerializeField]
	public Vector2 damagePow = new Vector2(10, 0.5f);
#if UNITY_EDITOR
	[SerializeField]
	public float debugLimit = -1;
#endif
	[SerializeField]
	public float hitColorRatio = 0.4f;
	[SerializeField]
	public float hitColorMin = 0.3f;

	[Header("Gas")]
	[SerializeField]
	public Color gasColor = Color.red;
	[SerializeField]
	public Vector2 gasFadeInOut = new Vector2(0.5f, 2);
	[SerializeField]
	public float gasPulseSpeed = 5;

	[Header("Chromatic properties")]
	public ChromProps idle;
	public ChromProps hit;
	public ChromProps gas;
}
