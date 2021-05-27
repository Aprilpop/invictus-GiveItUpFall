using UnityEngine;

namespace InvictusMoreGames
{
	[RequireComponent(typeof(RectTransform))]
	public class OverrideAnchor : MonoBehaviour
	{
		public AnchorType type;
		public UpdateType updateType = UpdateType.OnStart;
		public NormalAnchorBox normalAnchor;
		public AdvancedAnchorBox advancedAnchor;

		private ScreenOrientation orientation;
		private RectTransform thisRectTransform;
		private float topPosition = 0;
		private float bottomPosition = 0;
		private float rightPosition = 0;
		private float leftPosition = 0;

		private void Awake()
		{
			thisRectTransform = gameObject.GetComponent<RectTransform>();
			orientation = Screen.orientation;
		}

		private void Start()
		{
			orientation = Screen.orientation;
			if (updateType == UpdateType.OnStart)
				ApplyAnchorSettings();
		}

		private void OnEnable()
		{
			if (updateType == UpdateType.OnEnable)
				ApplyAnchorSettings();
		}

		private void Update()
		{
			if (updateType == UpdateType.OnUpdate)
				ApplyAnchorSettings();

			if (updateType == UpdateType.OnOrientationChange && Screen.orientation != orientation)
			{
				orientation = Screen.orientation;
				ApplyAnchorSettings();
			}
		}

		private void LateUpdate()
		{
			if (updateType == UpdateType.OnLateUpdate)
				ApplyAnchorSettings();
		}

		private void FixedUpdate()
		{
			if (updateType == UpdateType.OnFixedUpdate)
				ApplyAnchorSettings();
		}

		public void ApplyAnchorSettings()
		{
			if (type == AnchorType.Normal)
				ApplyNormalAnchor();
			else if (type == AnchorType.Advanced)
				ApplyAdvancedAnchor();
		}

		private void ApplyNormalAnchor()
		{
			bool existsTarget = false;
			try { existsTarget = normalAnchor.target.gameObject != null; } catch { }

			if (!existsTarget)
				return;

			thisRectTransform.SetTopPosition(topPosition = (GetVerticalPositionWithAnchor(normalAnchor.top, normalAnchor.target) + normalAnchor.topPlus));
			thisRectTransform.SetBottomPosition(bottomPosition = (GetVerticalPositionWithAnchor(normalAnchor.bottom, normalAnchor.target) + normalAnchor.bottomPlus));
			thisRectTransform.SetRightPosition(rightPosition = (GetHorizontalPositionWithAnchor(normalAnchor.right, normalAnchor.target) + normalAnchor.rightPlus));
			thisRectTransform.SetLeftPosition(leftPosition = (GetHorizontalPositionWithAnchor(normalAnchor.left, normalAnchor.target) + normalAnchor.leftPlus));
			thisRectTransform.SetSize(new Vector2(Mathf.Abs(rightPosition - leftPosition), Mathf.Abs(topPosition - bottomPosition)));
		}

		private void ApplyAdvancedAnchor()
		{
			bool existsTop = false;
			bool existsBottom = false;
			bool existsRight = false;
			bool existsLeft = false;

			try { existsTop = advancedAnchor.topTarget.gameObject != null; } catch { }
			try { existsBottom = advancedAnchor.bottomTarget.gameObject != null; } catch { }
			try { existsRight = advancedAnchor.rightTarget.gameObject != null; } catch { }
			try { existsLeft = advancedAnchor.leftTarget.gameObject != null; } catch { }

			if (existsTop)
				thisRectTransform.SetTopPosition(topPosition = (GetVerticalPositionWithAnchor(advancedAnchor.top, advancedAnchor.topTarget) + advancedAnchor.topPlus));

			if (existsBottom)
				thisRectTransform.SetBottomPosition(bottomPosition = (GetVerticalPositionWithAnchor(advancedAnchor.bottom, advancedAnchor.bottomTarget) + advancedAnchor.bottomPlus));

			if (existsRight)
				thisRectTransform.SetRightPosition(rightPosition = (GetHorizontalPositionWithAnchor(advancedAnchor.right, advancedAnchor.rightTarget) + advancedAnchor.rightPlus));

			if (existsLeft)
				thisRectTransform.SetLeftPosition(leftPosition = (GetHorizontalPositionWithAnchor(advancedAnchor.left, advancedAnchor.leftTarget) + advancedAnchor.leftPlus));

			if (existsTop && existsBottom)
				thisRectTransform.SetHeight(Mathf.Abs(topPosition - bottomPosition));

			if (existsRight && existsLeft)
				thisRectTransform.SetWidth(Mathf.Abs(rightPosition - leftPosition));
		}

		private float GetVerticalPositionWithAnchor(nGUI_VerticalAnchor anchor, RectTransform target)
		{
			switch (anchor)
			{
				case nGUI_VerticalAnchor.TargetTop:
					return target.GetTopYPosition();

				case nGUI_VerticalAnchor.TargetBottom:
					return target.GetBottomYPosition();

				case nGUI_VerticalAnchor.TargetCenter:
					return target.GetCenterYPosition();
			}

			return target.GetCenterYPosition();
		}

		private float GetHorizontalPositionWithAnchor(nGUI_HorizontalAnchor anchor, RectTransform target)
		{
			switch (anchor)
			{
				case nGUI_HorizontalAnchor.TargetLeft:
					return target.GetLeftXPosition();

				case nGUI_HorizontalAnchor.TargetRight:
					return target.GetRightXPosition();

				case nGUI_HorizontalAnchor.TargetCenter:
					return target.GetCenterXPosition();
			}

			return target.GetCenterXPosition();
		}
	}

	[System.Serializable]
	public class NormalAnchorBox
	{
		public RectTransform target;
		public nGUI_VerticalAnchor top = nGUI_VerticalAnchor.TargetTop;
		public float topPlus;
		public nGUI_HorizontalAnchor right = nGUI_HorizontalAnchor.TargetRight;
		public float rightPlus;
		public nGUI_VerticalAnchor bottom = nGUI_VerticalAnchor.TargetBottom;
		public float bottomPlus;
		public nGUI_HorizontalAnchor left = nGUI_HorizontalAnchor.TargetLeft;
		public float leftPlus;
	}

	[System.Serializable]
	public class AdvancedAnchorBox
	{
		public RectTransform topTarget;
		public nGUI_VerticalAnchor top = nGUI_VerticalAnchor.TargetTop;
		public float topPlus;
		public RectTransform rightTarget;
		public nGUI_HorizontalAnchor right = nGUI_HorizontalAnchor.TargetRight;
		public float rightPlus;
		public RectTransform bottomTarget;
		public nGUI_VerticalAnchor bottom = nGUI_VerticalAnchor.TargetBottom;
		public float bottomPlus;
		public RectTransform leftTarget;
		public nGUI_HorizontalAnchor left = nGUI_HorizontalAnchor.TargetLeft;
		public float leftPlus;
	}

	public static class RectTransformExtensions
	{
		public static void SetLeftBottomPosition(this RectTransform trans, Vector2 newPos)
		{
			SetLeftPosition(trans, newPos.x);
			SetBottomPosition(trans, newPos.y);
		}

		public static void SetLeftTopPosition(this RectTransform trans, Vector2 newPos)
		{
			SetLeftPosition(trans, newPos.x);
			SetTopPosition(trans, newPos.y);
		}

		public static void SetRightBottomPosition(this RectTransform trans, Vector2 newPos)
		{
			SetRightPosition(trans, newPos.x);
			SetBottomPosition(trans, newPos.y);
		}

		public static void SetRightTopPosition(this RectTransform trans, Vector2 newPos)
		{
			SetRightPosition(trans, newPos.x);
			SetTopPosition(trans, newPos.y);
		}

		public static void SetBottomPosition(this RectTransform trans, float y)
		{
			trans.position = new Vector3(trans.position.x, y + (trans.pivot.y * trans.GetHeight()), trans.position.z);
		}

		public static void SetTopPosition(this RectTransform trans, float y)
		{
			trans.position = new Vector3(trans.position.x, y - ((1f - trans.pivot.y) * trans.GetHeight()), trans.position.z);
		}

		public static void SetLeftPosition(this RectTransform trans, float x)
		{
			trans.position = new Vector3(x + (trans.pivot.x * trans.GetWidth()), trans.position.y, trans.position.z);
		}

		public static void SetRightPosition(this RectTransform trans, float x)
		{
			trans.position = new Vector3(x - ((1f - trans.pivot.x) * trans.GetWidth()), trans.position.y, trans.position.z);
		}

		public static void SetPositionOfCenter(this RectTransform trans, Vector2 newPos)
		{
			trans.position = new Vector3(newPos.x, newPos.y, trans.position.z);
		}

		public static void SetPivotAndAnchors(this RectTransform trans, Vector2 aVector)
		{
			trans.pivot = aVector;
			trans.anchorMin = aVector;
			trans.anchorMax = aVector;
		}

		public static void SetSize(this RectTransform trans, Vector2 newSize)
		{
			if (trans.GetSize() == newSize)
				return;

			Vector2 oldSize = trans.rect.size;
			Vector2 deltaSize = newSize - oldSize;
			trans.offsetMin = trans.offsetMin - new Vector2(deltaSize.x * trans.pivot.x, deltaSize.y * trans.pivot.y);
			trans.offsetMax = trans.offsetMax + new Vector2(deltaSize.x * (1f - trans.pivot.x), deltaSize.y * (1f - trans.pivot.y));
		}

		public static void SetWidth(this RectTransform trans, float width)
		{
			if (trans.GetWidth() != width)
				trans.SetSize(new Vector2(width, trans.GetHeight()));
		}

		public static void SetHeight(this RectTransform trans, float height)
		{
			if (trans.GetHeight() != height)
				trans.SetSize(new Vector2(trans.GetWidth(), height));
		}

		public static void SetScaleToOne(this RectTransform trans)
		{
			trans.localScale = new Vector3(1f, 1f, 1f);
		}

		public static Vector2 GetSize(this RectTransform trans)
		{
			return trans.rect.size;
		}

		public static float GetWidth(this RectTransform trans)
		{
			return trans.rect.size.x;
		}

		public static float GetHeight(this RectTransform trans)
		{
			return trans.rect.size.y;
		}

		public static float GetTopYPosition(this RectTransform trans)
		{
			return trans.position.y + (trans.GetHeight() / 2f);
		}

		public static float GetBottomYPosition(this RectTransform trans)
		{
			return trans.position.y - (trans.GetHeight() / 2f);
		}

		public static float GetCenterYPosition(this RectTransform trans)
		{
			return trans.position.y;
		}

		public static float GetLeftXPosition(this RectTransform trans)
		{
			return trans.position.x - (trans.GetWidth() / 2f);
		}

		public static float GetRightXPosition(this RectTransform trans)
		{
			return trans.position.x + (trans.GetWidth() / 2f);
		}

		public static float GetCenterXPosition(this RectTransform trans)
		{
			return trans.position.x;
		}
	}

	public enum AnchorType
	{
		None,
		Normal,
		Advanced
	}

	public enum nGUI_HorizontalAnchor
	{
		TargetLeft,
		TargetRight,
		TargetCenter
	}

	public enum nGUI_VerticalAnchor
	{
		TargetTop,
		TargetBottom,
		TargetCenter
	}

	public enum UpdateType
	{
		OnStart,
		OnEnable,
		OnUpdate,
		OnLateUpdate,
		OnFixedUpdate,
		OnOrientationChange
	}
}