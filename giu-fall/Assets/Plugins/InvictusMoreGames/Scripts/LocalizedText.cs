using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace InvictusMoreGames
{
	[RequireComponent(typeof(Text))]
	public class LocalizedText : MonoBehaviour
	{
		public List<LanguageElement> languageElements;

		private Dictionary<SystemLanguage, LanguageElement> languageElementCache = null;
		private LanguageElement defaultLanguage = new LanguageElement();
		private Text text;

		public void Init(List<LanguageElement> languageElements = null)
		{
			if (text == null)
				text = GetComponent<Text>();

			if (languageElements != null)
				this.languageElements = languageElements;

			languageElementCache = new Dictionary<SystemLanguage, LanguageElement>();

			foreach (var languageElement in this.languageElements)
			{
				languageElementCache.Add(languageElement.languageId, languageElement);

				if (languageElement.languageId == SystemLanguage.English)
					defaultLanguage = languageElement;
			}

			SetTextWithLanguge();
		}

		public LanguageElement GetLanguageElement(SystemLanguage lang)
		{
			return languageElementCache[lang];
		}

		private void Awake()
		{
			Init();
		}

		private void OnEnable()
		{
			SetTextWithLanguge();
		}

		private void SetTextWithLanguge()
		{
			foreach (var languageElementKey in languageElementCache.Keys)
				if (GetSystemLanguage() == languageElementKey)
				{
					text.text = languageElementCache[languageElementKey].value;
					return;
				}

			text.text = defaultLanguage.value;
		}

		private SystemLanguage GetSystemLanguage()
		{
			if (MoreGamesBoxController.Instance.moreGamesLanguage == null)
				return Application.systemLanguage;
			else
				return (SystemLanguage)MoreGamesBoxController.Instance.moreGamesLanguage;
		}
	}

	[System.Serializable]
	public struct LanguageElement
	{
		public SystemLanguage languageId;
		public string value;
	}
}