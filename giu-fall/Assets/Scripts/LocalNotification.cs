using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_ANDROID
using Assets.SimpleAndroidNotifications;
#endif


public class NotificationRequestData
{
    public long activationTime;
    public string title;
    public string subtitle;
}


public interface NotificationRequester
{
    bool OnNotificationRequest(out NotificationRequestData[] requestData);
}


public class LocalNotification : MonoBehaviour {

    public const string TITLE = "Give IT Up! Fall";

    static LocalNotification instance;
    public static LocalNotification Instance
    {
        get
        {
            if (instance == null)
                new GameObject("LocalNotification", typeof(LocalNotification));
            return instance;
        }
    }


    public List<NotificationRequester> notificationRequesters = new List<NotificationRequester>();


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
             #if UNITY_IOS
               UnityEngine.iOS.NotificationServices.RegisterForNotifications(UnityEngine.iOS.NotificationType.Alert | UnityEngine.iOS.NotificationType.Badge | UnityEngine.iOS.NotificationType.Sound);
            #endif
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnApplicationPause(bool isPause)
    {
        if (isPause)
        {
            Debug.Log("LocalNotification: Pause");
            SetNotifications();
        }
        else
        {
#if UNITY_ANDROID
            NotificationManager.CancelAll();
#elif UNITY_IOS

            UnityEngine.iOS.NotificationServices.CancelAllLocalNotifications();
            UnityEngine.iOS.NotificationServices.ClearLocalNotifications();
#endif
        }
    }

    public void CancelNotifications()
    {
#if UNITY_ANDROID
        NotificationManager.CancelAll();
#elif UNITY_IOS

            UnityEngine.iOS.NotificationServices.CancelAllLocalNotifications();
            UnityEngine.iOS.NotificationServices.ClearLocalNotifications();
#endif
    }

    void OnApplicationQuit()
    {
        Debug.Log("LocalNotification: Quit");
        //SetNotifications();
    }

    void SetNotifications()
    {
        NotificationRequestData[] datas = null;
        foreach (NotificationRequester requester in notificationRequesters)
        {
            if (requester.OnNotificationRequest(out datas))
            {
                foreach (NotificationRequestData data in datas)
                    SetNotification(data);
            }
        }
    }


    void SetNotification(NotificationRequestData requestData)
    {
#if UNITY_ANDROID
        System.TimeSpan span = new System.DateTime(requestData.activationTime) - DateTime.Now;
        NotificationManager.SendWithAppIcon( span, requestData.title, requestData.subtitle, new Color(1, 0.3f, 0.15f), NotificationIcon.Bell);
#elif UNITY_IOS
        UnityEngine.iOS.LocalNotification notif = new UnityEngine.iOS.LocalNotification()
        {
            fireDate = new System.DateTime(requestData.activationTime),
            alertAction = requestData.title,
            alertBody = requestData.subtitle,
            soundName = UnityEngine.iOS.LocalNotification.defaultSoundName
        };
        UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(notif);
#endif
    }
}