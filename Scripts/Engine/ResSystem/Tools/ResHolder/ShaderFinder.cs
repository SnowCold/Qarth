//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/Qarth
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Qarth
{
    public class ShaderFinder
    {
        public static Shader FindShaderByFileName(string name)
        {
            Shader result = ResMgr.S.GetRes(name).asset as Shader;

            if (result == null)
            {
                Log.e("Not Find Shader:" + name);
            }

            if (!result.isSupported)
            {
                Log.e("Shader Not Support:" + name);
            }

            return result;
        }

        public static Shader FindShader(string name)
        {
            return ResHolder.S.FindShader(name);
        }

    }
}
