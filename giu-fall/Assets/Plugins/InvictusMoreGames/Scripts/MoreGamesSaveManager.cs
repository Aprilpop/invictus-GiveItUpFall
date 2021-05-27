using UnityEngine;

namespace InvictusMoreGames
{
	public static class SaveManager
	{
		public static void Save(SaveKey key, string value, bool saveToDisk = true)
		{
			PlayerPrefs.SetString(key.ToString(), value);

			if (saveToDisk)
				PlayerPrefs.Save();
		}

		public static void Save(SaveKey key, int value, bool saveToDisk = true)
		{
			PlayerPrefs.SetInt(key.ToString(), value);

			if (saveToDisk)
				PlayerPrefs.Save();
		}

		public static void Save(SaveKey key, float value, bool saveToDisk = true)
		{
			PlayerPrefs.SetFloat(key.ToString(), value);

			if (saveToDisk)
				PlayerPrefs.Save();
		}

		public static void Delete(SaveKey key, bool saveToDisk = true)
		{
			PlayerPrefs.DeleteKey(key.ToString());

			if (saveToDisk)
				PlayerPrefs.Save();
		}

		public static string Load(SaveKey key, string defaultValue)
		{
			return PlayerPrefs.GetString(key.ToString(), defaultValue);
		}

		public static float Load(SaveKey key, float defaultValue)
		{
			return PlayerPrefs.GetFloat(key.ToString(), defaultValue);
		}

		public static int Load(SaveKey key, int defaultValue)
		{
			return PlayerPrefs.GetInt(key.ToString(), defaultValue);
		}
	}

	public enum SaveKey
	{
		None,
		AppearsAppIconStyle_GameIndex
	}
}