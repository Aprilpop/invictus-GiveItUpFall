using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(EffectManager))]
public class EffectManager_Editor : Editor
{
	const string globalFog = "globalFog";
	Color orig;
	EffectManager effectManager;
	public override void OnInspectorGUI()
	{
		effectManager = (EffectManager)target;

		//if (GUILayout.Button("HIt Test"))
		//{

		//	effectManager.Hit(effectManager.CommonCamEffect.damageDir);
		//}

		EditorGUILayout.BeginHorizontal();

		bool temp = false;
		if (EditorPrefs.HasKey(globalFog))
			temp = EditorPrefs.GetBool(globalFog);

		orig = GUI.color;
		if (temp)
			GUI.color = Color.green;
		else
			GUI.color = Color.red;


		if (GUILayout.Button("Global Fog"))
		{

			temp = !temp;

			effectManager.SetGlobalFog((temp)? OnOff.On : OnOff.Off);
			EditorPrefs.SetBool(globalFog, temp);
		}

		GUI.color = orig;

		if (GUILayout.Button("Copy RainLayer"))
		{
			effectManager.CopyLayers();
		}

		if ( EffectManager.rainLayersS != null && GUILayout.Button("Paste RainLayer"))
		{
			effectManager.PastLayers();
		}

		EditorGUILayout.EndHorizontal();

		if (!_checked && !Application.isPlaying)
			Check();

		DrawDefaultInspector();
	}

	private void OnEnable()
	{
		_checked = false;


	}

	bool _checked;
	void Check()
	{

		bool temp = true;
		if (EditorPrefs.HasKey(globalFog))
			temp = EditorPrefs.GetBool(globalFog);
		effectManager.SetGlobalFog((temp) ? OnOff.On : OnOff.Off);
		_checked = true;
	}
}
