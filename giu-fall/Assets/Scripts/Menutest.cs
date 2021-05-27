using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MenuGUI;

[AddComponentMenu("Menu/test")]
public class test : Menu
{

    public override int UniqueID { get { return (int)MenuTypes.test; } }

    protected override void OnShow(ActivateParams activateParams)
    {
    }

    protected override void OnClose()
    {
    }

}
