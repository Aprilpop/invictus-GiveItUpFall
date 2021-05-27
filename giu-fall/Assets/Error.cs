using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MenuGUI;
using UnityEngine.UI;

[AddComponentMenu("Menu/Error")]
public class Error : Menu
{
    public override int UniqueID { get { return (int)MenuTypes.Error; } }

    [SerializeField]
    Text description;

    protected override void OnShow(ActivateParams activateParams = null)
    {
        if(((ResultsParams)activateParams).message != null)
        {
            description.text = ((ResultsParams)activateParams).message;
        }
    }

    protected override void OnClose()
    {
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            MenuManager.Instance.Back();
    }
}
