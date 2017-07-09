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
    public class BezierUtils
    {
        public static Vector3 Calculate2OrderPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;

            Vector3 p = uu * p0;
            p += 2 * u * t * p1;
            p += tt * p2;

            return p;
        }

        public static Vector3[] Get2OrderPoints(Vector3 p0, Vector3 p1, Vector3 p2, int segmentNum)
        {
            Vector3[] path = new Vector3[segmentNum];
            for (int i = 1; i <= segmentNum; i++)
            {
                float t = i / (float)segmentNum;
                Vector3 pixel = Calculate2OrderPoint(t, p0,
                    p1, p2);
                path[i - 1] = pixel;
            }
            return path;

        }

        public static void Get2OrderPoints(Vector3 p0, Vector3 p1, Vector3 p2, Vector3[] outData)
        {
            int segmentNum = outData.Length;
            for (int i = 1; i <= segmentNum; i++)
            {
                float t = i / (float)segmentNum;
                Vector3 pixel = Calculate2OrderPoint(t, p0,
                    p1, p2);
                outData[i - 1] = pixel;
            }
        }

        public static Vector3 Calculate3OrderPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            Vector3 p = p0 * uuu;
            p += 3 * p1 * t * uu;
            p += 3 * p2 * tt * u;
            p += p3 * ttt;

            return p;
        }

        public static Vector3[] Get3OrderPoints(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, int segmentNum)
        {
            Vector3[] path = new Vector3[segmentNum];
            for (int i = 1; i <= segmentNum; i++)
            {
                float t = i / (float)segmentNum;
                Vector3 pixel = Calculate3OrderPoint(t, p0,
                    p1, p2, p3);
                path[i - 1] = pixel;
            }
            return path;

        }

        public static void Get3OrderPoints(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, Vector3[] outData)
        {
            int segmentNum = outData.Length;
            for (int i = 1; i <= segmentNum; i++)
            {
                float t = i / (float)segmentNum;
                Vector3 pixel = Calculate3OrderPoint(t, p0,
                    p1, p2, p3);
                outData[i - 1] = pixel;
            }
        }
    }
}