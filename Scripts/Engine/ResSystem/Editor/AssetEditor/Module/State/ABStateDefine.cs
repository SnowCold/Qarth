//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/Qarth
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace Qarth.Editor
{
    public class ABStateDefine
    {
        public const int NONE = 0;//单纯文件夹
        public const int FILE = 1;//按文件标记
        public const int FOLDER = 2;//按文件夹标记
        public const int MIXED = 3;
        public const int LOST = 4;
    }

    public struct ABState
    {
        public int flag;//当前的标记状态
        public bool isLost;
        private bool m_HasMixed;

        public ABState(int flag, bool isLost)
        {
            this.flag = flag;
            this.isLost = isLost;
            m_HasMixed = false;
        }

        public bool isNoneFlag
        {
            get
            {
                return flag == ABStateDefine.NONE;
            }
        }

        public bool hasMixed
        {
            get
            {
                return m_HasMixed;
            }

            set
            {
                m_HasMixed = value;
            }
        }

        public bool isFolderFlag
        {
            get
            {
                if (isMixedFlag)
                {
                    return false;
                }

                if (((flag ^ ABStateDefine.FOLDER) & ABStateDefine.FOLDER) == 0)
                {
                    return true;
                }

                return false;
            }
        }

        public bool isFileFlag
        {
            get
            {
                if (isMixedFlag)
                {
                    return false;
                }

                if (((flag ^ ABStateDefine.FILE) & ABStateDefine.FILE) == 0)
                {
                    return true;
                }

                return false;
            }
        }

        public bool isMixedFlag
        {
            get
            {
                if (((flag ^ ABStateDefine.MIXED) & ABStateDefine.MIXED) == 0)
                {
                    return true;
                }

                return false;
            }
        }

        public static ABState NONE = new ABState(ABStateDefine.NONE, false);
    }
    
}
