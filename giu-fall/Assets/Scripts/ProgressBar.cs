using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum ValueDisplayType
{
    None,
    Percent,
    Value
}

public enum ProgressBarDirection
{
    Horizontal,
    Vertical
};

public class ProgressBar : MonoBehaviour {

#if UNITY_EDITOR
    [MenuItem("GameObject/UI/Progressbar")]
    static void DoSomething()
    {
        GameObject root = new GameObject("Progressbar");
        Image rootImage = root.AddComponent<Image>();
        ProgressBar bar =  root.AddComponent<ProgressBar>();

        RectTransform rootTransform = root.GetComponent<RectTransform>();
        rootTransform.sizeDelta = new Vector2(200, 20);


        GameObject go_progress = new GameObject("ProgressImage");
        Image progressImage = go_progress.AddComponent<Image>();
        progressImage.color = Color.gray;
        RectTransform progressRT = go_progress.GetComponent<RectTransform>();
        progressRT.sizeDelta = new Vector2(196, 16);
        progressRT.SetParent(rootTransform);

        GameObject go_Text = new GameObject("ProgressText");
        Text progressText = go_Text.AddComponent<Text>();
        progressText.alignment = TextAnchor.MiddleCenter;
        RectTransform textRT = go_Text.GetComponent<RectTransform>();
        textRT.SetParent(rootTransform);
        go_Text.AddComponent<Outline>();


        bar.progressTransform = progressRT;
        bar.borderOffset = new Vector2(2, 2);
        bar.progressText = progressText;
        bar.MaxValue = 1.0f;
    }
#endif


    [SerializeField]
    protected RectTransform progressTransform;
    [SerializeField]
    Vector2 borderOffset;

    [SerializeField]
    GameObject progressMarker;
    [SerializeField]
    Vector2 markerOffset;
    [SerializeField, Range(0, 1)]
    float[] markers;
    Transform markersTransform;
    [SerializeField]
    GameObject ValueDisplay;

    [SerializeField]
    Text progressText;
    [SerializeField]
    bool simpleValue;

    /* 
    bool valueDisplayInPercent = false;*/
    [SerializeField]
    ValueDisplayType displayType;


    [SerializeField]
    string minValueString = null;
    [SerializeField]
    string maxValueString = null;

    [SerializeField]
    ProgressBarDirection direction = ProgressBarDirection.Horizontal;


    [Space(20)]
    [SerializeField]
    protected float currenValue;
    [SerializeField]
    protected float minValue;
    [SerializeField]
    protected float maxValue;

	public virtual float CurrentValue
    {
        get { return currenValue - minValue; }
        set { {  currenValue = value; OnValidate(); } }
    }

    public virtual float MaxValue
    {
        get { return maxValue+minValue; }
        set { maxValue = value; OnValidate(); }
    }

    public virtual float MinValue
    {
        get { return minValue; }
        set { minValue = value; OnValidate(); }
    }

    public  float Ratio
	{
		get { return currenValue / maxValue; }
	}

    public void OnDrawGizmos()
    {
        RectTransform rectTransform = (transform as RectTransform);
        Rect rc = rectTransform.rect;
        Vector2 pivot = rectTransform.pivot;

        Vector3 pos = transform.position;
        if (direction == ProgressBarDirection.Horizontal)
            pos.x -= rc.width *pivot.x;
        else
            pos.y -= rc.height * pivot.y;

        for (int i = 0; i < markers.Length;++i)
        {
            Vector3 wpos = pos;
            if (direction == ProgressBarDirection.Horizontal)
            {

                float size = (rc.width - borderOffset.x * 2);
                wpos.x += (size * markers[i]) + borderOffset.x;
            }
            else
            {
                float size = (rc.height - borderOffset.y * 2);
                wpos.y += (size * markers[i]) + borderOffset.y;
            }
            wpos.x += markerOffset.x;
            wpos.y += markerOffset.y;
            Gizmos.DrawWireSphere(wpos , (rc.width > rc.height) ? rc.height : rc.width);
        }
    }

    public Vector2 GetWorldPosition( float ratio )
    {
        ratio = Mathf.Clamp(ratio, 0f, 1f);

        Vector3 wpos = transform.position;
        Rect rc = (transform as RectTransform).rect;

        if (direction == ProgressBarDirection.Horizontal)
        {
            float size = (rc.width - borderOffset.x * 2);
            wpos.x += (size * ratio) + borderOffset.x;
        }
        else
        {
            float size = (rc.height - borderOffset.y * 2);
            wpos.y += (size * ratio) + borderOffset.y;
        }
        return wpos;
    }


    protected void OnValidate()
    {
        maxValue = Mathf.Max(0, maxValue);
        currenValue = Mathf.Clamp(currenValue,minValue, MaxValue);
        borderOffset.x = Mathf.Max(0, borderOffset.x);
        borderOffset.y = Mathf.Max(0, borderOffset.y);
        UpdateProgress();
    }

    protected virtual void UpdateProgress()
    {
        float percent = (maxValue > 0) ? CurrentValue / maxValue : 0;
        if (progressTransform != null)
        {
            Rect rc = (transform as RectTransform).rect;

            progressTransform.anchorMin = Vector2.zero;
            progressTransform.anchorMax = Vector2.one;
            progressTransform.offsetMin = borderOffset;

            if (direction == ProgressBarDirection.Horizontal)
            {
                progressTransform.pivot = new Vector2(0, 0.5f);
                float size = (rc.width - borderOffset.x * 2);
                size = (size - size * percent) + borderOffset.x;
                progressTransform.offsetMax = new Vector2(-size , -borderOffset.y);
            }
            else
            {
                progressTransform.pivot = new Vector2(0.5f, 0);
                float size = (rc.height - borderOffset.y * 2);
                size = size - size * percent + borderOffset.y;
                progressTransform.offsetMax = new Vector2(-borderOffset.x, -size);
            }
        }

        if (progressText != null && displayType != ValueDisplayType.None)
            if (simpleValue)
            {
                if (displayType == ValueDisplayType.Percent)
                    progressText.text = ((int)(percent * 100)).ToString();
                else
                {
                    if (currenValue == 0 && minValueString != "")
                        progressText.text = minValueString;
                    else if (currenValue == maxValue && maxValueString != "")
                        progressText.text = maxValueString;
                    else
                        progressText.text = currenValue.ToString("N0");
                }
            }
            else
            {
                if(displayType == ValueDisplayType.Percent)
                    progressText.text = ((int)(percent*100)).ToString() + "/100";
                else
                    progressText.text = currenValue.ToString("N0") + "/" + maxValue.ToString("N0");
                
            }
    }

    private void Awake()
    {
        CreateMarkers(markers);

        if (ValueDisplay)
            ValueDisplay.SetActive( displayType != ValueDisplayType.None );
    }

    public void SetDisplayType(ValueDisplayType newType)
    {
        if (displayType != newType)
        {
            displayType = newType;

            if (ValueDisplay)
                ValueDisplay.SetActive(displayType != ValueDisplayType.None);
        }
    }

    public void CreateMarkers(float[] value)
    {
        if (markersTransform == null)
        {
            GameObject go = new GameObject("Markers");
            RectTransform rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 0.5f);
            rect.anchorMax = new Vector2(0, 0.5f);
            
            markersTransform = go.transform;
            markersTransform.SetParent(transform);       
            rect.anchoredPosition = new Vector2(0, 0);
        }
        else
            DestroyMarkers();

        markers = value;

        RectTransform rectTransform = (transform as RectTransform);
        Rect rc = rectTransform.rect;
        Vector2 pivot = rectTransform.pivot;
        Vector3 pos = transform.position;

        if (direction == ProgressBarDirection.Horizontal)
            pos.x -= rc.width * pivot.x;
        else
            pos.y -= rc.height * pivot.y;

        for (int i = 0; i < markers.Length; ++i)
        {
            Vector3 wpos = Vector3.zero;
            if (direction == ProgressBarDirection.Horizontal)
            {

                float size = (rc.width - borderOffset.x * 2);
                wpos.x += (size * markers[i]) + borderOffset.x;
            }
            else
            {
                float size = (rc.height - borderOffset.y * 2);
                wpos.y += (size * markers[i]) + borderOffset.y;
            }
            wpos.x += markerOffset.x;
            wpos.y += markerOffset.y;
            markersTransform.localScale = Vector3.one;
            GameObject marker = Instantiate(progressMarker, markersTransform);
            marker.transform.localPosition = wpos;
            marker.SetActive(true);
            //Instantiate(progressMarker, wpos, Quaternion.identity, markersTransform).SetActive(true);
        }
    }
    
    public void DestroyMarkers()
    {
        if (markersTransform)
        {
            for (int i = markersTransform.childCount - 1; i >= 0; --i)
                Destroy(markersTransform.GetChild(i).gameObject);
        }

    }

    public Image[] GetMarkersImage()
    {
        Image[] images = new Image[markersTransform.childCount];
        if (markersTransform)
        {
            for (int i = markersTransform.childCount - 1; i >= 0; --i)
                images[i] = markersTransform.GetChild(i).GetComponent<Image>();
        }
        return images;

    }

}
