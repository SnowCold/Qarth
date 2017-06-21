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
    public interface IResLoader
    {
        bool Add2Load(string name, Action<bool, IRes> listener, bool lastOrder = true);
        void ReleaseAllRes();
        void UnloadImage(bool flag);
    }
}
