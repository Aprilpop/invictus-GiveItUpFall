using UnityEngine;

public class LoadingCircle : MonoBehaviour
{
	public int steps = 12;
	public float delay = 0.2f;

	private float startTime;
	private float angle;

	private void OnEnable()
	{
		startTime = Time.unscaledTime;
		angle = -360.0f / steps;
	}

	private void Update()
	{
		if (Time.unscaledTime > startTime + delay)
		{
			startTime = Time.unscaledTime;
			this.transform.Rotate(0, 0, angle);
		}
	}
}