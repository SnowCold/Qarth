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
    public class LogHelper
    {
        public static void LogArray(string[] value)
        {
            if (value == null || value.Length == 0)
            {
                return;
            }

            for (int i = 0; i < value.Length; ++i)
            {
                Log.i(value[i]);
            }
        }
    }
}
