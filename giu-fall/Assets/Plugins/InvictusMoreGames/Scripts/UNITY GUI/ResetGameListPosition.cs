using UnityEngine;

public class ResetGameListPosition : MonoBehaviour
{
	public RectTransform scrollContent;
	public RectTransform scrollContentStartPosition;

	public void OnEnable()
	{
		scrollContent.transform.localPosition = scrollContentStartPosition.transform.localPosition;
	}
}