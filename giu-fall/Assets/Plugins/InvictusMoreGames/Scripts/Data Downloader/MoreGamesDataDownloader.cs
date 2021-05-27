using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.IO;

namespace InvictusMoreGames
{
	public class MoreGamesDataDownloader : MonoBehaviour
	{
		private Dictionary<string, Texture2D> textureChache = new Dictionary<string, Texture2D>();

		public string GetActualPlatform()
		{
			string platform = "ios";

#if UNITY_IPHONE
			platform = "ios";
#elif UNITY_ANDROID
			platform = "android";
#elif UNITY_IOS
			platform = "mac";
#elif UNITY_WP8
			platform = "wp8";
#elif UNITY_STANDALONE
			platform = "pc";
#endif
			return platform;
		}

		public string GetSettingsJsonFileName()
		{
			return GetActualPlatform() + "_" + MoreGamesSettings.gameName;
		}

		public void DownloadJSONDataInCrosspromoServer(System.Action<bool, string> onDownloadResult)
		{
			if (MoreGamesSettings.debugMode)
			{
				TextAsset asset = Resources.Load<TextAsset>("sample_json");
				if (asset != null)
					onDownloadResult(true, asset.text);
				else
					onDownloadResult(false, null);
			}
			else
				StartCoroutine(WWWStart(Path.Combine(MoreGamesSettings.promoUrl, GetSettingsJsonFileName()), onDownloadResult));
		}

		public void DownloadImageAsTexture(string url, System.Action<bool, Texture2D> result)
		{
			StartCoroutine(DownloadImageAsTextureCorutine(url, result));
		}

		public Texture2D ConvertBase64ToTexture(string imageData, RectTransform gameImageRect)
		{
			try
			{
				byte[] image = System.Convert.FromBase64String(imageData);
				Texture2D texture = null;
				if (gameImageRect)
					texture = new Texture2D((int)gameImageRect.GetWidth(), (int)gameImageRect.GetHeight(), TextureFormat.RGB24, false);
				else
					texture = new Texture2D(1, 1);

				texture.LoadImage(image);

				return texture;
			}
			catch
			{
				return null;
			}
		}

		private IEnumerator DownloadImageAsTextureCorutine(string url, System.Action<bool, Texture2D> result)
		{
			if (string.IsNullOrEmpty(url))
				yield break;

			if (textureChache.ContainsKey(url))
				result(true, textureChache[url]);
			else
			{
				WWW www = new WWW(url);
				bool success = true;

				while (!www.isDone)
					yield return null;

				if (textureChache.ContainsKey(url))
					result(success, textureChache[url]);
				else
				{
					Texture2D texture = null;
					try
					{
						if (www != null && www.error == null)
							texture = www.texture;
					}
					catch
					{
						success = false;
					}

					textureChache.Add(url, texture);
					result(success, texture);
				}
			}
		}

		private IEnumerator WWWStart(string url, System.Action<bool, string> onDownloadResult)
		{
			WWW www = new WWW(url);
			yield return www;

			if (www.error == null)
				onDownloadResult(true, www.text);
			else
				onDownloadResult(false, www.error);
		}
	}
}