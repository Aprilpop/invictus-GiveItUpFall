using System.Collections.Generic;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine;

namespace InvictusMoreGames
{
	public class GameBox : MonoBehaviour
	{
		public RawImage gameImage;
		public LocalizedText gameName;
		public LocalizedText gameDescription;
		public Button installButton;
		public VideoPlayer videoPlayer;
		public RenderTextureSettings renderTextureSettings;

		private static RenderTexture renderTexture = null;

		[HideInInspector]
		public GameBoxData data;

        const float topOffset = 40;

        public static bool IsiPhoneX
        {
            get
            {
                int width = Screen.width;
                int height = Screen.height;
                float aspect = (height > width) ? (float)height / width : (float)width / height;
                return (aspect > 2);
            }
        }

        public void Initialize(GameBoxData data)
		{
			this.data = data;

            if (IsiPhoneX)
            {
                RectTransform rect = GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, -200);

            }


            if (data.localVideoId != null)
			{
				VideoClip videoClip = Resources.Load<VideoClip>("Game Video/" + data.localVideoId);

				if (videoPlayer.clip == null || videoClip == null)
				{
					videoPlayer.Stop();
					videoPlayer.clip = null;
				}

				if (videoClip != null)
				{
					renderTexture = new RenderTexture((int)videoClip.width, (int)videoClip.height, renderTextureSettings.depth, renderTextureSettings.format, renderTextureSettings.readWrite);
					renderTexture.dimension = UnityEngine.Rendering.TextureDimension.Tex2D;
					videoPlayer.clip = videoClip;
					videoPlayer.targetTexture = renderTexture;
					gameImage.texture = renderTexture;
				}
			}

			if (videoPlayer.clip != null)
				videoPlayer.Play();

			//Video betöltése sikertelen, ezért letöltök a játék képét
			if (data.localVideoId == null || videoPlayer.clip == null)
				DowloadGameBoxTexture();

			if (gameName != null)
				gameName.Init(data.shortNameLanguageElements);

			if (gameDescription != null)
				gameDescription.Init(data.shortDescriptionLanguageElements);

			if (data.downloadLink != null && installButton != null)
			{
				installButton.gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
				installButton.gameObject.GetComponent<Button>().onClick.AddListener(
					() => Application.OpenURL(data.downloadLink)
				);
				installButton.gameObject.GetComponent<Button>().onClick.AddListener(
					() => MoreGamesBoxController.Instance.onClicked(gameName.GetLanguageElement(SystemLanguage.English).value)
				);

                
            }
		}

		public void DowloadGameBoxTexture()
		{
			Texture2D base64Texture = null;

			if (data.imageData != null)
				base64Texture = MoreGamesBoxController.Instance.dataDownloader.ConvertBase64ToTexture(data.imageData, gameImage.GetComponent<RectTransform>());

			if (base64Texture != null)
				DownloadedGameBoxTexture(this, base64Texture);
			else
				MoreGamesBoxController.Instance.dataDownloader.DownloadImageAsTexture(data.imageLink, (bool success, Texture2D texture) =>
				{
					if (success)
						gameImage.texture = texture;
					//TODO: Sikertelenséget lekezelni!
				});
		}

		private void DownloadedGameBoxTexture(GameBox gameBox, Texture2D base64Texture)
		{
			AspectRatioFitter ratio = gameBox.gameImage.GetComponent<AspectRatioFitter>();

			if (base64Texture != null)
			{
				if (ratio != null)
					ratio.aspectRatio = base64Texture.width / base64Texture.height;

				gameBox.gameImage.texture = base64Texture;
			}
		}

		[System.Serializable]
		public struct GameBoxData
		{
			public int ID;
			public string localVideoId;
			public string imageLink;
			public string imageData;
			public string downloadLink;
			public List<LanguageElement> shortNameLanguageElements;
			public List<LanguageElement> shortDescriptionLanguageElements;

			public static GameBoxData GetDataWithJSON(int ID, JSONNode jsonObject)
			{
				GameBoxData gameBoxData = new GameBoxData();
				gameBoxData.ID = ID;
				gameBoxData.localVideoId = jsonObject["local_video_id"];
				gameBoxData.downloadLink = jsonObject["download-link"];
				gameBoxData.imageLink = jsonObject["image-link"];
				gameBoxData.imageData = jsonObject["image-data"];

				SetLanguageElements(jsonObject, "localized_short_name", "short_name", "name", ref gameBoxData.shortNameLanguageElements);
				SetLanguageElements(jsonObject, "localized_short_description", "short_description", "description", ref gameBoxData.shortDescriptionLanguageElements);

#if UNITY_ANDROID
				if (MoreGamesSettings.trackingFunction && gameBoxData.downloadLink != null)
				{
					gameBoxData.downloadLink += gameBoxData.downloadLink.Contains("?") ? "&" : "?";

					if (gameBoxData.downloadLink.Contains("id="))
						gameBoxData.downloadLink += "referrer=utm_source%3Dinvictus_more_games";
					else
						gameBoxData.downloadLink += "id=com.invictus.moregames&referrer=utm_source%3Dinvictus_more_games";
				}
#endif

				return gameBoxData;
			}

			private static void SetLanguageElements(JSONNode jsonObject, string id, string short_engId, string engId, ref List<LanguageElement> languageElements)
			{
				languageElements = new List<LanguageElement>();

				if (jsonObject[id].AsArray.Count > 0)
					foreach (JSONNode localized_name in jsonObject[id].AsArray)
						languageElements.Add(new LanguageElement()
						{
							languageId = (SystemLanguage)System.Enum.Parse(typeof(SystemLanguage), localized_name["SystemLanguage"]),
							value = localized_name["value"]
						});
				else
				{
					languageElements.Add(new LanguageElement()
					{
						languageId = SystemLanguage.English,
						value = jsonObject[short_engId] != null ? jsonObject[short_engId] : jsonObject[engId]
					});
				}
			}
		}

		[System.Serializable]
		public class RenderTextureSettings
		{
			public int depth = 2;
			public RenderTextureFormat format = RenderTextureFormat.ARGB32;
			public RenderTextureReadWrite readWrite = RenderTextureReadWrite.Default;
		}
	}
}