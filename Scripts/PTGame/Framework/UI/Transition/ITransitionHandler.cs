//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/PTFramework
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCEngine
{
    public interface ITransitionHandler
    {
        AbstractPanel transitionPanel
        {
            get;
        }
        void OnTransitionPrepareFinish();
        void OnTransitionInFinish();
        void OnTransitionOutFinish();
    }

}
