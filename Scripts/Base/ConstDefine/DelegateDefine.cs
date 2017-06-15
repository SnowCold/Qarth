//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/Qarth
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using System;
using System.Collections;
using System.Collections.Generic;

namespace Qarth
{
    public delegate void DTableOnParse(byte[] data);
    public delegate void Run();
    public delegate void Run<T>(T v);
    public delegate void Run<T, K>(T v1, K v2);
    
}
