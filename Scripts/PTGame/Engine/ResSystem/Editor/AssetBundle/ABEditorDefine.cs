using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace PTGame.Framework.Editor
{
    public class ABFlagMode
    {
        public const int NONE = 0;//单纯文件夹
        public const int FILE = 1;//按文件标记
        public const int FOLDER = 2;//按文件夹标记
        public const int MIXED = 3;
        public const int LOST = 4;//存在未标记资源

    }
    
}
