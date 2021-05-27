using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Vibrate : MonoBehaviour
{
    // Start is called before the first frame update
    public static bool vib;
    public Image image;
    private int intvib;
    void Start()
    {
         intvib =PlayerPrefs.GetInt("VIB",1);
        if (intvib == 1)
        {
            image.enabled = false;
            vib = true;
        }
        else
        {
            image.enabled = true;
            vib = false;
        }
        Debug.Log("Vib"+vib);
    }

    public void Oncklickvib()
    {
        intvib *= -1;
        if (intvib == 1)
        {
            image.enabled = false;
            vib = true;
        }
        else
        {
            image.enabled = true;
            vib = false; 
        }
        PlayerPrefs.SetInt("VIB", intvib);
        Debug.Log("Vib" + vib);
    }
}
