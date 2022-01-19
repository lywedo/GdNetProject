using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Net.Share;
using Net.Component;
using System;

public class MyClientSceneManager : Net.Component.SceneManager
{
    public override void OnOperationOther(Operation opt)
    {
        switch (opt.cmd)
        {
            case MyCommand.Attack:
            {
            }
                break;
        }
    }
}