using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{

    void Start()
    {
        GDPR_Aswer ads_aswer = GDPR.Instance.GetRequestTypeAnswer("ads");

        //GDPR Terület ellenörzése
        if (ads_aswer == GDPR_Aswer.None)
        {
            GDPR.Instance.CheckGDPR(succces =>
            {
                if (succces)
                    GDPR.Instance.ShowPopup("ads", "Give It Up! 2", "To improve this app and show relevant ads to you (app neve) collects usage data and device identifiers and shares it with Invictus's data processor partners. By using this app you agree to the Terms of Service and Privacy Policy.");
            });
        }


        //Van-e változás a policy-ben
        GDPR.Instance.CheckModified(result =>
        {
            if (result == Privacy_Modified.Modified && ads_aswer == GDPR_Aswer.Enable)
                GDPR.Instance.ShowPopup("ads", "Give It Up! 2", "Terms of Service and Privacy Policy has changed. By using this app you agree to the updated conditions. ");
        });


    }

}
