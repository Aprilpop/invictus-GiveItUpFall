using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SoundObject : MonoBehaviour
{
	
	public SfxContainer container;
	public float MaxDistance = 50;
	public AudioSource source;
	bool started = false;
	public bool IsPLaying { get { return started; } }
	public bool SourceIsPLaying { get { return source.isPlaying; } }
	SoundMain sound;

	[SerializeField]
	bool returnToManager = false;

	float _pitch;
	float _pitchOrig;
	float _vol { get { return sound._volume * container.volume; } }
	float _volOrig;
	float _volTarget;
	float _time;
	float _timeOrig;
	float _timaGap;

	float distance;
	float distT;

	bool waitEnd = false;
	[SerializeField]
	bool PanWithScreen = false;
	[SerializeField]
	AnimationCurve panCurve;
	float Pan;


    private void Awake()
    {
        source.spatialize = false;
    }

    public void _play(SfxContainer _sfxContainer = null)
	{
		if(_sfxContainer != null)
			this.container = _sfxContainer;
		if (container == null )
			return;
		SetSoundObject();
		if (source.clip != null)
		{
			source.Play();
			started = true;

		}

	}

	

	public void reset()
	{
		//if (container.waitEnd)
		//	waitEnd = true;
		//else
		//{
			source.Stop();
			started = false;
	
		//}

	}

	public void ObjUpdate()
	{
		if (!started)
			return;
		/*
		distance = (Camera.main.transform.position - transform.position).magnitude;
		if(distance > container.MaxDistance )
		{
			if (distT < SoundManager.Instance.DistanceTimeOut )
				distT += Time.deltaTime;
			else
			{
				reset();
				SoundManager.UnuseSFX(this);
			}
		}
		*/
		if(container.soundType == SoundType.tremolo)
		{
			if(_time > 0)
			{
				float adj = _time / _timeOrig;
				source.volume = Mathf.Lerp(_volOrig, _volTarget, adj);
				source.pitch = Mathf.Lerp(_pitchOrig, _pitch, adj);
				_time -= Time.deltaTime;
			}
			else
			{
				RandomizeNext();
			}
		}

		if (container.useTimeGap != GapType.none)
		{
			
			if (container.useTimeGap == GapType.simple)
			{
				if (_timaGap > 0)
					_timaGap -= Time.deltaTime;
				else
				{
					SetSoundObject();
					source.Play();
					_timaGap = container.GapTime.RandomVec();
				}

			}
			if (container.useTimeGap == GapType.waitTillEnd )
			{
				if (_timaGap > 0 && !source.isPlaying)
					_timaGap -= Time.deltaTime;
				if (_timaGap <= 0 && !source.isPlaying)
				{
					SetSoundObject();
					source.Play();
					_timaGap = container.GapTime.RandomVec();
				}
			}
		}
		else
		{
			if (!container.loop && started && !source.isPlaying)
			{
				reset();
				if(returnToManager)
					SoundManager.UnuseSFX(this);

			}
			if (container.loop && started && !source.isPlaying)
			{
				GetRandomClip();
				if (container.randomize)
				{
					Randomize();
				}
				source.Play();
			}
		}
		if(waitEnd && !source.isPlaying)
		{
			waitEnd = false;
			reset();
		}
		if (container.killOnEnd && !source.isPlaying)
		{
			
			Destroy(gameObject);
		}

		if(PanWithScreen)
		{
			float half = Screen.width / 2;
			Pan = (Camera.main.WorldToScreenPoint(transform.position).x - half) / half;
			source.panStereo = panCurve.Evaluate(Pan);
		}

	}

	void SetSoundObject()
	{
		SoundMain[] ac = container.sounds;
		if (ac == null || ac.Length == 0)
			return;
		sound = ac[UnityEngine.Random.Range(0, ac.Length)];

		source.maxDistance = (container.MaxDistance > 0)? container.MaxDistance : MaxDistance;
		source.volume = _vol;
		source.pitch = sound._pitch;

		if (container.randomize)
		{
			Randomize();
		}

		source.outputAudioMixerGroup = container.mixerGroup;
		source.bypassReverbZones = container.byPssReverbZones;
		source.spatialBlend = (container.SFX2D) ? 0.0f : 1.0f;
		//source.spatialize = !container.SFX2D ;
		if (container.useTimeGap != GapType.waitTillEnd)
			source.loop = container.loop;
		//Debug.Log(AudioSettings.GetSpatializerPluginName());

		if(container.soundType == SoundType.simple)
		{
			//if (sound.clips.Length > 1)
			//{
				GetRandomClip();
			//}
			//else
			//	source.clip = sound.clips[0];
		}
		if(container.soundType == SoundType.tremolo)
		{
			GetRandomClip();
			RandomizeNext();
		}

	//	source.clip = _clip;
		

	}

	void Randomize()
	{
		source.volume = container.randomVolume * _vol;
		source.pitch = container.randomPitch * sound._pitch;

	}

	void RandomizeNext()
	{
		_volOrig = source.volume;
		_pitchOrig = source.pitch;
		_volTarget = container.randomVolume * _vol;
		_pitch = container.randomPitch * sound._pitch;
		_time = _timeOrig = container.randomTime;
	}

	void GetRandomClip()
	{
		source.clip = sound.clips[UnityEngine.Random.Range(0, sound.clips.Length)];

	}
}