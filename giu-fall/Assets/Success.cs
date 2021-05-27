using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MenuGUI;

[AddComponentMenu("Menu/Success")]
public class Success : Menu
{

    public override int UniqueID { get { return (int)MenuTypes.Success; } }

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
