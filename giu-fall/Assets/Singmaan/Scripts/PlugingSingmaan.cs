using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlugingSingmaan
{

    private static PlugingSingmaan instance;
    public static PlugingSingmaan Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new PlugingSingmaan();
             //   instance.isDebugMode = instance.GetIsDebugMode();
            }
            return instance;
        }
    }

    private AndroidPlatform androidPlatform;
    public AndroidPlatform AndroidPlatform
    {
        get
        {
            if (androidPlatform == null)
            {
                androidPlatform = new AndroidPlatform();

            }
            return androidPlatform;
        }
    }



    /*public bool IsTestOver()
    {
#if UNITY_EDITOR
        return false;
#elif UNITY_ANDROID
          return AndroidPlatform.IsTestOver();
#elif UNITY_IOS
        return true;
#endif

    }*/

    public bool isDebugMode;

  /*  public bool GetIsDebugMode()
    {

#if UNITY_EDITOR
        return true;
#elif UNITY_ANDROID
        return AndroidPlatform.GetIsDebugMode();
#elif UNITY_IOS
        return false;
#endif
    }*/



    //    public string GetDefaultLanguage()
    //    {
    //#if UNITY_EDITOR 
    //        return "en";

    //#elif UNITY_ANDROID
    //         return AndroidPlatform.GetDefaultLanguage();

    //#endif

    //    }



}
