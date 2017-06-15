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
    [Serializable]
    public class ABUnit
    {
        public string abName;
        public string[] abDepends;
        public string md5;
        public long buildTime;
        public int fileSize;

        public ABUnit(string name, string[] depends, string md5, int fileSize, long buildTime)
        {
            this.abName = name;
            this.md5 = md5;
            this.fileSize = fileSize;
            this.buildTime = buildTime;
            if (depends == null || depends.Length == 0)
            {

            }
            else
            {
                this.abDepends = depends;
            }
        }

        public override string ToString()
        {
            string result = string.Format("ABName:{0}<{1}>-<{2}>-<{3}B>", abName, md5, buildTime, fileSize);

            if (abDepends == null)
            {
                return result;
            }

            for (int i = 0; i < abDepends.Length; ++i)
            {
                result += string.Format(" #:{0}", abDepends[i]);
            }

            return result;
        }
    }
}
