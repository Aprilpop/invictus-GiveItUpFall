using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using System;

namespace InvictusMoreGames
{
	public class MoreGamesBoxController : MonoBehaviour
	{
		/**Analítikához, a string paraméter a játék neve.*/
		public Action<string> onClicked = delegate { };
		public Action<bool> onJsonReadSuccess = delegate { };

		[HideInInspector]
		public SystemLanguage? moreGamesLanguage = null;
		public EventSystem eventSystemPrefab;
		public RectTransform rootPanel;
		public MoreGamesDataDownloader dataDownloader;
		public MoreGamesServerData serverData;
		public GameBox gameBox;

		private EventSystem eventSystem;
		private int gameIndex = 0;
		private bool jsonReadSuccess = false;
		private int jsonDownloadTrialCount = 0;

		private static bool applicationQuit = false;

		private static MoreGamesBoxController instance;
		public static MoreGamesBoxController Instance
		{
			get
			{
				if (instance == null)
					instance = FindObjectOfType<MoreGamesBoxController>();

				return instance;
			}
		}

		public void SetLanguage(SystemLanguage language)
		{
			moreGamesLanguage = language;
		}

		//TODO:Erre figyelni kell
		public bool JsonReadSuccess { get { return jsonReadSuccess; } }
		public bool IsActive { get { return rootPanel.gameObject.activeInHierarchy; } }

		//TODO:Ezt kell meghívni, hogy új játék töltődjön be
		public virtual void ShowNewGame()
		{
			onJsonReadSuccess -= NextGame;

			if (!jsonReadSuccess)
			{
				onJsonReadSuccess += NextGame;
				StartDataDownloader();
			}
			else
				NextGame(true);
		}

		public void Show()
		{
			rootPanel.gameObject.SetActive(true);
		}

		public void Hide()
		{
			rootPanel.gameObject.SetActive(false);
		}

		public void StartDataDownloader()
		{
			dataDownloader.DownloadJSONDataInCrosspromoServer(OnDownloadedJSONData);
		}

		protected virtual void Initialize()
		{
			MoreGamesSettings.LoadSettings();

			if (FindObjectOfType<EventSystem>() == null)
			{
				eventSystem = GameObject.Instantiate(eventSystemPrefab) as EventSystem;
				eventSystem.gameObject.transform.SetParent(rootPanel.transform);
			}

			jsonDownloadTrialCount = 0;
			jsonReadSuccess = false;
			gameIndex = SaveManager.Load(SaveKey.AppearsAppIconStyle_GameIndex, 0);

			if (MoreGamesSettings.automaticPreloadData)
				StartDataDownloader();
		}

		protected void NextGame(bool dataDownloaded)
		{
			if (!dataDownloaded)
				return;

			if (gameIndex >= serverData.gameBoxDataList.Count)
				gameIndex = 0;

			gameBox.Initialize(serverData.gameBoxDataList[gameIndex++]);
			SaveManager.Save(SaveKey.AppearsAppIconStyle_GameIndex, gameIndex);
		}

		protected virtual void Awake()
		{
			instance = this;
			Initialize();
		}

		protected virtual void OnEnable()
		{
		}

		protected virtual void Start()
		{
		}

		protected virtual void Update()
		{
		}

		protected static T CreateMyInstance<T>(string prefabName) where T : MonoBehaviour
		{
			if (!applicationQuit)
			{
				T panel = Resources.Load(prefabName, typeof(T)) as T;
				T moreGamesPanel = GameObject.Instantiate(panel) as T;
				moreGamesPanel.name = moreGamesPanel.name.Split(new char[] { '(' })[0];
				DontDestroyOnLoad(moreGamesPanel);
				return moreGamesPanel;
			}
			else
				return null;
		}

		private void OnDownloadedJSONData(bool success, string text)
		{
			if (jsonReadSuccess)
				return;

			if (success)
			{
				int actualID = 0;
				var json = JSON.Parse(text);

				Logger.Log("JSON data:\n" + text);

				var games = json["games"];
				serverData.gameBoxDataList = new List<GameBox.GameBoxData>();
				foreach (JSONNode jsonObject in games.AsArray)
					serverData.gameBoxDataList.Add(GameBox.GameBoxData.GetDataWithJSON(actualID++, jsonObject));

				onJsonReadSuccess(jsonReadSuccess = serverData.gameBoxDataList.Count > 0);
			}
			else if (jsonDownloadTrialCount++ < 4)
				StartDataDownloader();
		}

		private void OnApplicationQuit()
		{
			applicationQuit = true;
		}
	}

	public struct MoreGamesServerData
	{
		public List<GameBox.GameBoxData> gameBoxDataList;
	}
}