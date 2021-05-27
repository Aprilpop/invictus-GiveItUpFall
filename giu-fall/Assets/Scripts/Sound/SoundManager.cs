using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;
using UnityEngine.SceneManagement;

[System.Serializable]
public enum SoundType { simple,tremolo }

[System.Serializable]
public enum GapType {none, simple, waitTillEnd }

[System.Serializable]
public enum RadioState { none, hissIn, speak, hissOut }

[System.Serializable]
public class SoundMain
{
	
	public float _volume = 1.0f;
	public float _pitch = 1.0f;

	public AudioClip[] clips;
	
}



[System.Serializable]
public class SfxContainer
{
	public string name;
	public int enumId = -1;
	public SoundType soundType = SoundType.simple;
	[SerializeField]
	int maxCount = -1;
	public int MaxCount
		{ get { return maxCount; } }
	public int currentCount;
	public bool waitEnd = false;
	public bool killOnEnd = false;
	public float MaxDistance = -1;

	[Range(0.0f, 1.0f)]
	public float volume = 1.0f;
	public bool SFX2D = false;
	public bool byPssReverbZones = false;

	public bool loop = false;

	public bool randomize = false;
	public Vector2 RandomPitch = new Vector2(0.8f, 1.1f);
	public float randomPitch { get { return UnityEngine.Random.Range(RandomPitch.x, RandomPitch.y); } }
	public Vector2 RandomVolume = new Vector2(0.8f, 1.1f);
	public float randomVolume { get { return UnityEngine.Random.Range(RandomVolume.x, RandomVolume.y); } }

	public GapType useTimeGap;
	public Vector2 GapTime = new Vector2(2f, 3f);
	public Vector2 RandomTime = new Vector2(2f, 3f);
	public float randomTime { get { return UnityEngine.Random.Range(RandomTime.x, RandomTime.y); } }
	public SoundMain[] sounds;
	public AudioMixerGroup mixerGroup;
}

public class SoundManager : MonoBehaviour {
	static SoundManager instance;
	public static SoundManager Instance
	{
		get
		{
            if (instance == null)
                Instantiate(Resources.Load("SoundManager"));
            return instance;
		}
		set { SoundManager.Instance = value; }
	}
	[SerializeField]
	public GameObject SoundObjectPrefab;

	public AudioMixer Mixer;

	public float DistanceTimeOut = 1;

	public SfxContainer[] sfxArray;
	Dictionary<string, SfxContainer> sounds = new Dictionary<string, SfxContainer>();

	List<SoundObject> used = new List<SoundObject>();
	List<SoundObject> unUsed = new List<SoundObject>();

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
			InitDictionary();
		}
		else
		{
			DestroyImmediate(gameObject);
			return;
		}

	}

	public static SoundObject Play(SfxArrayEnum value, Vector3 pos , Transform _parent = null)
	{
		string sT = Enum.GetName(typeof(SfxArrayEnum), value);
		if (sT == "")
		{
			Debug.LogError("No Such Sound: " + value.ToString());
			return null;
		}
		sT = sT.Replace("_", " ");

		SoundObject obj = null;

		obj= Instance. _Play(sT,pos,_parent);
		//Debug.Log(value + " " + _parent.name, _parent);

		return obj;
	}

	public static SoundObject Play(SfxArrayEnum value, Transform _parent = null)
	{
		string sT = Enum.GetName(typeof(SfxArrayEnum), value);
		if (sT == "")
		{
			Debug.LogError("No Such Sound: " + value.ToString());
			return null;
		}
		sT = sT.Replace("_", " ");
		Vector3 pos = Vector3.zero;
		SoundObject obj = null;
		obj = Instance._Play(sT, pos, _parent);
		//Debug.Log(value + " " + _parent.name , _parent);

		return obj;

	}

	public SfxContainer GetSFX(SfxArrayEnum value)
	{
		string sT = Enum.GetName(typeof(SfxArrayEnum), value);
		if (sT == "")
		{
			Debug.LogError("No Such Sound: " + value.ToString());
			return null;
		}
		sT = sT.Replace("_", " ");
		SfxContainer a = null;
		sounds.TryGetValue(sT, out a);
		return a;
	}

	void InitDictionary()
	{
		if (sfxArray != null)
			foreach (var element in sfxArray)
			{
				if (element != null)
				{
					string name = element.name;
					if (!sounds.ContainsKey(name))
						sounds.Add(name, element);
				}
			}
	}
	SfxContainer LookupClip(string _name)
	{
		SfxContainer a;
		sounds.TryGetValue(_name, out a);
		return a;
	}

	public SoundObject _Play(string clipName,Vector3 pos, Transform _parent = null)
	{
		SfxContainer container = LookupClip(clipName);



		if (container != null)
		{
			if(container.MaxCount > 0)
			{
				if (container.currentCount >= container.MaxCount)
					return null;
				else
					container.currentCount++;
			}


			SoundMain[] ac = container.sounds;

			if (ac.Length > 0)
			{
				SoundObject SoundObject = GetSoundObject(clipName);


				used.Add(SoundObject);


				if(!container.SFX2D)
				{
					SoundObject.transform.position = pos;
				}
				if (!container.SFX2D && _parent != null)
				{
					SoundObject.transform.SetParent(_parent);
				}
				SoundObject.enabled = true;

				SoundObject.reset();
				SoundObject._play(container);

				return SoundObject;

			}
			return null;

		}
		else
		{
			Debug.LogWarning("SoundManagerConainer NULL!!");

			return null;
		}
	}

	SoundObject GetSoundObject(string clipName = "")
	{
		GameObject go = null;
		SoundObject SoundObject = null;

		if (unUsed.Count <= 0)
		{
			if (SoundObjectPrefab == null)
			{
				go = new GameObject("Audio: " + clipName);
				go.transform.SetParent(this.transform);
				SoundObject = go.AddComponent<SoundObject>();
				SoundObject.source = go.AddComponent<AudioSource>();
			}
			else
			{
				go = Instantiate(SoundObjectPrefab);
				go.transform.SetParent(this.transform);
				SoundObject = go.GetComponent<SoundObject>();
				SoundObject.source = go.GetComponent<AudioSource>();
			}

		}
		else
		{
			SoundObject = unUsed[unUsed.Count - 1];
			unUsed.Remove(SoundObject);
			go = SoundObject.source.gameObject;
		}

		return SoundObject;
	}

	public static void UnuseSFX(SoundObject source)
	{

		Instance.used.Remove(source);
		Instance.unUsed.Add(source);
		source.reset();
		source.enabled = false;
		source.transform.SetParent(Instance.transform);
		if (source.container.MaxCount > 0 && source.container.currentCount>0)
			source.container.currentCount--;
	}

	private void OnEnable()
	{
		SceneManager.sceneLoaded += OnLevelFinishedLoad;
	}

	private void OnDisable()
	{
		SceneManager.sceneLoaded -= OnLevelFinishedLoad;
	}

	void OnLevelFinishedLoad(Scene scene, LoadSceneMode mode)
	{
		//Debug.Log(scene.name);
	}

	void getMIxers()
	{
		
	} 

	private void Update()
	{
		for (int i = 0; i < used.Count; i++)
		{
			used[i].ObjUpdate();
		}
	}

}
