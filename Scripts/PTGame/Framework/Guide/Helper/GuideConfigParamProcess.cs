using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PTGame.Framework
{
    public class GuideConfigParamProcess
    {
        public static object[] ProcessParam(string paramMsg, object[] commonParams)
        {
            if (string.IsNullOrEmpty(paramMsg))
            {
                return null;
            }

            string[] paramsArray = paramMsg.Split(',');

            object[] resultArray = null;
            if (commonParams != null)
            {
                resultArray = new object[paramsArray.Length];
                if (paramsArray.Length > 0)
                {
                    for (int p = 0; p < paramsArray.Length; ++p)
                    {
                        string pps = paramsArray[p] as string;
                        if (pps.StartsWith("#"))
                        {
                            int index = int.Parse(pps.Substring(1));
                            if (index < commonParams.Length)
                            {
                                resultArray[p] = commonParams[index];
                            }
                            else
                            {
                                Log.w("Invalid Param For Process:" + paramMsg);
                            }
                        }
                        else
                        {
                            resultArray[p] = paramsArray[p];
                        }
                    }
                }

            }
            else
            {
                resultArray = paramsArray;
            }

            return resultArray;
        }

    }
}
