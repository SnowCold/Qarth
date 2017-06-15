//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/Qarth
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Qarth
{
    //面板管理的内部事件
    public enum ViewEvent
    {
        MIN = 0,
        Action_ClosePanel,
        Action_HidePanel,
        Action_ShowPanel,
        OnPanelOpen,
        OnPanelClose,
        OnParamUpdate,
        OnSortingLayerUpdate,
        DumpTest,
    }

    public interface IView
    {

    }

    public interface IViewDelegate
    {

    }
}
