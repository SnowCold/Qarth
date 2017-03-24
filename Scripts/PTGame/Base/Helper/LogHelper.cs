using System;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace PTGame.Framework
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
