using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

[System.Serializable]
public enum eFade { none, fade}

public class MusicManager : MonoBehaviour
{
#if UNITY_EDITOR
	[SerializeField]
	MusicArrayEnum musicArray;

	private void OnValidate()
	{
		if (last != musicArray)
			PlayMusic(musicArray);
	}

#endif

	public bool debug = false;
	public Text clipName;
	public GameObject canvasToHide;

	[SerializeField]
	AudioMixer mainMixer;
	const string MusicVol = "MusicVol";
	float origSoundVol;
	const string SoundVol = "SoundVol";
	float origMusicVol;
	const string UIVol = "UI_Vol";
	float origUiVol;
	const string HeliCopterVol = "HeliCopter_Vol";
	float origHeliCopterVol;

	[SerializeField]
	bool OnlyIfNotPlaying = false;
	[SerializeField]
    MusicArrayEnum StarUP = MusicArrayEnum.First;

	[Header("Heli")]
	[SerializeField]
	float fadeTimeHeli = 1;
	[SerializeField]
	float loudnessRaise = 5;
	bool heliLoud = false;
	float heliCurVol;

	public void SetHeliLoud(bool on)
	{
		mainMixer.GetFloat(HeliCopterVol, out heliCurVol);

		heliLoud = on;
	}

	[Space(5)]

	static MusicManager instance;
	public static MusicManager Instance
	{
		get
		{
			return instance;
		}
		set { MusicManager.Instance = value; }
	}

	//public enum MusicType {menu, intro, ingame, unknown}

	//MusicType musicType = MusicType.unknown;

	public float fadeTime = 2;
	bool fading = false;
	IEnumerator currentRoutine = null;

	[System.Serializable]
	public class Music
	{
		[Range(0,1)]
		public float volume = 1.0f;
        public string clipName;
		//public AudioClip clip;
	}
	[System.Serializable]
	public class MusicGroup
	{
		public string name;
		public Music[] musics;
	}
	[SerializeField]
	bool randomNext = true;

	public MusicGroup[] musicGroups;
	MusicGroup currentG;
	public MusicArrayEnum last;

	//public List<Music> menuMusic = new List<Music>();
	//public List<Music> introMusic = new List<Music>();
	//public List<Music> ingameMusic = new List<Music>();

	//List<Music> tempL = new List<Music>();
	Dictionary<string, MusicGroup> dict;
	Music current;

	public AudioSource musicSource;
	public AudioSource musicSource2;
	public AudioSource curSource;

	bool firstRun = true;

	public bool IsPlaying {  get { return musicSource.isPlaying || musicSource2.isPlaying || curSource.isPlaying; } }

	//string st = "";
	//string stu = "";

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
			//if(canvasToHide != null)
			//	ShowCanvas();
        }
		else
		{
			//instance.PlayMusic(StarUP);
			DestroyImmediate(gameObject);
			return;
		}
		CreateDictionary();
		if(musicSource2 == null)
		{
			musicSource2 = (AudioSource)GameObject.Instantiate(musicSource, transform) as AudioSource;
			curSource = musicSource;
		}
		//st +=  "Awake" + "\n";
	}

	void CreateDictionary()
	{
		dict = new Dictionary<string, MusicGroup>();
		for (int i = 0; i < musicGroups.Length; i++)
		{
			dict.Add(musicGroups[i].name, musicGroups[i]);
		}
		//st += "CreateDictionary" + "\n";
		//foreach (var item in dict)
		//{
		//	Debug.Log(item.Key);
		//}
	}

	private void Start()
	{
		//st += "Start" + "\n";
		heliLoud = false;
		mainMixer.GetFloat(MusicVol, out origMusicVol);
		mainMixer.GetFloat(SoundVol, out origSoundVol);
		mainMixer.GetFloat(UIVol, out origUiVol);
		mainMixer.GetFloat(HeliCopterVol, out origHeliCopterVol);

		SetMusicVol(ProfileManager.Instance.Music);
		SetSoundVol(ProfileManager.Instance.Sound);
		//if(!OnlyIfNotPlaying || (OnlyIfNotPlaying && !IsPlaying))
		PlayMusic(ProfileManager.Instance.currentMusic);
		firstRun = false;
		//enebleCounter++;
		
	}

	public void SetMusicVol(bool enabled)
	{
		mainMixer.SetFloat(MusicVol, enabled ? origMusicVol : -80.0f);
	}

	public void SetSoundVol(bool enabled)
	{
		mainMixer.SetFloat(SoundVol, enabled ? origSoundVol : -80.0f);
		//bool uiVol = true;
		//if (!ProfileManager.Instance.Sound)
		//	uiVol = false;
		//mainMixer.SetFloat(UIVol, uiVol ? origUiVol : -80.0f);
	}

	//public void PlayMusic( MusicType type)
	//{
	//	if (type == MusicType.unknown)
	//		return;
	//	Debug.Log("Music " + type.ToString());
	//	int currentMusic = NowPlays();
		
	//	if (type != musicType)
	//	{
	//		musicType = type;
	//		currentMusic = -1;
	//	}
	//	NextMusic();

	//}

	public void PlayMusic(MusicArrayEnum type)
	{
		
		if (type == MusicArrayEnum.unknown ||(type == last && IsPlaying))
			return;

		//st += "PlayMusic: " + type.ToString() + "\n";

		Debug.Log("Music " + type.ToString());
		last = type;
		int currentMusic = NowPlays();
		MusicGroup group = GetCurrent(type);
		if (currentG != group)
		{
			currentG = group;
			currentMusic = -1;
		}
		NextMusic();

	}

	MusicGroup GetCurrent(MusicArrayEnum type)
	{
		string sT = Enum.GetName(typeof(MusicArrayEnum), type);
		return dict[sT];
	}

	public void NextMusic()
	{
		int currentMusic = NowPlays();
		if (currentG.musics.Length > 1)
		{
			if (!randomNext)
			{
				if (currentMusic + 1 < currentG.musics.Length)
					current = currentG.musics[currentMusic + 1];
				else
					current = currentG.musics[0];
			}
			else
			{
				int temp = currentMusic;
				while (temp == currentMusic)
				{
					temp = UnityEngine. Random.Range(0, currentG.musics.Length);
				}
				current = currentG.musics[temp];
			}
		}
		else
			current = currentG.musics[0];
		//st += "NextMusic: " + current.clip.name + "\n";
		Play();
	}

	[SerializeField]
	eFade fade;

	void Play()
	{
		if (curSource.isPlaying )
		{
			//if(currentRoutine != null)
			//	StopCoroutine(currentRoutine);
			//currentRoutine = Fade();
			//StartCoroutine(currentRoutine);

			from = curSource;
			fromVol = from.volume;
			_fadeTime = fadeTime;
			to = (curSource != musicSource) ? musicSource : musicSource2;
			toVol = current.volume;
            to.clip = Resources.Load<AudioClip>(current.clipName);
			//to.clip = current.clip;
			to.volume = 0;
			to.Play();
			fade = eFade.fade;

			//st += "Fade Start: " + "musicSource is " + (musicSource.isPlaying ? "" : "NOT") + " playing! " + "\n" + "musicSource2 is " + (musicSource2.isPlaying ? "" : "NOT") + " playing!" + "\n";

		}

		else
		{
			curSource = musicSource;
            curSource.clip = Resources.Load<AudioClip>(current.clipName);
            //curSource.clip = current.clip;
            curSource.volume = current.volume;
			curSource.Play();

			//st += "Simple Start: " + "musicSource is " + (musicSource.isPlaying ? "" : "NOT") + " playing! " + "\n" + "musicSource2 is " + (musicSource2.isPlaying ? "" : "NOT") + " playing!" + "\n";

			//if (debug)
			//	clipName.text = musicSource.clip.name.ToString();
		}

	}


	bool ClipEnded;

	void Update()
	{
		ClipEnded = curSource.time >= curSource.clip.length && fade != eFade.fade;
		if (StarUP != MusicArrayEnum.unknown && musicSource != null && musicSource2 != null && curSource != null && !IsPlaying && fade != eFade.fade && ClipEnded)
		{
			//st += "Fade: " + fade.ToString() + "\n";
			NextMusic();
		}
		if (heliLoud && heliCurVol < (origHeliCopterVol + loudnessRaise))
		{
			mainMixer.GetFloat(HeliCopterVol, out heliCurVol);
			mainMixer.SetFloat(HeliCopterVol, heliCurVol + Time.deltaTime * loudnessRaise / fadeTimeHeli);

		}

		if(fade == eFade.fade)
		{
			if (_fadeTime > 0)
			{
				_fadeTime -= Time.deltaTime;
				float adj = 1 - (_fadeTime / fadeTime);
				from.volume = Mathf.Lerp(fromVol, 0, adj);
				to.volume = Mathf.Lerp(0, toVol, adj);
				
			}
			else
			{
				from.volume = 0;
				from.Stop();
				curSource = to;
				fade = eFade.none;
			}
		}
		//stu = fade.ToString() + " " + _fadeTime + " TimeScale: " + Time.timeScale + "\n";
		//stu += curSource.time + " / " + curSource.clip.length + " " + ClipEnded +  "\n";
		//stu += "musicSource is " + (musicSource.isPlaying ? "" : "NOT") + " playing! " + "\n" + "musicSource2 is " + (musicSource2.isPlaying ? "" : "NOT") + " playing!" + "\n" + "\n";
	}

	AudioSource from;
	AudioSource to;
	float fromVol;
	float _fadeTime;
	float toVol;

	//IEnumerator Fade()
	//{
	//	from = curSource;
	//	float fromVol = from.volume;
	//	float _fadeTime = fadeTime;
	//	to = (curSource != musicSource) ? musicSource : musicSource2;
	//	float toVol = current.volume;
	//	fading = true;
	//	to.clip = current.clip;
	//	to.volume = 0;
	//	to.Play();
	//	while (_fadeTime > 0)
	//	{
	//		_fadeTime -= Time.deltaTime;
	//		float adj =1-( _fadeTime / fadeTime);
	//		from.volume = Mathf.Lerp(fromVol, 0, adj);
	//		to.volume = Mathf.Lerp( 0, toVol, adj);
	//		yield return null;
	//	}
	//	from.volume = 0;
	//	from.Stop();

	//	fading = false;
	//	curSource = to;
	//	currentRoutine = null;
	//}

	//IEnumerator Fade()
	//{
	//	AudioSource from = (musicSource.isPlaying) ? musicSource : musicSource2;
	//	float vol = from.volume / fadeTime;
	//	AudioSource to = (!musicSource.isPlaying) ? musicSource : musicSource2;
	//	fading = true;
	//	while (musicSource.volume > 0)
	//	{
	//		from.volume -= Time.deltaTime / fadeTime;
	//		yield return null;
	//	}
	//	musicSource.volume = 0;
	//	musicSource.clip = current.clip;
	//	musicSource.Play();
	//	if (debug)
	//		clipName.text = musicSource.clip.name.ToString();
	//	while (musicSource.volume < current.volume)
	//	{
	//		musicSource.volume += Time.deltaTime / fadeTime;
	//		yield return null;
	//	}
	//	musicSource.volume = current.volume;
	//	fading = false;
	//	currentRoutine = null;
	//}

	int NowPlays()
	{
		
		int curr = -1;

		//curr = CurrList().FindIndex(d => d == current);
		if(currentG != null && currentG.musics != null)
		curr = System.Array.IndexOf(currentG.musics, current);
		return curr;
		
	}

	//List<Music> CurrList()
	//{
	//	//if (musicType == MusicType.menu)
	//	//	return menuMusic;
	//	//else if (musicType == MusicType.intro)
	//	//	return introMusic;
	//	//else
	//	//	return ingameMusic;
		
	//}
	


	//public void ShowCanvas(bool show = false)
	//{
	//	debug = show;
	//	if (!debug)
	//		canvasToHide.SetActive(false);
	//	else
	//	{
	//		canvasToHide.SetActive(true);
	//		clipName.text = musicSource.clip.name.ToString();
	//	}
			
	//}

	void OnLevelFinishedLoad(Scene scene, LoadSceneMode mode)
	{
		if(!firstRun)
			SetSoundVol(ProfileManager.Instance.Sound);
	}

	public void AdjustSoundVol(float _adj)
	{
		float fix = ProfileManager.Instance.Sound ? Mathf.Lerp(-80.0f, origSoundVol, _adj) : -80.0f;
		mainMixer.SetFloat(SoundVol, fix);
	}

	//	int enebleCounter;
	//private void OnGUI()
	//{
	//	if (debug)
	//	{
	//		GUIStyle myButtonStyle = new GUIStyle(GUI.skin.button);
	//		myButtonStyle.fontSize = 25;

	//		//st += "\n" + "musicSource: " + ((musicSource.isPlaying) ? " Playing" : "NOT Playing");
	//		//st += "\n" + "musicSource2: " + ((musicSource2.isPlaying) ? " Playing" : "NOT Playing");



	//		GUI.Button(new Rect(30, 30, 500, Screen.height - 60), stu + st, myButtonStyle);
	//	}
	//}
}
