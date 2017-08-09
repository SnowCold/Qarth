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
    public class CaptureHelper
    {

        public static IEnumerator Capture(Camera camera, Action<Texture2D> listener)
        {
            if (listener == null || camera == null)
            {
                yield break;
            }

            yield return new WaitForEndOfFrame();

            Rect rect = new Rect(0, 0, Screen.width, Screen.height);

            RenderTexture renderTexture = RenderTexture.GetTemporary(Screen.width, Screen.height);

            camera.targetTexture = renderTexture;

            camera.Render();

            RenderTexture.active = renderTexture;
            Texture2D tex = new Texture2D(Screen.width, Screen.height);

            tex.ReadPixels(rect, 0, 0);

            tex.Apply();

            camera.targetTexture = null;
            RenderTexture.active = null;

            RenderTexture.ReleaseTemporary(renderTexture);

            if (listener != null)
            {
                listener(tex);
            }
        }
    }
}
