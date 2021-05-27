using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SoundManagerPlay : MonoBehaviour
{
	[SerializeField]
	bool sendParent;
	[SerializeField]
	Transform parent;

	[SerializeField]
	bool sendPos;
	[SerializeField]
	Transform pos;

	[Header("ObjectSound")]
	public SfxArrayEnum onEableE = SfxArrayEnum.unknown;
	public SfxArrayEnum onDisableE = SfxArrayEnum.unknown;

	public Transform target;

	public void Play(SfxArrayEnum value)
	{
		if (SoundManager.Instance == null || !SoundManager.Instance)
			return;
		if(scrollRect != null)
			scroll = SoundManager.Play(value, (sendPos) ? ((pos != null)?pos.position : transform.position):Vector3.zero,(sendParent)?(parent ?? transform) :null);
		else
			SoundManager.Play(value, (sendPos) ? ((pos != null) ? pos.position : transform.position) : Vector3.zero, (sendParent) ? (parent ?? transform) : null);
	}
	public void PlayPos(SfxArrayEnum value)
	{
		if (SoundManager.Instance == null || !SoundManager.Instance)
			return;
		if (target != null)
			SoundManager.Play(value, target.transform.position, target.transform);
		else
			SoundManager.Play(value,transform.position,transform);
	}

	public void OnEnable()
	{
		if (onEableE != SfxArrayEnum.unknown)
			Play(onEableE);
		
		if (onPointerUpE != SfxArrayEnum.unknown || onPointerUpEDisabled != SfxArrayEnum.unknown)
		{
			if (button == null)
				button = GetComponent<Button>();
			if (button != null)
				button.onClick.AddListener(OnPointerUp);

			if (toggle == null)
				toggle = GetComponent<Toggle>();
			if (toggle != null)
				toggle.onValueChanged.AddListener(OnToggle);

			if (dropdown == null)
				dropdown = GetComponent<Dropdown>();
			if (dropdown != null)
				dropdown.onValueChanged.AddListener(OnDropDowm);

			

#if UNITY_EDITOR
			if (button == null && toggle == null && dropdown == null)
			{
				Debug.LogError("NO BUTTON NO TOGGLE! " , gameObject);
			}
#endif
		}

		if (scrollE != SfxArrayEnum.unknown)
		{
			if (scrollRect == null)
				scrollRect = GetComponent<ScrollRect>();
			if (scrollRect != null)
				scrollRect.onValueChanged.AddListener(ScrollRectValueChanged);
			else
				Debug.LogWarning("Szólj Aranynak!");
		}
	}


	public void OnDisable()
	{
		if (onDisableE != SfxArrayEnum.unknown)
			Play(onDisableE);
		if((onPointerUpE != SfxArrayEnum.unknown || onPointerUpEDisabled != SfxArrayEnum.unknown) )
		{
			if(button != null)
				button.onClick.RemoveListener(OnPointerUp);
			if (toggle != null)
				toggle.onValueChanged.RemoveListener(OnToggle);
			if (dropdown != null)
				dropdown.onValueChanged.RemoveListener(OnDropDowm);
		}
		if (scrollRect != null)
		{
			scrollRect.onValueChanged.RemoveListener(ScrollRectValueChanged);
			//	Debug.Log("ScrollListener removed ");
		}
		contentCount = 0;
	}








	[Header("Scroll")]

	public SfxArrayEnum scrollE = SfxArrayEnum.unknown;
	[SerializeField]
	bool justOnce = false;
	public float scrollMinLimit;
	public GameObject contentFolderToCount;
	float contentCount;
	float lastValueX;
	float lastValueY;

	Vector2 normalPos;
	bool noVelocity;
	bool playing;
	[SerializeField]
	Vector2 minVelocity;
	[SerializeField]
	float stopVelocity;
	[SerializeField]
	float timer = 0.2f;
	float _timer ;
	Vector2 lastVelocity;
	ScrollRect scrollRect;

	SoundObject scroll;

	public void Scroll(float value)
	{
		Debug.Log(value);
	}

	public void PlayMScrollE()
	{
		Play(scrollE);
	}

	public void ScrollRectValueChanged(Vector2 vec)
	{
		if (scrollRect != null && RectTransformUtility.RectangleContainsScreenPoint(scrollRect.viewport,Input.mousePosition))
		{
			if(!justOnce)
			{
				if (contentFolderToCount != null)
					contentCount = contentFolderToCount.transform.childCount;
				float limit = (contentFolderToCount == null ? scrollMinLimit : 1 / contentCount);
				if (Mathf.Abs(vec.x - lastValueX) > limit || Mathf.Abs(vec.y - lastValueY) > limit)
				{
					PlayMScrollE();
					lastValueX = vec.x;
					lastValueY = vec.y;
				}
			}
			else
			{
				if((Mathf.Abs(scrollRect.velocity.x) > minVelocity.x && noVelocity) && scroll == null && _timer <= 0)
				{
					PlayMScrollE();
				}
			}
		}
	}
	
	private void Update()
	{
		if (justOnce)
		{
			
			//if((lastVelocity - scrollRect.velocity).magnitude < stopVelocity)
			if ((scrollRect.velocity).magnitude < stopVelocity)
			{
				noVelocity = true;
				if (scroll != null)
				{
					scroll.reset();
					scroll = null;
					_timer = timer;
				}
			}
			else
			{
				noVelocity = false;
				
			}
			if(_timer > 0)
			{
				_timer -= Time.deltaTime;
			}
			lastVelocity = scrollRect.velocity;
		}
	}
	


	[Header("Click")]

	Button button;
	Toggle toggle;
	Dropdown dropdown;
	public SfxArrayEnum onPointerUpE = SfxArrayEnum.unknown;
	public SfxArrayEnum onPointerUpEDisabled = SfxArrayEnum.unknown;

	public void OnDropDowm(/*Dropdown change*/ int v)
	{
		OnPointerUp();
	}

	public void OnToggle(bool value)
	{
		OnPointerUp();
	}

	public void OnPointerUp()
	{
		if (onPointerUpE != SfxArrayEnum.unknown)
			Play(onPointerUpE);


	}





}

