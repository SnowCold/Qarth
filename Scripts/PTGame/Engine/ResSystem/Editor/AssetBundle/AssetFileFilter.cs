using System;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace SCFramework.Editor
{
    public class AssetFileFilter
    {
        public static bool IsAsset(string fileName)
        {
            if (fileName.Contains(".meta"))
            {
                return false;
            }
            return true;
        }
    }
}
