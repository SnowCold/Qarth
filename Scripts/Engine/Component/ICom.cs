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
    public interface ICom
    {
        AbstractActor actor
        {
            get;
        }

        int comOrder
        {
            get;
        }

        //自身的初始化工作
        void AwakeCom(AbstractActor actor);
        //和其它组件有关的初始化工作
        void OnComStart();
        void OnComEnable();
        void OnComUpdate(float dt);
        void OnComLateUpdate(float dt);
        void OnComDisable();
        void DestroyCom();
    }
}
