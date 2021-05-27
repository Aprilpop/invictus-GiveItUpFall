using System;
using System.IO;
using UnityEngine;

namespace InvictusMoreGames
{
#if UNITY_EDITOR

	using UnityEditor;

	public class InvictusMoreGamesSettings : EditorWindow
	{
		private static EditorWindow window = null;

		[MenuItem("Tools/More Games/Settings")]
		public static void ShowWindow()
		{
			int width = 700;
			int height = 200;

			window = EditorWindow.GetWindow(typeof(InvictusMoreGamesSettings), true, "More Games Settings", true);
			window.minSize = window.maxSize = new Vector2(width, height);
			window.autoRepaintOnSceneChange = true;

			MoreGamesSettings.LoadSettings();
		}

		private void OnGUI()
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label("Game name (example: giveitup):");
			MoreGamesSettings.gameName = GUILayout.TextField(MoreGamesSettings.gameName);
			GUILayout.EndHorizontal();
			GUILayout.Space(5);

			GUILayout.BeginHorizontal();
			GUILayout.Label("Cross promo url (example: http://crosspromo.invictus.com:8888/):");
			MoreGamesSettings.promoUrl = GUILayout.TextField(MoreGamesSettings.promoUrl);
			GUILayout.EndHorizontal();
			GUILayout.Space(5);

			MoreGamesSettings.automaticPreloadData = GUILayout.Toggle(MoreGamesSettings.automaticPreloadData, "  All data preloading from the server");
			MoreGamesSettings.trackingFunction = GUILayout.Toggle(MoreGamesSettings.trackingFunction, "  Tracking function (android)");
			MoreGamesSettings.logMode = GUILayout.Toggle(MoreGamesSettings.logMode, "  LOG mode");
			MoreGamesSettings.debugMode = GUILayout.Toggle(MoreGamesSettings.debugMode, "  DEBUG mode");
			GUILayout.Space(20);

			if (GUILayout.Button("Save settings"))
			{
				if (EditorUtility.DisplayDialog("Sure?",
						"Are you sure you want to overwrite the current save?",
						"Yes",
						"No"))
				{
					SaveSettings();
					window.Close();
					MoreGamesSettings.LoadSettings();
				}
			}

			if (GUILayout.Button("Remove More Games plugin"))
			{
				if (EditorUtility.DisplayDialog("Sure?",
						"Sure?",
						"Yes",
						"No"))
				{
					window.Close();
					RemovePlugin();
				}
			}
		}

		private void RemovePlugin()
		{
			string path = Application.dataPath + "/../Assets/Plugins/InvictusMoreGames";
			if (Directory.Exists(path))
				Directory.Delete(path, true);

			AssetDatabase.Refresh();
		}

		public static void SaveSettings()
		{
#if UNITY_EDITOR
			bool ok = true;
			string errorLog = "";
			try
			{
				if (!Directory.Exists(MoreGamesSettings.directory))
					Directory.CreateDirectory(MoreGamesSettings.directory);

				JSONNode jsonData = JSON.Parse("{}");
				jsonData.Add(SettingTypes.PromoUrl.ToString(), MoreGamesSettings.promoUrl);
				jsonData.Add(SettingTypes.GameName.ToString(), MoreGamesSettings.gameName);
				jsonData.Add(SettingTypes.AutomaticDownloadData.ToString(), MoreGamesSettings.automaticPreloadData.ToString());
				jsonData.Add(SettingTypes.TrackingFunction.ToString(), MoreGamesSettings.trackingFunction.ToString());
				jsonData.Add(SettingTypes.LogMode.ToString(), MoreGamesSettings.logMode.ToString());
				jsonData.Add(SettingTypes.DebugMode.ToString(), MoreGamesSettings.debugMode.ToString());
				JSONNode jsonRoot = JSON.Parse("{}");
				jsonRoot.Add(SettingTypes.Settings.ToString(), jsonData);

				if (!File.Exists(MoreGamesSettings.settingsJSONPath))
					using (var sw = File.CreateText(MoreGamesSettings.settingsJSONPath))
						sw.Write(jsonRoot.ToString());
				else
					using (var sw = new StreamWriter(MoreGamesSettings.settingsJSONPath, false))
						sw.Write(jsonRoot.ToString());
			}
			catch (Exception e)
			{
				ok = false;
				errorLog = e.Message;
			}

			AssetDatabase.Refresh();

			if (ok)
				Logger.Log("Beállítások mentve (adatok helyessége nincs ellenőrízve)");
			else
				Logger.LogError("Hiba a beállítások mentése során: " + errorLog);
#endif
		}
	}

#endif

	public static class MoreGamesSettings
	{
		public static string gameName = "";
		public static string promoUrl = "http://crosspromo.invictus.com:8888/";
		public static string directory = Application.dataPath + "/../Assets/Plugins/InvictusMoreGames/Resources/";
		public static string settingsPath = directory + "MoreGamesSettings.txt";
		public static string settingsJSONPath = directory + "MoreGamesSettingsJSON.txt";
		public static bool automaticPreloadData = true;
		public static bool trackingFunction = true;
		public static bool debugMode = false;
		public static bool logMode = false;

		static MoreGamesSettings()
		{
			LoadSettings();
		}

		public static void LoadSettings()
		{
#if UNITY_EDITOR
			string text = "";
			try
			{
				text = File.ReadAllText(settingsJSONPath);
				LoadData(text);
			}
			catch
			{
				Logger.LogError("Még nincsenek mentett beállítások.");
			}
#else
			TextAsset asset = (TextAsset)Resources.Load("MoreGamesSettingsJSON");

			if (asset != null)
			{
				System.String textFromFile = asset.text;
				LoadData(textFromFile);
			}
			else
				Logger.LogError("More Games settings error.");
#endif
		}

		private static void LoadData(string jsonText)
		{
			var json = JSON.Parse(jsonText);
			var settings = json[SettingTypes.Settings.ToString()];

			if (settings[SettingTypes.PromoUrl.ToString()] != null)
				promoUrl = settings[SettingTypes.PromoUrl.ToString()];
			if (settings[SettingTypes.GameName.ToString()] != null)
				gameName = settings[SettingTypes.GameName.ToString()];
			if (settings[SettingTypes.AutomaticDownloadData.ToString()] != null)
				automaticPreloadData = Convert.ToBoolean(settings[SettingTypes.AutomaticDownloadData.ToString()]);
			if (settings[SettingTypes.TrackingFunction.ToString()] != null)
				trackingFunction = Convert.ToBoolean(settings[SettingTypes.TrackingFunction.ToString()]);
			if (settings[SettingTypes.LogMode.ToString()] != null)
				logMode = Convert.ToBoolean(settings[SettingTypes.LogMode.ToString()]);
			if (settings[SettingTypes.DebugMode.ToString()] != null)
				debugMode = Convert.ToBoolean(settings[SettingTypes.DebugMode.ToString()]);
		}
	}

	public enum SettingTypes
	{
		Settings,
		PromoUrl,
		GameName,
		AutomaticDownloadData,
		TrackingFunction,
		LogMode,
		DebugMode
	}
}