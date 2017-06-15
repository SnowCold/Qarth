//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/Qarth
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Qarth
{
    public partial class TDLanguageTable
    {
        static void CompleteRowAdd(TDLanguage tdData)
        {

        }

        public static string GetFormat(string key, params object[] args)
        {
            string msg = Get(key);
            if (string.IsNullOrEmpty(msg))
            {
                return msg;
            }
            return string.Format(msg, args);
        }
    }
}
