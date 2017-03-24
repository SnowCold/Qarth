using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace PTGame.Framework
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