#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace InvictusMoreGames
{
	public class InvictusMoreGamesAbout : EditorWindow
	{
		private static EditorWindow window = null;

		[MenuItem("Tools/More Games/About")]
		public static void ShowWindow()
		{
			int width = 220;
			int height = 60;

			window = GetWindow(typeof(InvictusMoreGamesAbout), true, "Invictus More Games About", true);
			window.minSize = window.maxSize = new Vector2(width, height);
			window.autoRepaintOnSceneChange = true;
		}

		private void OnGUI()
		{
			GUILayout.Label("Created by: Miklós Borsi (Mikuci)");
			GUILayout.Label("Minimum unity version: 2017.4.x");
			GUILayout.Label("Version: 1.0 (Voodoo copy)");
		}
	}
}

#endif