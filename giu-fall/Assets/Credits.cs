using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MenuGUI;

[AddComponentMenu("Menu/Credits")]
public class Credits : Menu
{

    public override int UniqueID { get { return (int)MenuTypes.Credits; } }

    protected override void OnShow(ActivateParams activateParams)
    {
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
