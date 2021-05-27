using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;

public class SoundManagerEnumHelper : AssetPostprocessor
{

	//public delegate void PostprocessAllAssetsDelagate(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths);
	//public static PostprocessAllAssetsDelagate OnPostprocessAllAssetsDelagate;


	static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		//if(OnPostprocessAllAssetsDelagate != null)
		//	OnPostprocessAllAssetsDelagate(importedAssets, deletedAssets, movedAssets, movedFromAssetPaths);

		List<String> strings = new List<string>(); ;
		SoundManager soundManager = null;
		MusicManager musicManager = null;
		foreach (string str in importedAssets)
		{
			try
			{
				if (str.Contains("SoundManager"))
				{
					GameObject go = (GameObject)AssetDatabase.LoadAssetAtPath(str, typeof(GameObject));
					if (go.name.Contains("SoundManager"))
						soundManager = go.GetComponent<SoundManager>();
				}

			}
			catch
			{
			}
			try
			{
				if (str.Contains("MusicManager"))
				{
					GameObject go = (GameObject)AssetDatabase.LoadAssetAtPath(str, typeof(GameObject));
					if (go.name.Contains("MusicManager"))
						musicManager = go.GetComponent<MusicManager>();
				}

			}
			catch
			{
			}

		}
		
		if(soundManager != null)
		{
			//
			Debug.Log("Found Sound MAnager");
			for (int i = 0; i < soundManager.sfxArray.Length; i++)
			{
				String sT = soundManager.sfxArray[i].name;
				sT = sT.Replace(" ", "_");
				strings.Add(sT);
				Debug.Log(soundManager.sfxArray[i].name);
			}

			string enumName = "SfxArrayEnum";
			string filePathAndName = "Assets/" + enumName + ".cs"; //The folder Scripts/Enums/ is expected to exist

			using (StreamWriter streamWriter = new StreamWriter(filePathAndName))
			{

				streamWriter.WriteLine(" [System.Serializable]");
				streamWriter.WriteLine("public enum " + enumName);
				streamWriter.WriteLine("{");
				streamWriter.WriteLine("unknown,");
				for (int i = 0; i < strings.Count; i++)
				{
					string _st = "\t" + strings[i];
					if (soundManager.sfxArray[i].enumId > 0)
						_st += " = " + soundManager.sfxArray[i].enumId;

					//streamWriter.WriteLine("\t" + strings[i] + ",");
					streamWriter.WriteLine(_st + ",");

				}
				streamWriter.WriteLine("}");
			}
			AssetDatabase.Refresh();
			Debug.Log("SoundManagerEnum Reimported");


			if(soundManager.Mixer != null)
			{
				AudioMixer mixer = soundManager.Mixer;
				Array parameters = (Array)mixer.GetType().GetProperty("exposedParameters").GetValue(mixer, null);

				List<string> ExposedParams = new List<string>();
				for (int i = 0; i < parameters.Length; i++)
				{
					var o = parameters.GetValue(i);
					string Param = (string)o.GetType().GetField("name").GetValue(o);
					ExposedParams.Add(Param);
				}

				enumName = "ExposedMixerEnum";
				filePathAndName = "Assets/" + enumName + ".cs"; //The folder Scripts/Enums/ is expected to exist

				using (StreamWriter streamWriter = new StreamWriter(filePathAndName))
				{

					streamWriter.WriteLine(" [System.Serializable]");
					streamWriter.WriteLine("public enum " + enumName);
					streamWriter.WriteLine("{");
					streamWriter.WriteLine("unknown,");
					for (int i = 0; i < ExposedParams.Count; i++)
					{

						streamWriter.WriteLine("\t" + ExposedParams[i] + ",");
					}
					streamWriter.WriteLine("}");
				}
				AssetDatabase.Refresh();
				Debug.Log("ExposedMixerEnum Reimported");
			}
		}

		if (musicManager != null)
		{
			//
			Debug.Log("Found Music MAnager");
			
			for (int i = 0; i < musicManager.musicGroups.Length; i++)
			{
				String sT = musicManager.musicGroups[i].name;
				sT = sT.Replace(" ", "_");
				strings.Add(sT);
				Debug.Log(musicManager.musicGroups[i].name);
			}

			string enumName = "MusicArrayEnum";
			string filePathAndName = "Assets/" + enumName + ".cs"; //The folder Scripts/Enums/ is expected to exist

			using (StreamWriter streamWriter = new StreamWriter(filePathAndName))
			{

				streamWriter.WriteLine(" [System.Serializable]");
				streamWriter.WriteLine("public enum " + enumName);
				streamWriter.WriteLine("{");
				streamWriter.WriteLine("unknown,");
				for (int i = 0; i < strings.Count; i++)
				{

					streamWriter.WriteLine("\t" + strings[i] + ",");
				}
				streamWriter.WriteLine("}");
			}
			AssetDatabase.Refresh();
			Debug.Log("Music ManagerEnum Reimported");
		}
	}
}
