#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace InvictusMoreGames
{
	public class InvictusMoreGamesHelp : EditorWindow
	{
		private static EditorWindow window = null;

		[MenuItem("Tools/More Games/Help")]
		public static void ShowWindow()
		{
			int width = 455;
			int height = 525;

			window = EditorWindow.GetWindow(typeof(InvictusMoreGamesHelp), true, "Help", true);
			window.minSize = window.maxSize = new Vector2(width, height);
			window.autoRepaintOnSceneChange = true;
		}

		private void OnGUI()
		{
			GUILayout.Label("Példa kód:");
			GUILayout.Label("public class OurGamesButton : MonoBehaviour\r\n{\r\n void Awake()\r\n {\r\n  MoreGamesPanelController.Instance.showPanelEvent += MoreGamesShow;\r\n  MoreGamesPanelController.Instance.hidePanelEvent += MoreGamesHide;\r\n }\r\n\r\n private void MoreGamesShow()\r\n {\r\n  SoundManager.muteMusic(true);\r\n  SoundManager.muteSFX(true);\r\n }\r\n\r\n private void MoreGamesHide()\r\n {\r\n  SoundManager.muteMusic(false);\r\n  SoundManager.muteSFX(false);\r\n }\r\n\r\n void OnPress(bool isDown)\r\n {\r\n  if (isDown)\r\n  {\r\n   Social.Instance.IsConnected((bool connected) =>\r\n   {\r\n    if (connected)\r\n    {\r\n   MoreGamesPanelController.Instance.Show();\r\n    }\r\n    else\r\n    {\r\n    }\r\n   });\r\n  }\r\n }\r\n}");
		}
	}
}

#endif