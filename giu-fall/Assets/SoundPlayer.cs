using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SoundPlayer : MonoBehaviour
{

    [SerializeField]
    SfxArrayEnum soundTypes;

    Toggle toggle;

    Button button;

    [SerializeField]
    bool listener = true;

    public bool selected = true;

    private void Awake()
    {
        if (!listener)
            return;
        toggle = GetComponent<Toggle>();
        button = GetComponent<Button>();
        /*
		if(button && listener)
		{            
            button.onClick.AddListener(() => { Play(soundTypes); });
		}
		if (toggle && listener)
		{
            toggle.onValueChanged.RemoveAllListeners();
            toggle.onValueChanged.AddListener((value) => { Play(soundTypes); });
        }*/
    }


    public void PlayEnum(SfxArrayEnum _types)
    {
        if (selected)
            SoundManager.Play(_types);
    }
}
