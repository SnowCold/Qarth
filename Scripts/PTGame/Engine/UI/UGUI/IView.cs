using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PTGame.Framework
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
        DumpTest,
    }

    public interface IView
    {

    }

    public interface IViewDelegate
    {

    }
}
