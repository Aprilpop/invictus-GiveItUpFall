using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndroidPlatform
{
    //打包的日期
    public readonly DateTime buildTime = new DateTime(2020, 3, 1, 0, 0, 0);

    //测试是否过期（结束）
    public bool IsTestOver()
    {
        try
        {
            using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                //获取到Activity
                using (AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    //调用Java方法
                    int testTime = jo.Call<int>("getTestTime");//有效期时长  单位 月
                    var deadline = buildTime.AddMonths(testTime);//截至日期
                    TimeSpan timeSpan = deadline - DateTime.Now;//与本地日期比较
                    return timeSpan.TotalMilliseconds <= 0;
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log("callSdk error:" + ex.Message);
        }
        return false;
    }

    //public string GetDefaultLanguage()
    //{
    //    try
    //    { 
    //        using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
    //        {
    //            //获取到Activity
    //            using (AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"))
    //            {
    //                //调用Java方法
    //                string languageCode = jo.Call<string>("getDefaultLanguageCode");//有效期时长  单位 月

    //                return languageCode;
    //            }
    //        }
    //    }
    //    catch (System.Exception ex)
    //    {
    //        Debug.Log("callSdk error:" + ex.Message);
    //    }
    //    return "en";
    //}

    public bool GetIsDebugMode()
    {
        try
        {
            using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                //获取到Activity
                using (AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                     
                    return jo.Call<bool>("getIsDebugMode"); 

               
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log("callSdk error:" + ex.Message);
        }
        return false;
    }









}
