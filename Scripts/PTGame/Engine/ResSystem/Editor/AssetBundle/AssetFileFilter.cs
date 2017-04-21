using System;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace PTGame.Framework.Editor
{
    public class AssetFileFilter
    {
        public static bool IsAsset(string fileName)
        {
            if (fileName.EndsWith(".meta") || fileName.EndsWith(".gaf"))
            {
                return false;
            }
            return true;
        }
    }
}
