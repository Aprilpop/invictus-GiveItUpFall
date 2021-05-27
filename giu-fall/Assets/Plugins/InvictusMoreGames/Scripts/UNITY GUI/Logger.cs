using UnityEngine;

namespace InvictusMoreGames
{
	public class Logger : MonoBehaviour
	{
		private void Awake()
		{
			MoreGamesSettings.LoadSettings();
		}

		public static void Log(object msg)
		{
			if (MoreGamesSettings.logMode)
				Debug.Log("More Games LOG: " + msg);
		}

		public static void LogWarning(object msg)
		{
			if (MoreGamesSettings.logMode)
				Debug.LogWarning("More Games LOG: " + msg);
		}

		public static void LogError(object msg)
		{
			if (MoreGamesSettings.logMode)
				Debug.LogError("More Games LOG: " + msg);
		}

		public static void LogException(System.Exception exception)
		{
			if (MoreGamesSettings.logMode)
				Debug.LogError("More Games LOG: " + exception.Message);
		}
	}
}